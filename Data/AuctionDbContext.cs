using Microsoft.EntityFrameworkCore;
using Lab2Auction.Models;
namespace Lab2Auction.Data
{
	public class AuctionDbContext : DbContext
	{
		public AuctionDbContext(DbContextOptions<AuctionDbContext> options)
			: base(options)
		{
		}
		public DbSet<Lab2Auction.Models.Auction> Auction { get; set; } = default!;
		public DbSet<Lab2Auction.Models.Bid> Bid { get; set; } = default!;

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			// Configure the Auction-Bid relationship
			builder.Entity<Bid>()
				.HasOne(b => b.Auction)    // Each Bid is associated with a single Auction
				.WithMany(a => a.Bids)     // An Auction can have many Bids
				.HasForeignKey(b => b.AuctionId)
				.OnDelete(DeleteBehavior.Cascade); // to cascade delete Bids when an Auction is deleted
		}
	}
}
