using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DonkeyWork.Chat.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingPromptIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Prompts_Title_UserId",
                schema: "ApiPersistence",
                table: "Prompts",
                columns: new[] { "Title", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Prompts_Title_UserId",
                schema: "ApiPersistence",
                table: "Prompts");
        }
    }
}
