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
        public string Owner { get; set; }
    }

    public abstract class ShopItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public int FileType { get; set; }
        public byte[] MUDFile { get; set; }
    }
}
