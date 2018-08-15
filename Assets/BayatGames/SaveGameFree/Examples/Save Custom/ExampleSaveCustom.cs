using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BayatGames.SaveGameFree.Examples
{
    public class ExampleSaveCustom : MonoBehaviour
    {
        public CustomData customData;
        public InputField highScoreInputField;
        public string identifier = "exampleSaveCustom";
        public bool loadOnStart = true;
        public InputField scoreInputField;

        private void Start()
        {
            if (loadOnStart) Load();
        }

        public void SetScore(string score)
        {
            customData.score = int.Parse(score);
        }

        public void SetHighScore(string highScore)
        {
            customData.highScore = int.Parse(highScore);
        }

        public void Save()
        {
            SaveGame.Save(identifier, customData, SerializerDropdown.Singleton.ActiveSerializer);
        }

        public void Load()
        {
            customData = SaveGame.Load(
                identifier,
                new CustomData(),
                SerializerDropdown.Singleton.ActiveSerializer);
            scoreInputField.text = customData.score.ToString();
            highScoreInputField.text = customData.highScore.ToString();
        }

        [Serializable]
        public struct Level
        {
            public bool unlocked;
            public bool completed;

            public Level(bool unlocked, bool completed)
            {
                this.unlocked = unlocked;
                this.completed = completed;
            }
        }

        [Serializable]
        public class CustomData
        {
            public int highScore;
            public List<Level> levels;

            public int score;

            public CustomData()
            {
                score = 0;
                highScore = 0;

                // Dummy data
                levels = new List<Level>
                {
                    new Level(true, false),
                    new Level(false, false),
                    new Level(false, true),
                    new Level(true, false)
                };
            }
        }
    }
}