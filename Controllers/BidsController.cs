using Microsoft.AspNetCore.Mvc;
using Lab2Auction.Data;
using Microsoft.AspNetCore.Authorization;
using Lab2Auction.Services;
using Microsoft.AspNetCore.Identity;
using Lab2Auction.Models;
using AuctionApp.Areas.Identity.Data;
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
			return View(bids); // ✅ Model is a list of bids
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
        public IActionResult Create(int auctionId)
        {
            var canPlaceBid = _bidService.CanUserPlaceBidAsync(auctionId, User).Result;
            if (!canPlaceBid)
            {
                return Forbid();
            }
            var bidModel = new Bid { AuctionId = auctionId };
            return View(bidModel);
        }
        // POST: Bids/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Amount, AuctionId")] Bid bid)
        {
            var canPlaceBid = await _bidService.CanUserPlaceBidAsync(bid.AuctionId, User);
            if (!canPlaceBid)
            {
                return Forbid();
            }
            bid.UserEmail = _userManager.GetUserName(User);
            var placedBid = await _bidService.PlaceBidAsync(bid, User);
            if (placedBid == null)
            {
                ModelState.AddModelError("Amount", "Your bid must be higher than the current highest bid.");
                return View(bid);
            }
            return RedirectToAction("Details", "Auctions", new { id = bid.AuctionId });
        }
        private bool BidExists(int id)
        {
            return (_auctionContext.Bid?.Any(e => e.Id == id)).GetValueOrDefault();
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SellToBidder(int auctionId, int bidId)
		{
			var userId = _userManager.GetUserId(User); // ✅ get the user ID
			var success = await _auctionService.SellAuctionToBidderAsync(auctionId, bidId, userId); // ✅ pass it

			if (!success)
			{
				return NotFound();
			}
			return RedirectToAction("MyAuctions");
		}


	}
}
