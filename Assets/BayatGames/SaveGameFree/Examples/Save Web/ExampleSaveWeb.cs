using System.Collections;
using BayatGames.SaveGameFree.Types;
using UnityEngine;

namespace BayatGames.SaveGameFree.Examples
{
    public class ExampleSaveWeb : MonoBehaviour
    {
        public bool encode = true;
        public string encodePassword = "h@e#ll$o%^";
        public string identifier = "exampleSaveWeb";
        public bool loadOnStart = true;
        public string password = "$@ve#game%free";

        public Transform target;
        public string url = "http://www.example.com/savegamefree.php";
        public string username = "savegamefree";

        private void Start()
        {
            Load();
        }

        private void Update()
        {
            var position = target.position;
            position.x += Input.GetAxis("Horizontal");
            position.y += Input.GetAxis("Vertical");
            target.position = position;
        }

        public void Load()
        {
            StartCoroutine(LoadEnumerator());
        }

        public void Save()
        {
            StartCoroutine(SaveEnumerator());
        }

        private IEnumerator LoadEnumerator()
        {
            Debug.Log("Downloading...");
            var web = new SaveGameWeb(
                username,
                password,
                url,
                encode,
                encodePassword,
                SerializerDropdown.Singleton.ActiveSerializer);
            yield return StartCoroutine(web.Download(identifier));
            target.position = web.Load<Vector3Save>(identifier, Vector3.zero);
            Debug.Log("Download Done.");
        }

        private IEnumerator SaveEnumerator()
        {
            Debug.Log("Uploading...");
            var web = new SaveGameWeb(
                username,
                password,
                url,
                encode,
                encodePassword,
                SerializerDropdown.Singleton.ActiveSerializer);
            yield return StartCoroutine(web.Save<Vector3Save>(identifier, target.position));
            Debug.Log("Upload Done.");
        }
    }
}