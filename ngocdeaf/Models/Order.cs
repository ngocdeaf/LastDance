﻿using Microsoft.AspNetCore.Identity;
namespace Ngocdeaf.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public double Total { get; set; }
    }
}
