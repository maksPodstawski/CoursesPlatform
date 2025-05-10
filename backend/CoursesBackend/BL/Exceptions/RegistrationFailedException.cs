using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Exceptions
{
    public class RegistrationFailedException(IEnumerable<string> errorDescriptions)
        : Exception($"Registration failed with errors: {string.Join(Environment.NewLine, errorDescriptions)}")
    {
    }
}
