using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Options
{
    public class JwtOptions
    {
        public const string JwtOptionsKey = "JwtOptions";

        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpirationTimeInMinutes { get; set; }
    }
}
