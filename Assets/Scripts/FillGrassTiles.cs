using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;


public class FillGrassTiles : MonoBehaviour
{

    public Tilemap MainTilemap;
    public Tilemap FGTilemap;

    public Tile Tile;
    public Tile Undergrass;
    public Tile Overgrass;
    public Tile SidegrassRight;
    public Tile SidegrassLeft;

#if UNITY_EDITOR
    
    public void FillWithGrass(Vector3Int direction)
    {
        var tileType = direction == Vector3Int.down ? Undergrass    :
                       direction == Vector3Int.up   ? Overgrass     :
                       direction == Vector3Int.left ? SidegrassRight:
                                                      SidegrassLeft ;
        
        
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
    
    public void RemoveAllGrass()
    {
        FGTilemap.RemoveTileOfTypeFromTilemap<Tile>(Undergrass);
        FGTilemap.RemoveTileOfTypeFromTilemap<Tile>(Overgrass);
        FGTilemap.RemoveTileOfTypeFromTilemap<Tile>(SidegrassLeft);
        FGTilemap.RemoveTileOfTypeFromTilemap<Tile>(SidegrassRight);
    }
    
#endif


}
