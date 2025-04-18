using Lab2Auction.Models;

namespace AuctionApp.Models
{
	public class AuctionImage
	{
		public int Id { get; set; }
		public string ImagePath { get; set; } // e.g., "/images/auctions/filename.jpg"

		public int AuctionId { get; set; }
		public Auction Auction { get; set; }
	}
}
