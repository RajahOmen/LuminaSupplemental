using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using Lumina.Data;
using Lumina.Excel.Sheets;

using LuminaSupplemental.Excel.Model;

namespace LuminaSupplemental.SpaghettiGenerator.Steps;

public partial class ItemSupplementStep
{
    public List<ItemSupplement> ProcessLodestoneItems()
    {
        var itemIdMapFile = "./lodestone-item-id.txt";
        var itemIdData = File.ReadLines(itemIdMapFile).ToList();
        var itemIdMap = new Dictionary<int, string>();
        for (var index = 0; index < itemIdData.Count; index++)
        {
            var line = itemIdData[index];
            if (line != "")
            {
                itemIdMap.Add(index, line);
            }
        }


        var itemSupplements = new List<ItemSupplement>();
        var storeProductCacheDirectory = Path.Combine(this.appConfig.Parsing.OnlineCacheDirectory,"FFXIV Lodestone Cache");
        Directory.CreateDirectory(storeProductCacheDirectory);
        //This is very shit code but parsing 30k HTML files is slow and I'll do everything I can to speed it up
        foreach (var itemId in itemIdMap.AsParallel())
        {
            var actualItemId = itemId.Key;
            var cacheFile = Path.Combine(storeProductCacheDirectory, $"{actualItemId - 1}.html");
            if (File.Exists(cacheFile))
            {
                string html = File.ReadAllText(cacheFile);

                if (!html.Contains("Yielded Item"))
                {
                    //Console.WriteLine("Skipped " + cacheFile);
                    continue;
                }

                string divPattern = @"<div class=""db-view__data__reward__item__name__wrapper"">.*?<strong>(.*?)<\/strong>";
                var matches = Regex.Matches(html, divPattern, RegexOptions.Singleline);
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        var name = match.Groups[1].Value.ToParseable();
                        if (itemsByName.ContainsKey(name) && itemsByName.ContainsKey(name))
                        {
                            var yieldedItem = this.itemSheet.GetRow(itemsByName[name]);
                            itemSupplements.Add(new ItemSupplement(yieldedItem.RowId, (uint)actualItemId , ItemSupplementSource.Loot));
                        }
                    }
                }
            }
        }

        return itemSupplements;
    }
}
