using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Lumina;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace LuminaSupplemental.Excel.Model;

public class MobSpawnPosition : ICsv
{
    public uint BNpcBaseId;
    public uint BNpcNameId;
    public uint TerritoryTypeId;
    public Vector3 Position;
    public byte Subtype;

    public LazyRow< BNpcBase > BNpcBase;
    public LazyRow< BNpcName > BNpcName;
    public LazyRow< TerritoryType > TerritoryType;

    public MobSpawnPosition(uint bNpcBaseId, uint bNpcNameId, uint territoryTypeId, Vector3 position, byte subtype)
    {
        BNpcBaseId = bNpcBaseId;
        BNpcNameId = bNpcNameId;
        TerritoryTypeId = territoryTypeId;
        Position = position;
        Subtype = subtype;
    }

    public static List<string> GetHeaders()
    {
        return new List<string>()
        {
            "BNpcBaseId",
            "BNpcNameId",
            "TerritoryTypeId",
            "Position",
            "Subtype"
        };
    }

    public MobSpawnPosition()
    {
        
    }
    
    public void FromCsv(string[] lineData)
    {
        BNpcBaseId = uint.Parse( lineData[ 0 ] );
        BNpcNameId = uint.Parse( lineData[ 1 ] );
        TerritoryTypeId = uint.Parse( lineData[ 2 ] );
        var positionData = lineData[3].Split(";").Select(float.Parse).ToList();
        Position = new Vector3(positionData[0], positionData[1], positionData[2]);
        Subtype = byte.Parse(lineData[4]);
    }

    public string[] ToCsv()
    {
        List<String> data = new List<string>()
        {
            BNpcBaseId.ToString(),
            BNpcNameId.ToString(),
            TerritoryTypeId.ToString(),
            Position.X + ";" + Position.Y + ";" + Position.Z,
            Subtype.ToString()
        };
        return data.ToArray();
    }

    public bool IncludeInCsv()
    {
        return false;
    }

    public virtual void PopulateData( GameData gameData, Language language )
    {
        BNpcBase = new LazyRow< BNpcBase >( gameData, BNpcBaseId, language );
        BNpcName = new LazyRow< BNpcName >( gameData, BNpcNameId, language );
        TerritoryType = new LazyRow< TerritoryType >( gameData, TerritoryTypeId, language );
    }
}