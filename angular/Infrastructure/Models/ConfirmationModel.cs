using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace security_demo_angular.Infrastructure.Models
{
    public class ConfirmationModel
    {
        public string Code { get; set; }
        public string UserId { get; set; }
        public bool Error { get; set; }
    }
}
