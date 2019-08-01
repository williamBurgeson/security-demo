using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace security_demo_angular.Infrastructure.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastLogin { get; set; }
        public int LoginsCount { get; set; }
    }
}
