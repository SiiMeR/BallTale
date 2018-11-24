using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;


public class FillDirtTiles : MonoBehaviour
{

    public Tilemap MainTilemap;
    public Tilemap FGTilemap;

    public Tile Tile;
    // down, left, up, right, sw, nw, ne, se, edgesw, edgenw, edgene, edgese,  all, ldr, lur, tld, trd, lr, ud  -- tiles in this order
    //    0,    1,  2,     3,  4,  5,  6,  7,      8,      9,     10,     11,  12 ,  13,  14,  15,  16, 17, 18
    public Tile[] Tiles;
    

#if UNITY_EDITOR

    /// <summary>
    /// Fill all nearby tiles with crust tiles depending if they are empty
    /// </summary>
    /// 
    public void FillAllWithDirt()
    {
        var allPositions = MainTilemap.GetTileOfTypeInAllDirections<Tile>(Tile);

        foreach (var (position, nearbySum) in allPositions.Select(x => (x.Key, x.Value)))
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            
            switch (nearbySum)
            {
                case 0: // no empty
                    continue;
                case 1: // down only
                case 129:
                case 241:
                    tile = Tiles[0];
                    break;
                case 2: // left only
                case 50: // sw nw l
                case 18: // sw l    
                case 34: // l nw
                    tile = Tiles[1];
                    break;
                case 3: // d + l
                    tile = Tiles[4];
                    break;
                
                
                case 4: // up only
                case 100: // nw u ne
                case 36: // nw u
                case 68: // u ne
                    tile = Tiles[2];
                    break;
                case 5: // u + d
                    
                    break;
                case 6: // u + l
                    tile = Tiles[5];
                    break;
                case 7: // u + d + l

                    break;
                case 8: // right only
                case 40:
                case 72:
                case 136:
                case 200:    
                    tile = Tiles[3];
                    break;
                
                case 9: // r + d
                
                case 153:  
                case 217:
                case 137:
                case 201:
                    tile = Tiles[7];
                    break;
                case 10: // r + l

                    break;
                case 11: // r + d + l

                    break;
                case 12: // r + u
                case 76:
                case 108:
                case 236:
                case 204:
                    
                    tile = Tiles[6];
                    break;
                case 13: // r + u + d

                    break;
                case 14: // r + u + l

                    break;
                case 15: // r + u + d + l

                    break;
                case 16: // sw
                    tile = Tiles[8];
                    break;
                
                case 17: // sw d
                    tile = Tiles[0];
                    break;
                case 32: // nw
                    tile = Tiles[9];
                    break;
                case 64: // ne
                case 96:
                    tile = Tiles[10];
                    break;
                case 128: // se
                    tile = Tiles[11];
                    break;
                
                case 145: // sw se d
                    tile = Tiles[0];
                    break;
                
                case 147: // l sw d se
                case 19:    
                case 51:
                    tile = Tiles[4];
                    break;
                case 255:
                    tile = Tiles[12];
                    break;
                
                case 54: // sw l nw u
                case 118: // sw l nw u ne
                case 38: // l nw u
                case 102: // l nw u ne   
                    tile = Tiles[5];
                    break;
                
                
                
                // ldr
                case 155:
                    tile = Tiles[13];
                    break;
                // lur
                case 110: 
                case 254:
                    tile = Tiles[14];
                    break;
                
                // tld
                case 247:
                    tile = Tiles[15];
                    break;
                
                // trd
                case 253:
                    tile = Tiles[16];
                    break;
                case 245:
                case 181:
                case 213:
                    tile = Tiles[18];
                    break;
                
                case 250:
                    tile = Tiles[17];
                    break;
            }

            FGTilemap.SetTile(position, tile);
            
        }
    }
    
    
    public void FillWithDirt(Vector3Int direction)
    {
        var tileType = direction == Vector3Int.down ? Tiles[0]    :
                       direction == Vector3Int.up   ? Tiles[2]     :
                       direction == Vector3Int.left ? Tiles[1]:
                                                      Tiles[3] ;
        
        
        var positionsInDirection = MainTilemap.GetEmptyAdjacentTilesInDirection<Tile>(direction);
        
        foreach (var (position, nearbySum) in positionsInDirection.Select(x => (x.Key, x.Value)))
        {
            var tilePos = position + direction;
            
            var tile = MainTilemap.GetTile(position);

            if (tile && tile.name.Equals(Tile.name))
            {    
                FGTilemap.SetTile(tilePos, tileType);
            }
        }
        
    }
    
    public void RemoveAllDirt()
    {
        foreach (var tile in Tiles)
        {
            FGTilemap.RemoveTileOfTypeFromTilemap<Tile>(tile);
        }      
    }
    
#endif


}
