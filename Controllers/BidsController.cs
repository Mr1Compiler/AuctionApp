using Microsoft.AspNetCore.Mvc;
using Lab2Auction.Data;
using Microsoft.AspNetCore.Authorization;
using Lab2Auction.Services;
using Microsoft.AspNetCore.Identity;
using Lab2Auction.Models;
using AuctionApp.Areas.Identity.Data;
using AuctionApp.Enums;
using Microsoft.EntityFrameworkCore;
namespace Lab2Auction.Controllers
{
	[Authorize]
	public class BidsController : Controller
	{
		private readonly IAuctionService _auctionService;
		private readonly IBidService _bidService;
		private readonly AuctionDbContext _auctionContext;
		private readonly UserManager<AccountUser> _userManager;
		public BidsController(AuctionDbContext auctionContext, IBidService bidService, UserManager<AccountUser> userManager, IAuctionService auctionService)
		{
			_bidService = bidService;
			_auctionService = auctionService;
			_auctionContext = auctionContext;
			_userManager = userManager;
		}
		// GET: Bids
		public async Task<IActionResult> Index()
		{
			var bids = await _bidService.GetAllBidsAsync();
			return View(bids);
		}
		// GET: Bids/MyBids
		public async Task<IActionResult> MyBids()
		{
			ViewData["Title"] = "My Bids";
			var myBids = await _bidService.GetBidsByUserAsync(User);
			return View("Index", myBids);
		}
		// Get: Bids/ListBids
		public async Task<IActionResult> ListBids(int auctionId)
		{
			var bids = await _bidService.GetBidsForAuctionAsync(auctionId);
			var auction = await _auctionService.GetAuctionDetailsAsync(auctionId);

			if (auction == null)
			{
				return NotFound();
			}

			ViewBag.Auction = auction;
			return View(bids); // Model is a list of bids
		}


		// GET: Bids/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var bid = await _bidService.GetBidDetailsAsync(id.Value);
			if (bid == null)
			{
				return NotFound();
			}
			return View(bid);
		}
		// GET: Bids/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var bid = await _bidService.GetBidDetailsAsync(id.Value);
			if (bid == null || bid.UserId != _userManager.GetUserId(User))
			{
				return NotFound();
			}
			return View(bid);
		}
		// POST: Bids/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var success = await _bidService.DeleteBidAsync(id, User);
			if (!success)
			{
				return NotFound();
			}
			return RedirectToAction(nameof(MyBids));
		}


		// GET: Bids/Create
		[HttpGet]
		public async Task<IActionResult> Create(int auctionId)
		{
			var userId = _userManager.GetUserId(User);

			var existingBid = await _auctionContext.Bid
				.FirstOrDefaultAsync(b => b.AuctionId == auctionId && b.UserId == userId);

			if (existingBid != null)
			{
				ViewBag.AlreadyBid = true;
				ViewBag.BidId = existingBid.Id;
				return View(new Bid { AuctionId = auctionId }); // still pass the model for consistency
			}

			ViewBag.AlreadyBid = false;
			return View(new Bid { AuctionId = auctionId });
		}


		// POST: Bids/Create
		// POST: Bids/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Amount, AuctionId")] Bid bid)
		{
			var userId = _userManager.GetUserId(User);

			bool alreadyBid = await _auctionContext.Bid
				.AnyAsync(b => b.AuctionId == bid.AuctionId && b.UserId == userId);

			if (alreadyBid)
			{
				ModelState.AddModelError("", "You have already placed a bid on this auction.");
				return View(bid);
			}

			var canPlaceBid = await _bidService.CanUserPlaceBidAsync(bid.AuctionId, User);
			if (!canPlaceBid)
			{
				return Forbid();
			}

			bid.UserEmail = _userManager.GetUserName(User);
			bid.UserId = userId;

			var placedBid = await _bidService.PlaceBidAsync(bid, User);

			if (placedBid == null)
			{
				ModelState.AddModelError("Amount", "Your bid must be higher than the current highest bid.");
				return View(bid);
			}

			// ✅ Redirect to ListBids
			return RedirectToAction("ListBids", "Bids", new { auctionId = bid.AuctionId });
		}


		private bool BidExists(int id)
		{
			return (_auctionContext.Bid?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SellToBidder(int auctionId, int bidId)
		{
			var auction = await _auctionContext.Auction
				.Include(a => a.Bids)
				.FirstOrDefaultAsync(a => a.Id == auctionId);

			if (auction == null || auction.Status == AuctionStatus.Sold)
				return NotFound();

			if (auction.Status == AuctionStatus.ApprovalPending)
			{
				TempData["Message"] = "Already waiting for admin approval.";
				return RedirectToAction("ListBids", new { auctionId });
			}

			var bid = auction.Bids.FirstOrDefault(b => b.Id == bidId);
			if (bid == null)
				return NotFound();

			auction.Status = AuctionStatus.ApprovalPending;
			auction.WinningBidId = bid.Id;

			await _auctionContext.SaveChangesAsync();

			TempData["Message"] = "Auction sent for admin approval.";
			return RedirectToAction("ListBids", new { auctionId });
		}


		public async Task<IActionResult> Edit(int id)
		{
			var bid = await _auctionContext.Bid.FindAsync(id);
			var userId = _userManager.GetUserId(User);

			if (bid == null || bid.UserId != userId)
			{
				return Forbid();
			}

			return View(bid);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Amount")] Bid updatedBid)
		{
			var bid = await _auctionContext.Bid.FindAsync(id);
			var userId = _userManager.GetUserId(User);

			if (bid == null || bid.UserId != userId)
			{
				return Forbid();
			}

			// Optional: Prevent lowering the bid
			if (updatedBid.Amount <= bid.Amount)
			{
				ModelState.AddModelError("Amount", "New bid must be higher than the current bid.");
				return View(bid);
			}

			bid.Amount = updatedBid.Amount;
			bid.BidDate = DateTime.Now;
			await _auctionContext.SaveChangesAsync();

			return RedirectToAction("ListBids", new { auctionId = bid.AuctionId });
		}


	}
}
