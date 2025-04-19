using Microsoft.AspNetCore.Mvc;
using Lab2Auction.Data;
using Lab2Auction.Models;
using Microsoft.AspNetCore.Authorization;
using Lab2Auction.Services;
using Microsoft.AspNetCore.Identity;
using AuctionApp.Areas.Identity.Data;
using AuctionApp.Models;
using AuctionApp.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace Lab2Auction.Controllers
{
	[Authorize]
	public class AuctionsController : Controller
	{
		private readonly IAuctionService _auctionService;
		private readonly AuctionDbContext _auctionContext;
		private readonly UserManager<AccountUser> _userManager;
		public AuctionsController(AuctionDbContext auctionContext, IAuctionService auctionService, UserManager<AccountUser> userManager)
		{
			_auctionService = auctionService;
			_auctionContext = auctionContext;
			_userManager = userManager;
		}
		// GET: Auctions / list ongoing auctions        
		public async Task<IActionResult> Index()
		{
			var ongoingAuctions = await _auctionService.GetOngoingAuctionsAsync();
			ViewData["Title"] = "Browse Auctions";
			return View(ongoingAuctions);
		}

		public async Task<Auction?> GetAuctionDetailsAsync(int auctionId)
		{
			return await _auctionContext.Auction
				.Include(a => a.Images) // ✅ This includes the images
				.FirstOrDefaultAsync(a => a.Id == auctionId);
		}



		// GET: Auctions / details / 5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var auction = await _auctionService.GetAuctionDetailsAsync(id.Value);
			if (auction == null)
			{
				return NotFound();
			}
			return View(auction);
		}
		// GET: Auctions/Create        
		public IActionResult Create()
		{
			return View();
		}
		// POST: Auctions/Create        
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(AuctionCreateViewModel model)
		{
			if (model.Images != null && model.Images.Count > 3)
			{
				ModelState.AddModelError("Images", "You can upload up to 3 images only.");
				return View(model);
			}

			const long maxSize = 3 * 1024 * 1024;

			if (model.Images != null)
			{
				foreach (var file in model.Images)
				{
					if (file.Length > maxSize)
					{
						ModelState.AddModelError("Images", "Each image must be 3 MB or less.");
						return View(model);
					}
				}
			}

			if (ModelState.IsValid)
			{
				var auction = new Auction
				{
					Name = model.Name,
					Description = model.Description,
					StartingPrice = model.StartingPrice,
					EndDate = model.EndDate,
					UserId = _userManager.GetUserId(User),
					UserEmail = _userManager.GetUserName(User),
					Images = new List<AuctionImage>()
				};

				if (model.Images != null)
				{
					foreach (var file in model.Images)
					{
						var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
						var savePath = Path.Combine("wwwroot/images/auctions", uniqueName);

						using var stream = new FileStream(savePath, FileMode.Create);
						await file.CopyToAsync(stream);

						auction.Images.Add(new AuctionImage
						{
							ImagePath = $"/images/auctions/{uniqueName}"
						});
					}
				}

				await _auctionService.CreateAuctionAsync(auction);
				return RedirectToAction(nameof(Index));
			}

			return View(model);
		}

		// GET: Auctions/MyAuctions
		public async Task<IActionResult> MyAuctions()
		{
			var userId = _userManager.GetUserId(User);
			ViewData["Title"] = "My Auctions";
			var myAuctions = await _auctionService.GetAuctionsByUserIdAsync(userId);
			return View("Index", myAuctions);
		}
		// GET: Auctions/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var auction = await _auctionService.GetAuctionDetailsAsync(id.Value);
			if (auction == null || auction.UserId != _userManager.GetUserId(User))
			{
				return NotFound();
			}
			if (auction.EndDate <= DateTime.Now)
			{
				return Forbid();
			}
			return View(new AuctionEditModel { Id = auction.Id, Description = auction.Description });
		}
		// POST: Auctions/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Description")] AuctionEditModel model)
		{
			if (id != model.Id)
			{
				return NotFound();
			}
			if (ModelState.IsValid)
			{
				var userId = _userManager.GetUserId(User);
				var success = await _auctionService.UpdateAuctionAsync(model, userId);
				if (!success)
				{
					return NotFound();
				}
				return RedirectToAction(nameof(Index));
			}
			return View(model);
		}
		// GET: Auctions/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var auction = await _auctionService.GetAuctionDetailsAsync(id.Value);
			if (auction == null)
			{
				return NotFound();
			}
			// Only the owner of the auction should be able to delete it
			if (auction.UserId != _userManager.GetUserId(User))
			{
				return Forbid();
			}
			return View(auction);
		}
		// POST: Auctions/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var userId = _userManager.GetUserId(User);
			var success = await _auctionService.DeleteAuctionAsync(id, userId);
			if (!success)
			{
				return NotFound();
			}
			return RedirectToAction(nameof(Index));
		}
		// GET: Auctions/WonAuctions
		public async Task<IActionResult> WonAuctions()
		{
			var userId = _userManager.GetUserId(User);
			ViewData["Title"] = "Won Auctions";
			var wonAuctions = await _auctionService.GetWonAuctionsByUserIdAsync(userId);
			return View("Index", wonAuctions);
		}

		private bool AuctionExists(int id)
		{
			return (_auctionContext.Auction?.Any(e => e.Id == id)).GetValueOrDefault();
		}


		// In BidsController.cs
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SellToBidder(int auctionId, int bidId)
		{
			var userId = _userManager.GetUserId(User);
			var success = await _auctionService.SellAuctionToBidderAsync(auctionId, bidId, userId);

			if (!success)
			{
				return BadRequest(); // or show message
			}

			return RedirectToAction("ListBids", new { auctionId = auctionId });
		}

	}
}
