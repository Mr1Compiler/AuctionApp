using AuctionApp.Areas.Identity.Data;
using AuctionApp.Enums;

namespace AuctionApp.Models;

public class AdminLog
{
	public int Id { get; set; }
	public string Action { get; set; }
	public int ActionId { get; set; }
	public string AffectedUserId { get; set; }   
	public AccountUser AffectedUser { get; set; } 
	public string AdminId { get; set; }  
	public AccountUser Admin { get; set; }
	public DateTime TimeStamp { get; set; }
	public LogStatus Status { get; set; } = LogStatus.Success;
	public string Notes { get; set; }
}
