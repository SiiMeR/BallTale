using BayatGames.SaveGameFree.Types;
using UnityEngine;

namespace BayatGames.SaveGameFree.Examples
{
    public class ExampleSaveScale : MonoBehaviour
    {
        public string identifier = "exampleSaveScale.dat";
        public bool loadOnStart = true;

        public Transform target;

        private void Start()
        {
            if (loadOnStart) Load();
        }

        private void Update()
        {
            var scale = target.localScale;
            scale.x += Input.GetAxis("Horizontal");
            scale.y += Input.GetAxis("Vertical");
            target.localScale = scale;
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public void Save()
        {
            SaveGame.Save<Vector3Save>(identifier, target.localScale, SerializerDropdown.Singleton.ActiveSerializer);
        }

        public void Load()
        {
            target.localScale = SaveGame.Load(
                identifier,
                new Vector3Save(1f, 1f, 1f),
                SerializerDropdown.Singleton.ActiveSerializer);
        }
    }
}