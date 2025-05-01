using AuctionApp.Areas.Identity.Data;
using AuctionApp.Models;
using Lab2Auction.Data;
using Lab2Auction.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing.Printing;

namespace AuctionApp.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly UserManager<AccountUser> _userManager;
		private readonly AuctionDbContext _context;
		private readonly IAuctionService _auctionService;

		public HomeController(ILogger<HomeController> logger, UserManager<AccountUser> userManager, AuctionDbContext context, IAuctionService auctionService)
		{
			_logger = logger;
			_userManager = userManager;
			_context = context;
			_auctionService = auctionService;
		}

		public async Task<IActionResult> Index()
		{
			if (User.Identity.IsAuthenticated)
			{
				var user = await _userManager.GetUserAsync(User);
				var roles = await _userManager.GetRolesAsync(user);

				if (roles.Contains("Admin"))
				{
					return RedirectToAction("Dashboard", "Admin");
				}
				else
				{
					return View();
				}
			}

			return View();
		}


		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public async Task<IActionResult> RunAuctionExpiration()
		{
			await _auctionService.ProcessEndedAuctionsAsync();
			return Ok("Auctions processed.");
		}


	}
}
