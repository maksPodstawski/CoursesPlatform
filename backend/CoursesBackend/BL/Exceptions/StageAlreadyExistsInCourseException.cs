using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Exceptions
{
    public class StageAlreadyExistsInCourseException(string title)
        : Exception($"A stage titled \"{title}\" already exists in this course.")
    {
    }
}
