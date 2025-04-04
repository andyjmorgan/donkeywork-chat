using System;
using System.Collections.Generic;
using DonkeyWork.Chat.Common.Providers;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonkeyWork.Chat.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProviders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Dictionary<UserProviderDataKeyType, string>>(
                name: "Data",
                table: "BaseUserEntity",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpiresAt",
                table: "BaseUserEntity",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProviderType",
                table: "BaseUserEntity",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "Scopes",
                table: "BaseUserEntity",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "ProviderType",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "Scopes",
                table: "BaseUserEntity");
        }
    }
}
