/*
This controller should include:

dshboard() → shows admin overview.
PendingAuctions() → list of unapproved auctions.
Auctions() → all auctions (with filtering).
Users() → manage users.
Logs()
*/
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    public IActionResult Dashboard()
    {
        return View();
    }
}
