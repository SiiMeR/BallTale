using System;
using System.Linq;
using UnityEngine;

namespace BayatGames.SaveGameFree.Types
{
    /// <summary>
    ///     A class that allows creating and modifying meshes from scripts.
    /// </summary>
    [Serializable]
    public class MeshSave
    {
        public Color[] colors;
        public Color32[] colors32;
        public Vector3Save[] normals;
        public int[] triangles;
        public Vector2Save[] uv;

        public Vector3Save[] vertices;

        public MeshSave(Mesh mesh)
        {
            vertices = mesh.vertices.Cast<Vector3Save>().ToArray();
            triangles = mesh.triangles;
            uv = mesh.uv.Cast<Vector2Save>().ToArray();
            normals = mesh.normals.Cast<Vector3Save>().ToArray();
            colors = mesh.colors.Cast<Color>().ToArray();
            colors32 = mesh.colors32.Cast<Color32>().ToArray();
        }

        public static implicit operator MeshSave(Mesh mesh)
        {
            return new MeshSave(mesh);
        }

        public static implicit operator Mesh(MeshSave mesh)
        {
            var newMesh = new Mesh();
            newMesh.vertices = mesh.vertices.Cast<Vector3>().ToArray();
            newMesh.triangles = mesh.triangles;
            newMesh.uv = mesh.uv.Cast<Vector2>().ToArray();
            newMesh.normals = mesh.normals.Cast<Vector3>().ToArray();
            newMesh.colors = mesh.colors.Cast<Color>().ToArray();
            newMesh.colors32 = mesh.colors32.Cast<Color32>().ToArray();
            return newMesh;
        }
    }
}