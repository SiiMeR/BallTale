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

    /// <summary>
    /// Fill all nearby tiles with crust tiles depending if they are empty
    /// </summary>
    /// TODO: CHECK EVERY EMPTY BLOCK FOR NEARBY DIRT BLOCKS, NOT THE OTHER WAY AROUND. THIS DOESNT WORK lol
    public void FillAllWithDirt()
    {
        var allPositions = MainTilemap.GetEmptyAdjacentTilesInAllDirections<Tile>();

        foreach (var (position, nearbySum) in allPositions.Select(x => (x.Key, x.Value)))
        {
            var tile = ScriptableObject.CreateInstance<Tile>();
            var tilePos = Vector3Int.zero;
            
            switch (nearbySum)
            {
                case 0: // no empty
                    continue;
                case 1: // down only
                    tile = Underdirt;
                    tilePos = Vector3Int.down;
                    break;
                case 2: // left only
                    tile = SidedirtLeft;
                    tilePos = Vector3Int.left;
                    break;
                case 3: // d + l
                    tile = SWDirt;
                    tilePos = Vector3Int.down + Vector3Int.left;
                    break;
                case 4: // up only
                    tile = Overdirt;
                    tilePos = Vector3Int.up;
                    break;
                case 5: // u + d
                    tile = Underdirt;
                    tilePos = Vector3Int.up; 
                    break;
                case 6: // u + l
                    tile = NWDirt;
                    tilePos = Vector3Int.up + Vector3Int.left;
                    break;
                case 7: // u + d + l
                    tile = Underdirt;
                    tilePos = Vector3Int.down;
                    break;
                case 8: // right only
                    tile = SidedirtRight;
                    tilePos = Vector3Int.right;
                    break;
                case 9: // r + d
                    tile = SEDirt;
                    tilePos = Vector3Int.right + Vector3Int.down;
                    break;
                case 10: // r + l
                    tile = Underdirt;
                    tilePos = Vector3Int.down;
                    break;
                case 11: // r + d + l
                    tile = Underdirt;
                    tilePos = Vector3Int.down;
                    break;
                case 12: // r + u
                    tile = NEDirt;
                    tilePos = Vector3Int.right + Vector3Int.up;
                    break;
                case 13: // r + u + d
                    tile = Underdirt;
                    tilePos = Vector3Int.down;
                    break;
                case 14: // r + u + l
                    tile = Underdirt;
                    tilePos = Vector3Int.down;
                    break;
                case 15: // r + u + d + l
                    tile = Underdirt;
                    tilePos = Vector3Int.down;
                    break;
            }


            tilePos += position;
            
            var tilemapTile = MainTilemap.GetTile(position);

            if (tilemapTile && tilemapTile.name.Equals(Tile.name))
            {    
                FGTilemap.SetTile(tilePos, tile);
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
