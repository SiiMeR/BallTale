using BayatGames.SaveGameFree.Types;
using UnityEngine;

namespace BayatGames.SaveGameFree.Examples
{
    public class ExampleSaveRotation : MonoBehaviour
    {
        public string identifier = "exampleSaveRotation.dat";
        public bool loadOnStart = true;

        public Transform target;

        private void Start()
        {
            if (loadOnStart) Load();
        }

        private void Update()
        {
            var rotation = target.rotation.eulerAngles;
            rotation.z += Input.GetAxis("Horizontal");
            target.rotation = Quaternion.Euler(rotation);
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public void Save()
        {
            SaveGame.Save<QuaternionSave>(identifier, target.rotation, SerializerDropdown.Singleton.ActiveSerializer);
        }

        public void Load()
        {
            target.rotation = SaveGame.Load<QuaternionSave>(
                identifier,
                Quaternion.identity,
                SerializerDropdown.Singleton.ActiveSerializer);
        }
    }
}