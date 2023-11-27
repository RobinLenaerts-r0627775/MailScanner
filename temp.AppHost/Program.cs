var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.MailScanner_Api>("api");

builder.AddProject<Projects.MailScanner_Frontend>("frontend")
.WithReference(api);

builder.Build().Run();
