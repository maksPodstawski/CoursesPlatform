using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Exceptions
{
    public class LoginFailedException(string email): Exception($"Invalid email: {email} or password")
    {
    }
}
