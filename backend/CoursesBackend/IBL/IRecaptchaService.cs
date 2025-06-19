using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    public interface IRecaptchaService
    {
        Task<bool> VerifyAsync(string token);
    }
}
