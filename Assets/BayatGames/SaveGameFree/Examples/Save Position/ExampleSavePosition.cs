using BayatGames.SaveGameFree.Types;
using UnityEngine;

namespace BayatGames.SaveGameFree.Examples
{
    public class ExampleSavePosition : MonoBehaviour
    {
        public string identifier = "exampleSavePosition.dat";
        public bool loadOnStart = true;

        public Transform target;

        private void Start()
        {
            if (loadOnStart) Load();
        }

        private void Update()
        {
            var newPosition = target.position;
            newPosition.x += Input.GetAxis("Horizontal");
            newPosition.y += Input.GetAxis("Vertical");
            target.position = newPosition;
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public void Save()
        {
            SaveGame.Save<Vector3Save>(identifier, target.position, SerializerDropdown.Singleton.ActiveSerializer);
        }

        public void Load()
        {
            target.position = SaveGame.Load<Vector3Save>(
                identifier,
                Vector3.zero,
                SerializerDropdown.Singleton.ActiveSerializer);
        }
    }
}