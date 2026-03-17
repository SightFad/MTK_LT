using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuflitShop.Models;

namespace HuflitShop.ViewModels
{
    public class ProductViewModel
    {
        public List<Category> Categories { set; get; }
        public List<Product> Products { set; get; }
    }
}
