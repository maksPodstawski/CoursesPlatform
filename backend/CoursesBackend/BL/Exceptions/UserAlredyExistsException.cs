using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Exceptions
{
    public class UserAlredyExistsException(string email) : Exception($"User with email: {email} alredy exists")
    {
    }
}
