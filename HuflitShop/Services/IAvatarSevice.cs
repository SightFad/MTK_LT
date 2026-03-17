using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HuflitShop.Services
{
    public interface IAvatarSevice
    {
        public void UploadAvatar(IFormFile file);
    }
}
