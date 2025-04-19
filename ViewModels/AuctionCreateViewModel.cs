using System.ComponentModel.DataAnnotations;

namespace AuctionApp.ViewModels
{
	public class AuctionCreateViewModel
	{
		[Required]
		public string Name { get; set; }

		public string? Description { get; set; }

		[Required]
		public decimal StartingPrice { get; set; }

		[Required]
		[FutureDate(ErrorMessage = "End date must be in the future.")]
		public DateTime EndDate { get; set; }

		public IFormFileCollection? Images { get; set; }
	}
}

