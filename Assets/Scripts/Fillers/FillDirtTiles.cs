using System;
using System.Collections;
using System.Collections.Generic;
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

#if UNITY_EDITOR
    
    public void FillWithDirt(Vector3Int direction)
    {
        var tileType = direction == Vector3Int.down ? Underdirt    :
                       direction == Vector3Int.up   ? Overdirt     :
                       direction == Vector3Int.left ? SidedirtRight:
                                                      SidedirtLeft ;
        
        
        var positionsInDirection = MainTilemap.GetEmptyAdjacentTilesInDirection<Tile>(direction);
        
        foreach (var position in positionsInDirection)
        {
            var tilePos =  position + direction;
            
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
