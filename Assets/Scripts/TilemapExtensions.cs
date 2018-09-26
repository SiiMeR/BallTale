using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapExtensions
{
    public static Vector3Int[] GetEmptyAdjacentTilesInDirection<T>(this Tilemap tilemap, Vector3Int direction) where T : TileBase
    {
        var positions = new List<Vector3Int>();
        
        for (var y = tilemap.origin.y; y < (tilemap.origin.y + tilemap.size.y); y++)
        {
            for (var x = tilemap.origin.x; x < (tilemap.origin.x + tilemap.size.x); x++)
            {
                var tilePos = new Vector3Int(x, y, 0);
                var tile = tilemap.GetTile<T>(tilePos);

                if (tile == null) continue;

                var adjacentPosition = tilePos + direction;
                   
                var adjacentTile = tilemap.GetTile<T>(adjacentPosition);

                if (adjacentTile == null)
                {
                    positions.Add(tilePos);                    
                }


            }
        }

        return positions.ToArray();
    }
    
    public static void RemoveTileOfTypeFromTilemap<T>(this Tilemap tilemap, Tile tileToRemove) where T : TileBase
    {

        for (var y = tilemap.origin.y; y < (tilemap.origin.y + tilemap.size.y); y++)
        {
            for (var x = tilemap.origin.x; x < (tilemap.origin.x + tilemap.size.x); x++)
            {
                var tilePos = new Vector3Int(x, y, 0);
                var tile = tilemap.GetTile<T>(tilePos);

                if (tile == null) continue;

                if (tile.name.Equals(tileToRemove.name))
                {
                    tilemap.SetTile(tilePos, null);
                }


            }
        }

    }
}