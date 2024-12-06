using Lab2Auction.Data;
using Lab2Auction.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace Lab2Auction.Services
{
    public class BidService : IBidService
    {
        private readonly AuctionDbContext _auctionContext;
        public BidService(AuctionDbContext auctionContext)
        {
            _auctionContext = auctionContext;
        }

        public async Task<List<Bid>> GetAllBidsAsync()
        {
            return await _auctionContext.Bid
                .Include(b => b.Auction)
                .ToListAsync();
        }

        public async Task<List<Bid>> GetBidsByUserAsync(ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _auctionContext.Bid
                .Include(b => b.Auction)
                .Where(b => b.UserId == userId && b.Auction.EndDate > DateTime.Now)
                .OrderByDescending(b => b.BidDate)
                .ToListAsync();
        }

        public async Task<List<Bid>> GetBidsForAuctionAsync(int auctionId)
        {
            return await _auctionContext.Bid
                .Where(b => b.AuctionId == auctionId)
                .OrderByDescending(b => b.Amount)
                .ToListAsync();
        }

        public async Task<Bid> GetBidDetailsAsync(int bidId)
        {
            return await _auctionContext.Bid
                .Include(b => b.Auction)
                .FirstOrDefaultAsync(m => m.Id == bidId);
        }

        public async Task<bool> DeleteBidAsync(int bidId, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var bid = await _auctionContext.Bid
                .Where(b => b.Id == bidId && b.UserId == userId)
                .SingleOrDefaultAsync();
            if (bid == null)
            {
                return false;
            }
            _auctionContext.Bid.Remove(bid);
            await _auctionContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanUserPlaceBidAsync(int auctionId, ClaimsPrincipal user)
        {
            var auction = await _auctionContext.Auction.FindAsync(auctionId);
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            // Check if auction is null, if the user is the owner, or if the auction has ended
            if (auction == null || auction.UserId == userId || auction.EndDate <= DateTime.Now)
            {
                return false;
            }
            return true;
        }

        public async Task<Bid> PlaceBidAsync(Bid bid, ClaimsPrincipal user)
        {
            var auction = await _auctionContext.Auction.FindAsync(bid.AuctionId);
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            // if the auction exists, if the current user is the owner, or if the auction has already ended
            if (auction == null || auction.UserId == userId || auction.EndDate <= DateTime.Now)
            {
                return null;
            }
            // if the bid is higher than the current highest bid or the starting price
            decimal currentHighestBid = _auctionContext.Bid
                   .Where(b => b.AuctionId == bid.AuctionId)
                   .Max(b => (decimal?)b.Amount) ?? auction.StartingPrice;
            if (bid.Amount <= currentHighestBid)
            {
                return null;
            }
            bid.UserId = userId;
            bid.BidDate = DateTime.Now;
            _auctionContext.Add(bid);
            await _auctionContext.SaveChangesAsync();
            return bid;
        }
    }
}
