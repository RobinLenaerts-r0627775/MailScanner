using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MailScanner.Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddAttachmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "attachmentName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "attachmentlocation",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "subject",
                table: "Invoices",
                newName: "Subject");

            migrationBuilder.RenameColumn(
                name: "sender",
                table: "Invoices",
                newName: "Sender");

            migrationBuilder.RenameColumn(
                name: "receiver",
                table: "Invoices",
                newName: "Receiver");

            migrationBuilder.RenameColumn(
                name: "body",
                table: "Invoices",
                newName: "Body");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Invoices",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AttachmentName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Attachmentlocation = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InvoiceId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachment_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_InvoiceId",
                table: "Attachment",
                column: "InvoiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Invoices",
                newName: "subject");

            migrationBuilder.RenameColumn(
                name: "Sender",
                table: "Invoices",
                newName: "sender");

            migrationBuilder.RenameColumn(
                name: "Receiver",
                table: "Invoices",
                newName: "receiver");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "Invoices",
                newName: "body");

            migrationBuilder.AddColumn<string>(
                name: "attachmentName",
                table: "Invoices",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "attachmentlocation",
                table: "Invoices",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
