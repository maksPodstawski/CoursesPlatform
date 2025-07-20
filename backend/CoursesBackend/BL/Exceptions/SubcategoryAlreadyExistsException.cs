using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Exceptions
{
    public class SubcategoryAlreadyExistsException(string name)
        : Exception($"A subcategory named \"{name}\" already exists.")
    {
    }
}
