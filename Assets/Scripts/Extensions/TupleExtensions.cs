using UnityEngine;

namespace Extensions
{
    public static class TupleExtensions
    {
        public static Vector2 ToVector2(this (float x, float y) tuple) => new Vector2(tuple.x, tuple.y);
        
        public static Vector2Int ToVector2Int(this (int x, int y) tuple) => new Vector2Int(tuple.x, tuple.y);
    }
}