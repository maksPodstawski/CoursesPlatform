using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Exceptions
{
    public class CourseAlreadyExistsException(string title)
        : Exception($"A course titled \"{title}\" already exists.")
    {
    }
}
