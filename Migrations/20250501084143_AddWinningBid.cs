using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionApp.Migrations
{
    /// <inheritdoc />
    public partial class AddWinningBid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WinningBidId",
                table: "Auction",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WinningBidId1",
                table: "Auction",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auction_WinningBidId1",
                table: "Auction",
                column: "WinningBidId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_Bid_WinningBidId1",
                table: "Auction",
                column: "WinningBidId1",
                principalTable: "Bid",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auction_Bid_WinningBidId1",
                table: "Auction");

            migrationBuilder.DropIndex(
                name: "IX_Auction_WinningBidId1",
                table: "Auction");

            migrationBuilder.DropColumn(
                name: "WinningBidId",
                table: "Auction");

            migrationBuilder.DropColumn(
                name: "WinningBidId1",
                table: "Auction");
        }
    }
}
