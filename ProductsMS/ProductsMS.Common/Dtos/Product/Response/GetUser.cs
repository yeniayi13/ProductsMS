﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsMS.Common.Dtos.Product.Response
{
    public class GetUser
    {
        public Guid UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPassword { get; set; }
        public string? AuctioneerDni { get; set; }
        public string? UserName { get; set; } = string.Empty;
        public string? UserPhone { get; set; } = string.Empty;
        public string? UserAddress { get; set; } = string.Empty;
        //public string? UserAvailable { get; init; }
        public string? UserLastName { get; set; } = string.Empty;
        public DateOnly AuctioneerBirthday { get; set; }
        public bool AuctioneerDelete { get; set; } = false;
    }
}
