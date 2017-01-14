using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftOS.Objects
{
    public class Shop
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ShopItem> Items { get; set; }
    }

    public abstract class ShopItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public string ShopOwner { get; set; }


        protected abstract void OnBuy();

        protected abstract void GiveCPToShopOwner(int cp);

        public bool Buy(ref Save buyer)
        {
            if(buyer.Codepoints >= Cost)
            {
                buyer.Codepoints -= Cost;
                OnBuy();
                GiveCPToShopOwner(Cost);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
