// ------------------------------------------------------
// <copyright file="20250319111506_AddPrompts.cs" company="DonkeyWork.Dev">
// Provided as is by (c) DonkeyWork.Dev. No warranty or support is given.
// </copyright>
// ------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonkeyWork.Chat.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPrompts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "BaseUserEntity",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BaseUserEntity",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromptEntity_Title",
                table: "BaseUserEntity",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsageCount",
                table: "BaseUserEntity",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "PromptEntity_Title",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "UsageCount",
                table: "BaseUserEntity");
        }
    }
}
