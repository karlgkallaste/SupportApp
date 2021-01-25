using Microsoft.EntityFrameworkCore.Migrations;

namespace SupportApp.Migrations
{
    public partial class CategoriesForTickets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Ticket",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_CategoryId",
                table: "Ticket",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Categories_CategoryId",
                table: "Ticket",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Categories_CategoryId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_CategoryId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Ticket");
        }
    }
}
