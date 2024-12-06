using Lab2Auction.Models;
using System.Security.Claims;
namespace Lab2Auction.Services
{
    public interface IBidService
    {
        Task<List<Bid>> GetAllBidsAsync();
        Task<List<Bid>> GetBidsByUserAsync(ClaimsPrincipal user);
        Task<List<Bid>> GetBidsForAuctionAsync(int auctionId);
        Task<Bid> GetBidDetailsAsync(int bidId);
        Task<bool> DeleteBidAsync(int bidId, ClaimsPrincipal user);
        Task<bool> CanUserPlaceBidAsync(int auctionId, ClaimsPrincipal user);
        Task<Bid> PlaceBidAsync(Bid bid, ClaimsPrincipal user);
    }
}
