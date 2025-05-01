using Microsoft.AspNetCore.Mvc;
using Lab2Auction.Data;
using Lab2Auction.Models;
using Microsoft.AspNetCore.Authorization;
using Lab2Auction.Services;
using Microsoft.AspNetCore.Identity;
using AuctionApp.Areas.Identity.Data;
using AuctionApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using AuctionApp.Enums;

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
		[HttpGet]
		[HttpGet]
		public async Task<IActionResult> Index(string? search, int page = 1, string type = "browse", string sort = "end")
		{
			ViewData["Title"] = type == "my" ? "My Auctions" : "Browse Auctions";

			var query = _auctionContext.Auction
				.Include(a => a.Images)
				.Include(a => a.Bids)
				.AsQueryable();

			var userId = _userManager.GetUserId(User);

			// Filter by type
			if (type == "my")
			{
				query = query.Where(a => a.UserId == userId);
			}
			else
			{
				query = query.Where(a => a.Status == AuctionStatus.Approved);
			}

			// Apply search if provided
			if (!string.IsNullOrWhiteSpace(search))
			{
				query = query.Where(a => a.Name.Contains(search));
			}

			// Apply sorting
			query = sort switch
			{
				"publish" => query.OrderByDescending(a => a.CreatedAt),
				_ => query.OrderBy(a => a.EndDate)
			};

			// Apply pagination AFTER filtering + sorting
			int pageSize = 12;
			int totalCount = await query.CountAsync();

			var auctions = await query
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var vm = new AuctionListViewModel
			{
				Auctions = auctions,
				SearchQuery = search,
				CurrentPage = page,
				TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
			};

			return View("Index", vm);
		}



		public async Task<Auction?> GetAuctionDetailsAsync(int auctionId)
		{
			return await _auctionContext.Auction
				.Include(a => a.Images) // This includes the images
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

		//// GET: Auctions/MyAuctions
		//public async Task<IActionResult> MyAuctions()
		//{
		//	var userId = _userManager.GetUserId(User);
		//	ViewData["Title"] = "My Auctions";
		//	var myAuctions = await _auctionService.GetAuctionsByUserIdAsync(userId);
		//	return View("Index", myAuctions);
		//}


		// GET: Auctions/Edit/5
		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var auction = await _auctionContext.Auction
				.Include(a => a.Images)
				.Include(a => a.Bids)
				.FirstOrDefaultAsync(a => a.Id == id);

			if (auction == null) return NotFound();

			var vm = new AuctionApp.ViewModels.AuctionEditModel
			{
				Id = auction.Id,
				Name = auction.Name,
				Description = auction.Description,
				EndDate = auction.EndDate,
				Images = auction.Images.ToList(),
				Bids = auction.Bids.ToList()
			};

			return View(vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(AuctionApp.ViewModels.AuctionEditModel model)
		{
			var auction = await _auctionContext.Auction
				.Include(a => a.Images)
				.Include(a => a.Bids)
				.FirstOrDefaultAsync(a => a.Id == model.Id);

			if (auction == null) return NotFound();

			if (!ModelState.IsValid) return View(model);

			auction.Name = model.Name;
			auction.Description = model.Description;

			if (!auction.Bids.Any())
			{
				auction.EndDate = model.EndDate;
			}

			// Handle image uploads
			if (model.NewImages != null && model.NewImages.Count > 0)
			{
				foreach (var file in model.NewImages)
				{
					if (file.Length > 0)
					{
						var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(file.FileName);
						var path = Path.Combine("wwwroot/images", fileName);

						using (var stream = new FileStream(path, FileMode.Create))
						{
							await file.CopyToAsync(stream);
						}

						auction.Images.Add(new AuctionImage { AuctionId = auction.Id, ImagePath = "/images/" + fileName });
					}
				}
			}

			await _auctionContext.SaveChangesAsync();
			return RedirectToAction("MyAuctions");
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
		public async Task<IActionResult> WonAuctions(string? search, int page = 1)
		{
			var userId = _userManager.GetUserId(User);

			var query = _auctionContext.Auction
				.Include(a => a.Bids)
				.Include(a => a.Images)
				.Where(a =>
					a.Status == AuctionStatus.Sold &&
					a.Bids.Any(b => b.UserId == userId &&
						b.Id == a.WinningBidId))
				.AsQueryable();

			if (!string.IsNullOrWhiteSpace(search))
			{
				query = query.Where(a => a.Name.Contains(search));
			}

			int totalCount = await query.CountAsync();
			int pageSize = 10;

			var auctions = await query
				.OrderByDescending(a => a.EndDate)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			ViewData["Title"] = "Won Auctions";

			return View("Index", new AuctionListViewModel
			{
				Auctions = auctions,
				SearchQuery = search,
				CurrentPage = page,
				TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
			});
		}

		private bool AuctionExists(int id)
		{
			return (_auctionContext.Auction?.Any(e => e.Id == id)).GetValueOrDefault();
		}

		public async Task<IActionResult> MyAuctions(string? search, int page = 1)
		{
			int pageSize = 10;
			var userId = _userManager.GetUserId(User);
			var query = _auctionService.GetUserAuctionsQuery(userId);

			if (!string.IsNullOrEmpty(search))
				query = query.Where(a => a.Name.Contains(search));

			var totalCount = await query.CountAsync();
			var auctions = await query
				.OrderByDescending(a => a.EndDate)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			ViewData["Title"] = "My Auctions";

			return View("Index", new AuctionListViewModel
			{
				Auctions = auctions,
				CurrentPage = page,
				TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
				SearchQuery = search
			});
		}
	}
}
