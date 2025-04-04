using System;
using System.Collections.Generic;
using DonkeyWork.Chat.Common.Providers;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonkeyWork.Chat.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TpcMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaseUserEntity_BaseUserEntity_ConversationId",
                table: "BaseUserEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_BaseUserEntity_BaseUserEntity_ToolCallEntity_ConversationId",
                table: "BaseUserEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BaseUserEntity",
                table: "BaseUserEntity");

            migrationBuilder.DropIndex(
                name: "IX_BaseUserEntity_ConversationId",
                table: "BaseUserEntity");

            migrationBuilder.DropIndex(
                name: "IX_BaseUserEntity_MessagePairId",
                table: "BaseUserEntity");

            migrationBuilder.DropIndex(
                name: "IX_BaseUserEntity_ToolCallEntity_ConversationId",
                table: "BaseUserEntity");

            migrationBuilder.DropIndex(
                name: "IX_BaseUserEntity_ToolCallEntity_MessagePairId",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "MessageCount",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "MessageOwner",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "MessagePairId",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "PromptEntity_Title",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "Request",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "Response",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "ToolCallEntity_ConversationId",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "ToolCallEntity_MessagePairId",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "ToolName",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "UsageCount",
                table: "BaseUserEntity");

            migrationBuilder.EnsureSchema(
                name: "ApiPersistence");

            migrationBuilder.RenameTable(
                name: "BaseUserEntity",
                newName: "UserTokens",
                newSchema: "ApiPersistence");

            migrationBuilder.AlterColumn<List<string>>(
                name: "Scopes",
                schema: "ApiPersistence",
                table: "UserTokens",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProviderType",
                schema: "ApiPersistence",
                table: "UserTokens",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<Dictionary<UserProviderDataKeyType, string>>(
                name: "Data",
                schema: "ApiPersistence",
                table: "UserTokens",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(Dictionary<UserProviderDataKeyType, string>),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTokens",
                schema: "ApiPersistence",
                table: "UserTokens",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Conversations",
                schema: "ApiPersistence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    MessageCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prompts",
                schema: "ApiPersistence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    UsageCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prompts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConversationMessages",
                schema: "ApiPersistence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessagePairId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageOwner = table.Column<int>(type: "integer", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationMessages_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalSchema: "ApiPersistence",
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToolCalls",
                schema: "ApiPersistence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessagePairId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToolName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Request = table.Column<string>(type: "text", nullable: false),
                    Response = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToolCalls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToolCalls_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalSchema: "ApiPersistence",
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessages_ConversationId",
                schema: "ApiPersistence",
                table: "ConversationMessages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessages_MessagePairId",
                schema: "ApiPersistence",
                table: "ConversationMessages",
                column: "MessagePairId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolCalls_ConversationId",
                schema: "ApiPersistence",
                table: "ToolCalls",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_ToolCalls_MessagePairId",
                schema: "ApiPersistence",
                table: "ToolCalls",
                column: "MessagePairId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversationMessages",
                schema: "ApiPersistence");

            migrationBuilder.DropTable(
                name: "Prompts",
                schema: "ApiPersistence");

            migrationBuilder.DropTable(
                name: "ToolCalls",
                schema: "ApiPersistence");

            migrationBuilder.DropTable(
                name: "Conversations",
                schema: "ApiPersistence");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTokens",
                schema: "ApiPersistence",
                table: "UserTokens");

            migrationBuilder.RenameTable(
                name: "UserTokens",
                schema: "ApiPersistence",
                newName: "BaseUserEntity");

            migrationBuilder.AlterColumn<List<string>>(
                name: "Scopes",
                table: "BaseUserEntity",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<int>(
                name: "ProviderType",
                table: "BaseUserEntity",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<Dictionary<UserProviderDataKeyType, string>>(
                name: "Data",
                table: "BaseUserEntity",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(Dictionary<UserProviderDataKeyType, string>),
                oldType: "jsonb");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "BaseUserEntity",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConversationId",
                table: "BaseUserEntity",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BaseUserEntity",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "BaseUserEntity",
                type: "character varying(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "BaseUserEntity",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MessageCount",
                table: "BaseUserEntity",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MessageOwner",
                table: "BaseUserEntity",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MessagePairId",
                table: "BaseUserEntity",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromptEntity_Title",
                table: "BaseUserEntity",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Request",
                table: "BaseUserEntity",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "BaseUserEntity",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "BaseUserEntity",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ToolCallEntity_ConversationId",
                table: "BaseUserEntity",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ToolCallEntity_MessagePairId",
                table: "BaseUserEntity",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToolName",
                table: "BaseUserEntity",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsageCount",
                table: "BaseUserEntity",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BaseUserEntity",
                table: "BaseUserEntity",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BaseUserEntity_ConversationId",
                table: "BaseUserEntity",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseUserEntity_MessagePairId",
                table: "BaseUserEntity",
                column: "MessagePairId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseUserEntity_ToolCallEntity_ConversationId",
                table: "BaseUserEntity",
                column: "ToolCallEntity_ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_BaseUserEntity_ToolCallEntity_MessagePairId",
                table: "BaseUserEntity",
                column: "ToolCallEntity_MessagePairId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaseUserEntity_BaseUserEntity_ConversationId",
                table: "BaseUserEntity",
                column: "ConversationId",
                principalTable: "BaseUserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BaseUserEntity_BaseUserEntity_ToolCallEntity_ConversationId",
                table: "BaseUserEntity",
                column: "ToolCallEntity_ConversationId",
                principalTable: "BaseUserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
