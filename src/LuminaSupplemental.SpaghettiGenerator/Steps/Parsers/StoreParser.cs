using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

using Lumina.Data;
using Lumina.Excel.Sheets;

using Newtonsoft.Json;

namespace LuminaSupplemental.SpaghettiGenerator.Steps.Parsers;

public class StoreParser {
    private readonly AppConfig appConfig;

    public static Dictionary<uint, Dictionary<uint, Product>> StoreItems = new();
    public static Dictionary<uint, Product> StoreProducts = new();

    public static string UpdateStatus = string.Empty;

    public class ProductItem {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("number")]
        public int Number;
    }

    public class Product {
        [JsonProperty("id")]
        public uint ID;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("priceText")]
        public string PriceText;

        [JsonProperty("items")]
        public List<ProductItem> Items;
    }

    public class ProductListing {
        [JsonProperty("status")]
        public int Status;

        [JsonProperty("product")]
        public Product Product;
    }

    public class ProductList {
        [JsonProperty("status")]
        public int Status;

        [JsonProperty("products")]
        public List<Product> Products;
    }

    public StoreParser(AppConfig appConfig)
    {
        this.appConfig = appConfig;
    }


    public void UpdateItems() {
        UpdateStatus = "Fetching Product List";
        using var wc = new WebClient();
        var json = wc.DownloadString("https://api.store.finalfantasyxiv.com/ffxivcatalog/api/products/?lang=en-us&currency=USD&limit=10000");
        var productList = JsonConvert.DeserializeObject<ProductList>(json);
        if (productList == null) {
            UpdateStatus = "[Error] " + UpdateStatus;;
            return;
        }

        StoreItems.Clear();
        var storeProductCacheDirectory = Path.Combine(this.appConfig.Parsing.OnlineCacheDirectory,"FFXIV Store Cache");;
        Directory.CreateDirectory(storeProductCacheDirectory);

        var allItems = Service.GameData.Excel.GetSheet<Item>(Language.English);
        if (allItems == null) {
            UpdateStatus = "[Error] " + UpdateStatus;;
            return;
        }
        StoreProducts.Clear();
        for (var i = 0; i < productList.Products.Count; i++) {
            var p = productList.Products[i];
            try {
                string fullProductJson = null;

                var cacheFile = Path.Combine(storeProductCacheDirectory, $"{p.ID}.json");
                var usingCache = false;
                if (File.Exists(cacheFile)) {
                    UpdateStatus = $"Fetching Store Items: {i}/{productList.Products.Count} [{p.ID}, Cached]";

                    usingCache = true;
                    fullProductJson = File.ReadAllText(cacheFile);
                } else {
                    UpdateStatus = $"Fetching Store Items: {i}/{productList.Products.Count} [{p.ID}, from Store]";
                    fullProductJson = wc.DownloadString($"https://api.store.finalfantasyxiv.com/ffxivcatalog/api/products/{p.ID}?lang=en-us&currency=USD");
                }

                var productListing = JsonConvert.DeserializeObject<ProductListing>(fullProductJson);
                if (productListing?.Product == null) continue;
                if (productListing.Product.Items == null) {
                    Console.WriteLine($"{p.Name} has no Items?");
                } else {
                    StoreProducts.Add(p.ID, productListing.Product);

                    foreach (var item in productListing.Product.Items) {
                        var matchingItems = allItems.Where(i => i.Name.ExtractText() == item.Name).ToList();
                        if (matchingItems.Count == 0) {
                            Console.WriteLine($"Failed to find matching item for {item.Name}.");
                            continue;
                        }

                        if (matchingItems.Count > 1) {
                            Console.WriteLine($"Found multiple matching items for {item.Name}.");
                        }

                        foreach (var matchedItem in matchingItems) {
                            if (!StoreItems.ContainsKey(matchedItem.RowId)) {
                                StoreItems.Add(matchedItem.RowId, new Dictionary<uint, Product>());
                            }

                            if (!StoreItems[matchedItem.RowId].ContainsKey(p.ID)) {
                                StoreItems[matchedItem.RowId][p.ID] = p;
                            }
                        }
                    }

                    if (!usingCache) {
                        Console.WriteLine($"Cached Product Info: {p.ID}");
                        File.WriteAllText(cacheFile, fullProductJson);
                        Thread.Sleep( 500 );
                    }
                }
            } catch (Exception ex) {
                UpdateStatus = "[Error] " + UpdateStatus;
                Console.WriteLine(ex.Message + "Error in Update Task");
                return;
            }

            UpdateStatus = string.Empty;
        }
    }
}
