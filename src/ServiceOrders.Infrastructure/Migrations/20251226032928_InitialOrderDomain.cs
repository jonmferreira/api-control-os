using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServiceOrders.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialOrderDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChecklistTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PublishedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomInputComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    JsonBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomInputComponents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyTargets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    TargetEntries = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyTargets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrdersOfService",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    AssignedTechnician = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OpenedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RejectedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ClosingNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ChecklistResponseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersOfService", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistTemplateItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChecklistTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HasCustomInput = table.Column<bool>(type: "bit", nullable: false),
                    CustomInputComponentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DefaultOutcome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistTemplateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistTemplateItems_ChecklistTemplates_ChecklistTemplateId",
                        column: x => x.ChecklistTemplateId,
                        principalTable: "ChecklistTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistTemplateItems_CustomInputComponents_CustomInputComponentId",
                        column: x => x.CustomInputComponentId,
                        principalTable: "CustomInputComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderOfServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChecklistTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistResponses_ChecklistTemplates_ChecklistTemplateId",
                        column: x => x.ChecklistTemplateId,
                        principalTable: "ChecklistTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistResponses_OrdersOfService_OrderOfServiceId",
                        column: x => x.OrderOfServiceId,
                        principalTable: "OrdersOfService",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistResponseItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChecklistResponseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChecklistTemplateItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomInputPayload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistResponseItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistResponseItems_ChecklistResponses_ChecklistResponseId",
                        column: x => x.ChecklistResponseId,
                        principalTable: "ChecklistResponses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecklistResponseItems_ChecklistTemplateItems_ChecklistTemplateItemId",
                        column: x => x.ChecklistTemplateItemId,
                        principalTable: "ChecklistTemplateItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhotoAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderOfServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChecklistResponseItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoAttachments_ChecklistResponseItems_ChecklistResponseItemId",
                        column: x => x.ChecklistResponseItemId,
                        principalTable: "ChecklistResponseItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PhotoAttachments_OrdersOfService_OrderOfServiceId",
                        column: x => x.OrderOfServiceId,
                        principalTable: "OrdersOfService",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "ChecklistTemplates",
                columns: new[] { "Id", "IsPublished", "PublishedBy", "Title", "UpdatedAt" },
                values: new object[] { new Guid("f0d2fb49-7b89-42cc-9e6b-5f2f0e57291b"), true, "engenharia@serviceorders.local", "Checklist Veicular - v1", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "CustomInputComponents",
                columns: new[] { "Id", "JsonBody", "Name", "UpdatedAt" },
                values: new object[] { new Guid("a3137f9c-6126-4bf4-9dd4-1f3d9cb1aa10"), "{\n  \"fields\": [\n    { \"name\": \"placa\", \"label\": \"Placa\", \"type\": \"text\", \"required\": true },\n    { \"name\": \"chassi\", \"label\": \"Chassi\", \"type\": \"text\", \"required\": true },\n    { \"name\": \"modelo\", \"label\": \"Modelo\", \"type\": \"text\", \"required\": true },\n    { \"name\": \"ano\", \"label\": \"Ano\", \"type\": \"number\", \"required\": true }\n  ]\n}", "DadosCarro", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "Name", "PasswordHash", "Role", "UpdatedAt" },
                values: new object[] { new Guid("8a2c929f-3a3f-4f67-9d92-56977d042793"), new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "admin@serviceorders.local", true, "Administrador OS", "100000.YIbxL2Hjg2HsztaG1d2ZsQ==.aQ9YkZB0C63nQtw4LHZy8pqhQMR+fb68TyK3En1XagA=", "Admin", null });

            migrationBuilder.InsertData(
                table: "ChecklistTemplateItems",
                columns: new[] { "Id", "ChecklistTemplateId", "CustomInputComponentId", "DefaultOutcome", "Description", "DisplayOrder", "HasCustomInput" },
                values: new object[,]
                {
                    { new Guid("a7c75cf0-7016-4d72-9e76-4e927c6de7a8"), new Guid("f0d2fb49-7b89-42cc-9e6b-5f2f0e57291b"), new Guid("a3137f9c-6126-4bf4-9dd4-1f3d9cb1aa10"), "Approved", "Registrar dados do veículo", 1, true },
                    { new Guid("d2ccf0a2-49b9-4d3f-97a7-a1a2d1bdc1d5"), new Guid("f0d2fb49-7b89-42cc-9e6b-5f2f0e57291b"), null, "Approved", "Inspeção visual da carroceria", 0, false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistResponseItems_ChecklistResponseId",
                table: "ChecklistResponseItems",
                column: "ChecklistResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistResponseItems_ChecklistTemplateItemId",
                table: "ChecklistResponseItems",
                column: "ChecklistTemplateItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistResponses_ChecklistTemplateId",
                table: "ChecklistResponses",
                column: "ChecklistTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistResponses_OrderOfServiceId",
                table: "ChecklistResponses",
                column: "OrderOfServiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTemplateItems_ChecklistTemplateId",
                table: "ChecklistTemplateItems",
                column: "ChecklistTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistTemplateItems_CustomInputComponentId",
                table: "ChecklistTemplateItems",
                column: "CustomInputComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyTargets_Year_Month",
                table: "MonthlyTargets",
                columns: new[] { "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_TokenHash",
                table: "PasswordResetTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoAttachments_ChecklistResponseItemId",
                table: "PhotoAttachments",
                column: "ChecklistResponseItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PhotoAttachments_OrderOfServiceId",
                table: "PhotoAttachments",
                column: "OrderOfServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyTargets");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "PhotoAttachments");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ChecklistResponseItems");

            migrationBuilder.DropTable(
                name: "ChecklistResponses");

            migrationBuilder.DropTable(
                name: "ChecklistTemplateItems");

            migrationBuilder.DropTable(
                name: "OrdersOfService");

            migrationBuilder.DropTable(
                name: "ChecklistTemplates");

            migrationBuilder.DropTable(
                name: "CustomInputComponents");
        }
    }
}
