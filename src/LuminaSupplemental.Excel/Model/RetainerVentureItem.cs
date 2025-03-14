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

using Lumina.Excel.Sheets;

namespace LuminaSupplemental.Excel.Model
{
    public class RetainerVentureItem : ICsv
    {
        [Name("RowId")] public uint RowId { get; set; }
        [Name("ItemId")] public uint ItemId { get; set; }
        [Name("RetainerTaskRandomId")] public uint RetainerTaskRandomId { get; set; }
        
        public RowRef< Item > Item;
        
        public RowRef< RetainerTaskRandom > RetainerTaskRandom;

        public RetainerVentureItem(uint rowId, uint itemId, uint retainerTaskRandomId )
        {
            RowId = rowId;
            ItemId = itemId;
            RetainerTaskRandomId = retainerTaskRandomId;
        }

        public RetainerVentureItem()
        {
            
        }

        public void FromCsv(string[] lineData)
        {
            RowId = uint.Parse( lineData[ 0 ] );
            ItemId = uint.Parse( lineData[ 1 ] );
            RetainerTaskRandomId = uint.Parse( lineData[ 2 ] );
        }

        public string[] ToCsv()
        {
            return Array.Empty<string>();
        }

        public bool IncludeInCsv()
        {
            return false;
        }

        public virtual void PopulateData( ExcelModule module, Language language )
        {
            Item = new RowRef< Item >( module, ItemId);
            RetainerTaskRandom = new RowRef< RetainerTaskRandom >( module, RetainerTaskRandomId);
        }
    }
}
