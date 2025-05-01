using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Lab2Auction.Services;

public class AuctionExpirationService : BackgroundService
{
	private readonly IServiceScopeFactory _scopeFactory;

	public AuctionExpirationService(IServiceScopeFactory scopeFactory)
	{
		_scopeFactory = scopeFactory;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			using var scope = _scopeFactory.CreateScope();
			var auctionService = scope.ServiceProvider.GetRequiredService<IAuctionService>();

			await auctionService.ProcessEndedAuctionsAsync();

			await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // check every minute
		}
	}
}

