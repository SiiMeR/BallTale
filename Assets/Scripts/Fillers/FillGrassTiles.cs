using System.Linq;
using Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FillGrassTiles : MonoBehaviour
{
    public Tilemap FGTilemap;

    public Tilemap MainTilemap;
    public Tile Overgrass;
    public Tile SidegrassLeft;
    public Tile SidegrassRight;

    public Tile Tile;
    public Tile Undergrass;

#if UNITY_EDITOR

    public void FillWithGrass(Vector3Int direction)
    {
        var tileType = direction == Vector3Int.down ? Undergrass :
            direction == Vector3Int.up ? Overgrass :
            direction == Vector3Int.left ? SidegrassRight :
            SidegrassLeft;


        var positionsInDirection = MainTilemap.GetEmptyAdjacentTilesInDirection<Tile>(direction);

        foreach (var (position, nearbySum) in positionsInDirection.Select(x => (x.Key, x.Value)))
        {
            var tilePos = position + direction;

            var tile = MainTilemap.GetTile(position);

            if (tile && tile.name.Equals(Tile.name)) FGTilemap.SetTile(tilePos, tileType);
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