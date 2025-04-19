using Lab2Auction.Data;
using Lab2Auction.Models;
using Microsoft.EntityFrameworkCore;
namespace Lab2Auction.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly AuctionDbContext _auctionContext;
        public AuctionService(AuctionDbContext auctionContext)
        {
            _auctionContext = auctionContext;
        }

		public async Task<List<Auction>> GetOngoingAuctionsAsync()
		{
			return await _auctionContext.Auction
				.Include(a => a.Images) // ✅ include images
				.Where(a => a.EndDate > DateTime.Now)
				.OrderBy(a => a.EndDate)
				.ToListAsync();
		}

		public async Task<Auction> GetAuctionDetailsAsync(int auctionId)
		{
			return await _auctionContext.Auction
				.Include(a => a.Bids.OrderByDescending(b => b.Amount))
				.Include(a => a.Images) // ✅ include images
				.FirstOrDefaultAsync(m => m.Id == auctionId); // ❌ removed the EndDate filter
		}


		public async Task<Auction> CreateAuctionAsync(Auction auction)
        {
            _auctionContext.Add(auction);
            await _auctionContext.SaveChangesAsync();
            return auction;
        }

        public async Task<List<Auction>> GetAuctionsByUserIdAsync(string userId)
        {
            return await _auctionContext.Auction
                .Where(a => a.UserId == userId && a.EndDate > DateTime.Now)
                .OrderByDescending(a => a.EndDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateAuctionAsync(AuctionEditModel updatedAuction, string userId)
        {
            var auction = await _auctionContext.Auction
                .Where(a => a.Id == updatedAuction.Id && a.UserId == userId)
                .SingleOrDefaultAsync();
            if (auction == null || auction.EndDate <= DateTime.Now)
            {
                return false;
            }
            auction.Description = updatedAuction.Description;
            _auctionContext.Update(auction);
            await _auctionContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAuctionAsync(int auctionId, string userId)
        {
            var auction = await _auctionContext.Auction
                .Where(a => a.Id == auctionId && a.UserId == userId)
                .SingleOrDefaultAsync();
            if (auction == null)
            {
                return false;
            }
            _auctionContext.Auction.Remove(auction);
            await _auctionContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Auction>> GetWonAuctionsByUserIdAsync(string userId)
        {
            return await _auctionContext.Auction
                .Include(a => a.Bids)
                .Where(a => a.EndDate <= DateTime.Now &&
                            a.Bids.Any(b => b.UserId == userId) &&
                            a.Bids.OrderByDescending(b => b.Amount).FirstOrDefault().UserId == userId)
                .OrderByDescending(a => a.EndDate)
                .ToListAsync();
        }
    }
}
