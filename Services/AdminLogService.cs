using AuctionApp.Models;
using AuctionApp.Enums;
using System.Security.Claims;
using AuctionApp.Areas.Identity.Data;

public class AdminLogService
{
	private readonly ApplicationDbContext _context;
	private readonly IHttpContextAccessor _http;

	public AdminLogService(ApplicationDbContext context, IHttpContextAccessor http)
	{
		_context = context;
		_http = http;
	}

	private string GetAdminId() => _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

	public async Task LogAsync(string action, int actionId, string affectedUserId, string notes, LogStatus status = LogStatus.Success)
	{
		var adminId = _http.HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

		var log = new AdminLog
		{
			Action = action,
			ActionId = actionId,
			AffectedUserId = affectedUserId,
			AdminId = adminId,
			TimeStamp = DateTime.Now,
			Status = status,
			Notes = notes
		};

		_context.AdminLogs.Add(log);
		await _context.SaveChangesAsync();
	}
}
