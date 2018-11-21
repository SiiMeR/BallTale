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
    public Tile Underdirt;
    public Tile Overdirt;
    public Tile SidedirtRight;
    public Tile SidedirtLeft;

    public Tile SWDirt;
    public Tile SEDirt;
    public Tile NWDirt;
    public Tile NEDirt;
    

#if UNITY_EDITOR

    public void FillAllWithDirt()
    {
        var allPositions = MainTilemap.GetEmptyAdjacentTilesInAllDirections<Tile>();

        foreach (var (position, nearbySum) in allPositions.Select(x => (x.Key, x.Value)))
        {
            switch (nearbySum)
            {
                case 0: // no empty
                    break;
                case 1: // down only
                    break;
                case 2: // left only
                    break;
                case 3: // d + l
                    break;
                case 4: // up only
                    break;
                case 5: // u + d
                    break;
                case 6: // u + l
                    break;
                case 7: // u + d + l
                    break;
                case 8: // right only
                    break;
                case 9: // r + d
                    break;
                case 10: // r + l
                    break;
                case 11: // r + d + l
                    break;
                case 12: // r + u
                    break;
                case 13: // r + u + d
                    break;
                case 14: // r + u + l
                    break;
                case 15: // r + u + d + l
                    break;

            }
        }

    }
    
    
    public void FillWithDirt(Vector3Int direction)
    {
        var tileType = direction == Vector3Int.down ? Underdirt    :
                       direction == Vector3Int.up   ? Overdirt     :
                       direction == Vector3Int.left ? SidedirtRight:
                                                      SidedirtLeft ;
        
        
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
        FGTilemap.RemoveTileOfTypeFromTilemap<Tile>(Underdirt);
        FGTilemap.RemoveTileOfTypeFromTilemap<Tile>(Overdirt);
        FGTilemap.RemoveTileOfTypeFromTilemap<Tile>(SidedirtLeft);
        FGTilemap.RemoveTileOfTypeFromTilemap<Tile>(SidedirtRight);
    }
    
#endif


}
