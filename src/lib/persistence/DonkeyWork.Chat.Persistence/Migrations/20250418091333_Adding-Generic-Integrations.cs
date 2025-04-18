using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonkeyWork.Chat.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingGenericIntegrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GenericProviders",
                schema: "ApiPersistence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderType = table.Column<int>(type: "integer", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    Configuration = table.Column<string>(type: "character varying(10240)", maxLength: 10240, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericProviders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenericProviders_ProviderType_UserId",
                schema: "ApiPersistence",
                table: "GenericProviders",
                columns: new[] { "ProviderType", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenericProviders",
                schema: "ApiPersistence");
        }
    }
}
