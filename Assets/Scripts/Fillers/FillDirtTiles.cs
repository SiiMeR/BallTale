using System.Linq;
using Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FillDirtTiles : MonoBehaviour
{
    public Tilemap FGTilemap;

    public Tilemap MainTilemap;

    public Tile Tile;

    // down, left, up, right, sw, nw, ne, se, edgesw, edgenw, edgene, edgese,  all, ldr, lur, tld, trd, lr, ud  -- tiles in this order
    //    0,    1,  2,     3,  4,  5,  6,  7,      8,      9,     10,     11,  12 ,  13,  14,  15,  16, 17, 18
    public Tile[] Tiles;


#if UNITY_EDITOR

    /// <summary>
    ///     Fill all nearby tiles with crust tiles depending if they are empty
    /// </summary>
    public void FillAllWithDirt()
    {
        var allPositions = MainTilemap.GetTileOfTypeInAllDirections<Tile>(Tile);

        foreach (var (position, nearbySum) in allPositions.Select(x => (x.Key, x.Value)))
        {
            var tile = ScriptableObject.CreateInstance<Tile>();

            switch (nearbySum % 16) // normal cases, no "notches"
            {
                case 0: // no empty
                    break;

                case 1: // down only
                    tile = Tiles[0];
                    break;

                case 2: // left only
                    tile = Tiles[1];
                    break;

                case 4: // up only
                    tile = Tiles[2];
                    break;

                case 8: // right only
                    tile = Tiles[3];
                    break;

                case 3: // d + l
                    tile = Tiles[4];
                    break;

                case 6: // u + l
                    tile = Tiles[5];
                    break;

                case 12: // r + u
                    tile = Tiles[6];
                    break;

                case 9: // r + d 
                    tile = Tiles[7];
                    break;

                case 15: // r + u + d + l
                    tile = Tiles[12];
                    break;

                // ldr
                case 11: // r + d + l
                    tile = Tiles[13];
                    break;

                // lur
                case 14: // r + u + l
                    tile = Tiles[14];
                    break;

                // tld
                case 7: // u + d + l  
                    tile = Tiles[15];
                    break;

                // trd
                case 13: // r + u + d
                    tile = Tiles[16];
                    break;

                case 10:
                    tile = Tiles[17]; // multiples of 16
                    break;

                case 5: // u + d
                    tile = Tiles[18];
                    break;
            }

            switch (nearbySum) // special notch cases
            {
                case 16: // sw
                    tile = Tiles[8];
                    break;

                case 32: // nw
                    tile = Tiles[9];
                    break;

                case 64: // ne
                    tile = Tiles[10];
                    break;

                case 128: // se
                    tile = Tiles[11];
                    break;
            }

            FGTilemap.SetTile(position, tile);
        }
    }


    public void FillWithDirt(Vector3Int direction)
    {
        var tileType = direction == Vector3Int.down ? Tiles[0] :
            direction == Vector3Int.left ? Tiles[1] :
            direction == Vector3Int.up ? Tiles[2] :
            Tiles[3];


        var positionsInDirection = MainTilemap.GetEmptyAdjacentTilesInDirection<Tile>(direction);

        foreach (var (position, nearbySum) in positionsInDirection.Select(x => (x.Key, x.Value)))
        {
            var tilePos = position + direction;

            var tile = MainTilemap.GetTile(position);

            if (tile && tile.name.Equals(Tile.name)) FGTilemap.SetTile(tilePos, tileType);
        }
    }

    public void RemoveAllDirt()
    {
        foreach (var tile in Tiles) FGTilemap.RemoveTileOfTypeFromTilemap<Tile>(tile);
    }

#endif
}