using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuflitShop.Models;

namespace HuflitShop.ViewModels
{
    public class ProductSizeModel
    {
        public List<Product> Products { set; get; }
        public List<Size> Sizes { set; get; }
        public int SelectedPro { set; get; }
        public int SelectedSize { set; get; }
        public int Quantity { set; get; }
    }
}
