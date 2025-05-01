namespace AuctionApp.ViewModels;

public class AuctionListViewModel
{
	public List<Lab2Auction.Models.Auction> Auctions { get; set; } = new();
	public string? SearchQuery { get; set; }
	public int CurrentPage { get; set; }
	public int TotalPages { get; set; }
}

