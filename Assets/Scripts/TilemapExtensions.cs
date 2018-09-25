using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapExtensions
{
    public static T[] GetUnderTiles<T>(this Tilemap tilemap) where T : TileBase
    {
        var tiles = new List<T>();

        for (var y = tilemap.origin.y; y < (tilemap.origin.y + tilemap.size.y); y++)
        {
            for (var x = tilemap.origin.x; x < (tilemap.origin.x + tilemap.size.x); x++)
            {
                var tile = tilemap.GetTile<T>(new Vector3Int(x, y, 0));
                if (tile != null)
                {
                    var underTile = tilemap.GetTile<T>(new Vector3Int(x, y - 1, 0));

                    if (underTile == null)
                    {
                        tiles.Add(underTile);                        
                    }

                }
            }
        }

        return tiles.ToArray();
    }
}