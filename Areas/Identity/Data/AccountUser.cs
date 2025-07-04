﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionApp.Enums;
using Microsoft.AspNetCore.Identity;

namespace AuctionApp.Areas.Identity.Data;

// Add profile data for application users by adding properties to the AccountUser class
public class AccountUser : IdentityUser
{
	public UserStatus Status { get; set; } = UserStatus.Active;
}

