using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionApp.Migrations.AuctionDb
{
    /// <inheritdoc />
    public partial class AddAuctionStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Auction",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Auction");
        }
    }
}
