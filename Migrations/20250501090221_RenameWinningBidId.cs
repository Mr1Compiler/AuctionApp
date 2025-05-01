using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionApp.Migrations
{
    /// <inheritdoc />
    public partial class RenameWinningBidId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropIndex(
                name: "IX_Auction_WinningBidId1",
                table: "Auction");

            migrationBuilder.CreateIndex(
                name: "IX_Auction_WinningBidId",
                table: "Auction",
                column: "WinningBidId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auction_Bid_WinningBidId",
                table: "Auction",
                column: "WinningBidId",
                principalTable: "Bid",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auction_Bid_WinningBidId",
                table: "Auction");

            migrationBuilder.DropIndex(
                name: "IX_Auction_WinningBidId",
                table: "Auction");

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
    }
}
