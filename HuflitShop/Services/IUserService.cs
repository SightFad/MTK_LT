using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HuflitShop.Services
{
    public interface IUserService
    {
        string GetUserId();
        bool IsAuthenticated();
    }
}
