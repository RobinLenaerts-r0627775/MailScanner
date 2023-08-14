//Add configuration with user secrets and environment variables

var builder = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

var configuration = builder.Build();

//Setup Serilog
var loggerConfig = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}");
if (!string.IsNullOrWhiteSpace(configuration["LogLocation"]))
{
    loggerConfig = loggerConfig.WriteTo.File(configuration["LogLocation"] + "/logfile.log", outputTemplate:
        "[{Timestamp:dd:MM:yyyy - HH:mm:ss} {Level}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day);
}
var logger = loggerConfig.CreateLogger();

//Setup mysql connection
var optionsBuilder = new DbContextOptionsBuilder<MailScannerContext>();
var dbHost = configuration["DB_HOST"];
if (string.IsNullOrEmpty(dbHost))
{
    logger.Information("DB_HOST not set, add it to the environment variables please.");
    return;
}
var dbPort = configuration["DB_PORT"];
if (string.IsNullOrEmpty(dbPort))
{
    logger.Information("DB_PORT not set, using port 3306.");
    dbPort = "3306";
}
var dbUser = configuration["DB_USER"];
if (string.IsNullOrEmpty(dbUser))
{
    logger.Information("DB_USER not set, add it to the environment variables please.");
    return;
}
var dbPassword = configuration["DB_PASSWORD"];
if (string.IsNullOrEmpty(dbPassword))
{
    logger.Information("DB_PASSWORD not set, add it to the environment variables please.");
    return;
}
var connectionString = $"Server={dbHost};Port={dbPort};Database=MailScanner;Uid={dbUser};Pwd={dbPassword};";
optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
var context = new MailScannerContext(optionsBuilder.Options);

logger.Information("Starting migration...");
context.Database.Migrate();
logger.Information("Migration done.");

//Read mailbox
using (var client = new ImapClient())
{
    client.Connect(configuration["IMAP_HOST"], 993, true);

    client.Authenticate(configuration["IMAP_USER"], configuration["IMAP_PASSWORD"]);

    //open archive folder

    var archiveFolder = client.Inbox.GetSubfolder("Archive");
    archiveFolder.Open(FolderAccess.ReadWrite);

    // get last message
    var uids = archiveFolder.Search(SearchQuery.SubjectContains("Invoice").Or(SearchQuery.SubjectContains("Factuur").Or(SearchQuery.SubjectContains("Rekening").Or(SearchQuery.BodyContains("Invoice").Or(SearchQuery.BodyContains("Factuur").Or(SearchQuery.BodyContains("Rekening")))))));

    foreach (var uid in uids)
    {
        var message = client.Inbox.GetSubfolder("Archive").GetMessage(uid);
        var invoice = new Invoice
        {
            Date = message.Date.Date,
            Subject = message.Subject,
            Body = message.TextBody,
            Sender = message.From.ToString(),
            Receiver = message.To.ToString(),

        };
        if (message.Attachments.Any())
        {
            foreach (var attachment in message.Attachments)
            {
                attachment.WriteTo($"D:\\{attachment.ContentDisposition.FileName}");
                invoice.Attachments.Add(new Attachment
                {
                    AttachmentName = attachment.ContentDisposition.FileName,
                    Attachmentlocation = $"D:\\{attachment.ContentDisposition.FileName}"
                });
            }
            //flag the message as processed

        }
    }
}