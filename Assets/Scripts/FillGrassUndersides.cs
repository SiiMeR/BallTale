using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FillGrassUndersides : MonoBehaviour
{

    public Tilemap Tilemap;

    public Tile Tile;
    public Tile UndersideTile;
    
    // Start is called before the first frame update
    void Start()
    {
        var tiles = Tilemap.GetUnderTiles<Tile>();
        Debug.Log($"{tiles.Length}");
        
        foreach (var tile in tiles)
        {
         //   Tilemap.SetTile(tile.gameObject.transform.position,);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
