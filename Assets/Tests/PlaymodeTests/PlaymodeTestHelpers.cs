using UnityEngine;

namespace Tests
{
    public static class TestHelpers
    {
        public static GameObject InstantiatePrefab(string prefabName)
        {
            var prefab = Object.Instantiate(Resources.Load($"Prefabs/{prefabName}", typeof(GameObject))) as GameObject;

            if (!prefab) Debug.LogError($"Prefab not found in Resources/Prefabs/ for name {prefabName}");

            return prefab;
        }
    }
}