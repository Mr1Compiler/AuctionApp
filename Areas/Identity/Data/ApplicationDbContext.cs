using AuctionApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuctionApp.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<AccountUser>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	public DbSet<AdminLog> AdminLogs { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
	
		builder.Entity<AdminLog>()
			.HasOne(l => l.AffectedUser)
			.WithMany()
			.HasForeignKey(l => l.AffectedUserId)
			.OnDelete(DeleteBehavior.Restrict);


		builder.Entity<AccountUser>()
			.Property(u => u.Status)
			.HasConversion<string>();
	}
}
