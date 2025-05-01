using Lab2Auction.Models;
using System.ComponentModel.DataAnnotations;

namespace AuctionApp.ViewModels
{
	public class AuctionEditModel
	{
		public int Id { get; set; }

		[Required]
		public string? Name { get; set; }

		[Required]
		public string? Description { get; set; }

		[Required]
		[DataType(DataType.DateTime)]
		public DateTime EndDate { get; set; }

		public List<AuctionImage> Images { get; set; } = new();
		public List<Bid> Bids { get; set; } = new();
		public IFormFileCollection? NewImages { get; set; }
	}
}

