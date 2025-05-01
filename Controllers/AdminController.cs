using Lab2Auction.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuctionApp.Enums;
using AuctionApp.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
	private readonly AuctionDbContext _context;
	private readonly ApplicationDbContext _appContext;
	private readonly UserManager<AccountUser> _userManager;
	private readonly AdminLogService _logService;

	public AdminController(AuctionDbContext context, ApplicationDbContext appContext, UserManager<AccountUser> userManager, AdminLogService logService)
	{
		_context = context;
		_appContext = appContext; 
		_userManager = userManager;
		_logService = logService;
	}
	public async Task<IActionResult> Dashboard()
	{
		return View();
	}

	public async Task<IActionResult> Auctions()
	{
		var allAuctions = await _context.Auction
				.Include(a => a.Images)
				.Include(a => a.Bids)
				.ToListAsync();

		return View(allAuctions);
	}

	[HttpGet]
	public async Task<IActionResult> Auctions(string? search)
	{
		var query = _context.Auction
			.Include(a => a.Images)
			.Include(a => a.Bids)
			.AsQueryable();

		if (!string.IsNullOrWhiteSpace(search))
		{
			query = query.Where(a => a.Name.Contains(search));
		}

		var allAuctions = await query.ToListAsync();
		ViewBag.Search = search;
		return View(allAuctions);
	}

	public async Task<IActionResult> PendingAuctions()
	{
		var pendingAuctions = await _context.Auction
			.Include(a => a.Images)
			.Where(a => a.Status == AuctionStatus.Pending)
			.ToListAsync();

		return View(pendingAuctions);
	}


	[HttpPost]
	public async Task<IActionResult> ApproveAuction(int id)
	{
		var auction = await _context.Auction.FindAsync(id);
		if (auction == null) return NotFound();

		auction.Status = AuctionStatus.Approved;
		await _context.SaveChangesAsync();

		await _logService.LogAsync(
		"Approved Auction",
		id,
		auction.UserId,
		$"Auction '{auction.Name}' approved by admin."
	);

		return RedirectToAction(nameof(PendingAuctions));
	}


	[HttpPost]
	public async Task<IActionResult> RejectAuction(int id)
	{
		var auction = await _context.Auction.FindAsync(id);
		if (auction == null) return NotFound();

		auction.Status = AuctionStatus.Rejected;
		await _context.SaveChangesAsync();

		await _logService.LogAsync(
		"Rejected Auction",
		id,
		auction.UserId,
		$"Auction '{auction.Name}' rejected by admin."
	);

		return RedirectToAction(nameof(PendingAuctions));
	}


	[HttpGet]
	public async Task<IActionResult> Details(int id)
	{
		var auction = await _context.Auction
			.Include(a => a.Images)
			.FirstOrDefaultAsync(a => a.Id == id);

		if (auction == null)
			return NotFound();

		return View(auction); // Looks for Views/Admin/Details.cshtml
	}

	public async Task<IActionResult> Users(string search, UserStatus? status)
	{
		var users = _userManager.Users.AsQueryable();

		if (!string.IsNullOrEmpty(search))
			users = users.Where(u => u.UserName.Contains(search) || u.Email.Contains(search));

		if (status.HasValue)
			users = users.Where(u => u.Status == status);

		var list = await users.ToListAsync();
		ViewBag.StatusFilter = status;
		ViewBag.SearchTerm = search;
		return View(list);
	}


	[HttpPost]
	public async Task<IActionResult> ChangeStatus(string userId, UserStatus newStatus)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null) return NotFound();

		user.Status = newStatus;
		await _userManager.UpdateAsync(user);
		return RedirectToAction(nameof(Users));
	}


	[HttpPost]
	public async Task<IActionResult> ApproveSellWithBid(int id, int bidId)
	{
		var auction = await _context.Auction
			.Include(a => a.Bids)
			.FirstOrDefaultAsync(a => a.Id == id);

		if (auction == null)
			return NotFound();

		var bid = auction.Bids.FirstOrDefault(b => b.Id == bidId);
		if (bid == null)
			return NotFound();

		auction.Status = AuctionStatus.Sold;
		auction.WinningBidId = bid.Id;
		await _context.SaveChangesAsync();

		await _logService.LogAsync(
		"Approved Sell Request",
		id,
		auction.UserId,
		$"Sell to bidder '{bid.UserEmail}' approved."
		);

		return RedirectToAction("SellApprovals");
	}


	[HttpPost]
	public async Task<IActionResult> ApproveSell(int id)
	{
		var auction = await _context.Auction.FindAsync(id);
		if (auction == null) return NotFound();

		auction.Status = AuctionStatus.Sold;
		await _context.SaveChangesAsync();
		return RedirectToAction("SellApprovals");
	}


	[HttpPost]
	public async Task<IActionResult> RejectSell(int id)
	{
		var auction = await _context.Auction.FindAsync(id);
		if (auction == null) return NotFound();

		auction.Status = AuctionStatus.Approved; // revert back to approved
		await _context.SaveChangesAsync();
		return RedirectToAction("SellApprovals");
	}


	[HttpGet]
	public async Task<IActionResult> SellApprovals()
	{
		var auctions = await _context.Auction
			.Include(a => a.Bids)
			.Where(a => a.Status == AuctionStatus.ApprovalPending)
			.ToListAsync();

		return View(auctions);
	}

	[HttpPost]
	public async Task<IActionResult> CancelAuction(int id)
	{
		var auction = await _context.Auction.FindAsync(id);
		if (auction == null) return NotFound();

		auction.Status = AuctionStatus.Cancelled;
		await _context.SaveChangesAsync();

		return RedirectToAction(nameof(Auctions));
	}

	public async Task<IActionResult> Logs()
	{
		var logs = await _appContext.AdminLogs
			.Include(l => l.Admin)
			.Include(l => l.AffectedUser)
			.OrderByDescending(l => l.TimeStamp)
			.ToListAsync();

		return View(logs);
	}

}
