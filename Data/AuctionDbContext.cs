using Microsoft.EntityFrameworkCore;
using Lab2Auction.Models;
using AuctionApp.Models;
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
		public DbSet<AuctionImage> AuctionImages { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Auction - Bid relationship
			builder.Entity<Bid>()
				.HasOne(b => b.Auction)
				.WithMany(a => a.Bids)
				.HasForeignKey(b => b.AuctionId)
				.OnDelete(DeleteBehavior.Cascade);

			// ✅ Auction - AuctionImage relationship
			builder.Entity<AuctionImage>()
				.HasOne(ai => ai.Auction)
				.WithMany(a => a.Images)
				.HasForeignKey(ai => ai.AuctionId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
