using Lab2Auction.Models;
namespace Lab2Auction.Services
{
    public interface IAuctionService
    {
        Task<List<Auction>> GetOngoingAuctionsAsync();
        Task<Auction> GetAuctionDetailsAsync(int auctionId);
        Task<Auction> CreateAuctionAsync(Auction auction);
        Task<List<Auction>> GetAuctionsByUserIdAsync(string userId);
        Task<bool> UpdateAuctionAsync(AuctionEditModel updatedAuction, string userId);
        Task<bool> DeleteAuctionAsync(int auctionId, string userId);
		Task<bool> SellAuctionAsync(int auctionId, string userId);
		Task<List<Auction>> GetWonAuctionsByUserIdAsync(string userId);
		Task<bool> SellAuctionToBidderAsync(int auctionId, int bidId, string userId);
        IQueryable<Auction> GetUserAuctionsQuery(string userId);

	}
}
