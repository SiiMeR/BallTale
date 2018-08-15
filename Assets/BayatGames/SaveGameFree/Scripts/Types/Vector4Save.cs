using System;
using UnityEngine;

namespace BayatGames.SaveGameFree.Types
{
    /// <summary>
    ///     Representation of four-dimensional vectors.
    /// </summary>
    [Serializable]
    public struct Vector4Save
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vector4Save(float x)
        {
            this.x = x;
            y = 0f;
            z = 0f;
            w = 0f;
        }

        public Vector4Save(float x, float y)
        {
            this.x = x;
            this.y = y;
            z = 0f;
            w = 0f;
        }

        public Vector4Save(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            w = 0f;
        }

        public Vector4Save(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector4Save(Vector2 vector)
        {
            x = vector.x;
            y = vector.y;
            z = 0f;
            w = 0f;
        }

        public Vector4Save(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
            w = 0f;
        }

        public Vector4Save(Vector4 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
            w = vector.w;
        }

        public Vector4Save(Vector2Save vector)
        {
            x = vector.x;
            y = vector.y;
            z = 0f;
            w = 0f;
        }

        public Vector4Save(Vector3Save vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
            w = 0f;
        }

        public Vector4Save(Vector4Save vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
            w = vector.w;
        }

        public static implicit operator Vector4Save(Vector2 vector)
        {
            return new Vector4Save(vector);
        }

        public static implicit operator Vector2(Vector4Save vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static implicit operator Vector4Save(Vector3 vector)
        {
            return new Vector4Save(vector);
        }

        public static implicit operator Vector3(Vector4Save vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }

        public static implicit operator Vector4Save(Vector4 vector)
        {
            return new Vector4Save(vector);
        }

        public static implicit operator Vector4(Vector4Save vector)
        {
            return new Vector4(vector.x, vector.y, vector.z);
        }

        public static implicit operator Vector4Save(Vector2Save vector)
        {
            return new Vector4Save(vector);
        }

        public static implicit operator Vector2Save(Vector4Save vector)
        {
            return new Vector2Save(vector);
        }

        public static implicit operator Vector4Save(Vector3Save vector)
        {
            return new Vector4Save(vector);
        }

        public static implicit operator Vector3Save(Vector4Save vector)
        {
            return new Vector3Save(vector);
        }
    }
}