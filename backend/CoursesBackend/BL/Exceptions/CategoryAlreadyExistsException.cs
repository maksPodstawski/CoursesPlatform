using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Exceptions
{
    public class CategoryAlreadyExistsException(string name)
        : Exception($"A category named \"{name}\" already exists.")
    {
    }
}
