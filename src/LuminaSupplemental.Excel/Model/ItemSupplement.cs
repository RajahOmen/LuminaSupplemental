using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using Lumina;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace LuminaSupplemental.Excel.Model
{
    public class ItemSupplement : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("SourceItemId")] public uint SourceItemId { get; set; }
        [Name("ItemSupplementSource"), TypeConverter(typeof( EnumConverter ))] public ItemSupplementSource ItemSupplementSource { get; set; }
        
        public LazyRow< Item > Item;
        
        public LazyRow< Item > SourceItem;

        public ItemSupplement(uint rowId, uint itemId, uint sourceItemId, ItemSupplementSource itemSupplementSource )
        {
            RowId = rowId;
            ItemId = itemId;
            SourceItemId = sourceItemId;
            ItemSupplementSource = itemSupplementSource;
        }

        public ItemSupplement()
        {
            
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            SourceItemId = uint.Parse( lineData[ 2 ] );
            if( Enum.TryParse<ItemSupplementSource>( lineData[ 3 ], out var itemSupplementSource ) )
            {
                ItemSupplementSource = itemSupplementSource;
            }
        }

        public string[] ToCsv()
        {
            return Array.Empty<string>();
        }

        public bool IncludeInCsv()
        {
            return false;
        }

        public virtual void PopulateData( GameData gameData, Language language )
        {
            Item = new LazyRow< Item >( gameData, ItemId, language );
            SourceItem = new LazyRow< Item >( gameData, SourceItemId, language );
        }
    }
}