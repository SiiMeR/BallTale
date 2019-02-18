using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Extensions
{
    public static class TilemapExtensions
    {
        private static readonly Vector3Int[] TileDirections =
        {
            Vector3Int.down,
            Vector3Int.up,
            Vector3Int.left,
            Vector3Int.right,
            Vector3Int.down + Vector3Int.left,
            Vector3Int.down + Vector3Int.right,
            Vector3Int.up + Vector3Int.left,
            Vector3Int.up + Vector3Int.right
        };

        private static int GetDirectionSum(Vector3Int direction)
        {
            return direction == Vector3Int.down ? 1 :
                direction == Vector3Int.left ? 2 :
                direction == Vector3Int.up ? 4 :
                direction == Vector3Int.right ? 8 :
                direction == Vector3Int.down + Vector3Int.left ? 16 :
                direction == Vector3Int.up + Vector3Int.left ? 32 :
                direction == Vector3Int.up + Vector3Int.right ? 64 :
                128;
        }


        public static Dictionary<Vector3Int, int> GetTilesOfTypeInDirection<T>(this Tilemap tilemap,
            Vector3Int direction,
            Tile tileOfType) where T : TileBase
        {
            var positions = new Dictionary<Vector3Int, int>();

            for (var y = tilemap.origin.y; y < tilemap.origin.y + tilemap.size.y; y++)
            for (var x = tilemap.origin.x; x < tilemap.origin.x + tilemap.size.x; x++)
            {
                var tilePos = new Vector3Int(x, y, 0);
                var tile = tilemap.GetTile<T>(tilePos);

                if (tile) continue;

                var adjacentPosition = tilePos + direction;

                var adjacentTile = tilemap.GetTile<T>(adjacentPosition);

                if (!adjacentTile) continue;

                if (adjacentTile.name != tileOfType.name) continue;


                if (!positions.ContainsKey(tilePos))
                    positions.Add(tilePos, GetDirectionSum(direction));
                else
                    positions[tilePos] += GetDirectionSum(direction);
            }


            return positions;
        }

        public static Dictionary<Vector3Int, int> GetTileOfTypeInAllDirections<T>(this Tilemap tilemap, Tile tileOfType)
            where T : TileBase
        {
            var positions = new Dictionary<Vector3Int, int>();

            return TileDirections.Select(direction => tilemap.GetTilesOfTypeInDirection<T>(direction, tileOfType))
                .Aggregate(positions, (current, tileOfTypeInDirection) =>
                    tileOfTypeInDirection
                        .Keys
                        .Union(current.Keys)
                        .Select(key =>
                            {
                                tileOfTypeInDirection.TryGetValue(key, out var fval);
                                current.TryGetValue(key, out var bval);
                                return new KeyValuePair<Vector3Int, int>(key, fval + bval);
                            }
                        )
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        public static Dictionary<Vector3Int, int> GetEmptyAdjacentTilesInDirection<T>(this Tilemap tilemap,
            Vector3Int direction) where T : TileBase
        {
            var positions = new Dictionary<Vector3Int, int>();

            for (var y = tilemap.origin.y; y < tilemap.origin.y + tilemap.size.y; y++)
            for (var x = tilemap.origin.x; x < tilemap.origin.x + tilemap.size.x; x++)
            {
                var tilePos = new Vector3Int(x, y, 0);
                var tile = tilemap.GetTile<T>(tilePos);

                if (!tile) continue;

                var adjacentPosition = tilePos + direction;

                var adjacentTile = tilemap.GetTile<T>(adjacentPosition);

                if (adjacentTile) continue;


                if (!positions.ContainsKey(tilePos))
                    positions.Add(tilePos, GetDirectionSum(direction));
                else
                    positions[tilePos] += GetDirectionSum(direction);
            }

            return positions;
        }

        public static void RemoveTileOfTypeFromTilemap<T>(this Tilemap tilemap, Tile tileToRemove) where T : TileBase
        {
            for (var y = tilemap.origin.y; y < tilemap.origin.y + tilemap.size.y; y++)
            for (var x = tilemap.origin.x; x < tilemap.origin.x + tilemap.size.x; x++)
            {
                var tilePos = new Vector3Int(x, y, 0);
                var tile = tilemap.GetTile<T>(tilePos);

                if (tile == null) continue;

                if (tile.name.Equals(tileToRemove.name)) tilemap.SetTile(tilePos, null);
            }
        }
    }
}