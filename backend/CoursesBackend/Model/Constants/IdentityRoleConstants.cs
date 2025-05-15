using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Constans
{
    public static class IdentityRoleConstants
    {
        public static readonly Guid AdminRoleGuid = new("3b68d10b-cca1-416c-b02a-af424a16498c");
        public static readonly Guid UserRoleGuid = new("447cfac1-99b2-4821-816f-236b26e65fbf");


        public const string Admin = "Admin";
        public const string User = "User";
    }
}