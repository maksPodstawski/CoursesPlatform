using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Exceptions
{
    public class RegistrationFailedException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public RegistrationFailedException(IEnumerable<string> errorDescriptions)
            : base(string.Join(Environment.NewLine, errorDescriptions))
        {
            Errors = errorDescriptions;
        }
    }
}
