/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShiftOS.Objects;
using System.IO;
using static ShiftOS.Server.Program;
using NetSockets;

namespace ShiftOS.Server
{
    public static class ShopBackend
    {
        [MudRequest("update_shop_by_user", typeof(Dictionary<string, object>))]
        public static void UpdateShopByUser(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            List<Shop> shopList = new List<Shop>();
            if (File.Exists("shops.json"))
                shopList = JsonConvert.DeserializeObject<List<Shop>>(File.ReadAllText("shops.json"));

            var username = args["username"] as string;
            var updateShop = args["shop"] as Shop;

            for (int i = 0; i < shopList.Count; i++)
            {
                if (shopList[i] == null)
                {
                    shopList.RemoveAt(i);
                    i--;
                }
                else
                {
                    if (shopList[i].Owner == username)
                    {
                        shopList[i] = updateShop;
                    }
                }
            }

            File.WriteAllText("shops.json", JsonConvert.SerializeObject(shopList, Formatting.Indented));

            Program.ClientDispatcher.DispatchTo("shop_added", guid, "");
        }

        [MudRequest("create_shop", typeof(Dictionary<string, object>))]
        public static void CreateShop(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            List<Shop> shopFile = new List<Shop>();
            if (File.Exists("shops.json"))
                shopFile = JsonConvert.DeserializeObject<List<Shop>>(File.ReadAllText("shops.json"));

            var newShop = JsonConvert.DeserializeObject<Shop>(JsonConvert.SerializeObject(contents));

            foreach (var shop in shopFile)
            {
                if (shop != null)
                {
                    if (shop.Name == newShop.Name)
                    {
                        Program.ClientDispatcher.DispatchTo("shop_taken", guid, "");
                        return;
                    }
                }
            }

            shopFile.Add(newShop);
            File.WriteAllText("shops.json", JsonConvert.SerializeObject(shopFile, Formatting.Indented));


            Program.ClientDispatcher.DispatchTo("shop_added", guid, "");

        }

        [MudRequest("user_shop_check", typeof(Dictionary<string, object>))]
        public static void UserShopCheck(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            List<Shop> allshops = new List<Shop>();
            if (File.Exists("shops.json"))
                allshops = JsonConvert.DeserializeObject<List<Shop>>(File.ReadAllText("shops.json"));

            int res = 0;

            foreach (var shop in allshops)
            {
                if (shop != null)
                {
                    if (shop.Owner == args["username"] as string)
                    {
                        res = 1;
                    }
                }
            }

            Program.ClientDispatcher.DispatchTo("user_shop_check_result", guid, res.ToString());
        }

        [MudRequest("shop_getitems", typeof(Dictionary<string, object>))]
        public static void GetShopItems(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            var shopName = args["shopname"] as string;
            Shop tempShop = null;
            foreach (var item in JsonConvert.DeserializeObject<List<Shop>>(File.ReadAllText("shops.json")))
            {
                if (item != null)
                {
                    if (item.Name == shopName)
                    {
                        tempShop = item;
                    }
                }
            }

            if (tempShop != null)
                foreach (var item in tempShop.Items)
                {
                    Program.ClientDispatcher.DispatchTo("shop_additem", guid, new
                    {
                        shop = shopName,
                        itemdata = Program.Compress(Program.Compress(JsonConvert.SerializeObject(item)))
                    });
                }

        }

        [MudRequest("shop_getall", null)]
        public static void GetAllShops(string guid, object contents)
        {
            var args = contents as Dictionary<string, object>;
            List<Shop> shops = new List<Shop>();
            if (File.Exists("shops.json"))
                shops = JsonConvert.DeserializeObject<List<Shop>>(File.ReadAllText("shops.json"));
            //Purge all items in all shops temporarily.
            //This is to save on network bandwidth as it will take a long time to send everyone's shops down if we don't purge the stock.
            //And with high bandwidth usage, we may end up DOSing our clients when too many people upload too many things.
            //Furthermore, this'll make the MUD Control Centre seem faster...
            for (int i = 0; i < shops.Count; i++)
            {
                if (shops[i] == null)
                {
                    shops.RemoveAt(i);
                    i--;
                }
                else
                {
                    shops[i].Items = new List<ShopItem>();
                }
            }
            Program.ClientDispatcher.DispatchTo("shop_all", guid, shops);
        }

        [MudRequest("user_get_shop", typeof(string))]
        public static void GetShop(string guid, object contents)
        {
            string shopOwner = contents as string;
            if (File.Exists("shops.json"))
                foreach (var shop in JsonConvert.DeserializeObject<List<Shop>>(File.ReadAllText("shops.json")))
                {
                    if (shop != null)
                    {
                        if (shop.Owner == shopOwner)
                        {
                            server.DispatchTo(new Guid(guid), new NetObject("ruecuodaL", new ServerMessage
                            {
                                Name = "user_shop",
                                GUID = "server",
                                Contents = JsonConvert.SerializeObject(shop)
                            }));
                            return;
                        }
                    }
                }

            server.DispatchTo(new Guid(guid), new NetObject("ruecuodaL", new ServerMessage
            {
                Name = "user_noshop",
                GUID = "server",
            }));

        }
    }
}
