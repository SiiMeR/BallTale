using System.Collections.Generic;
using BayatGames.SaveGameFree.Serializers;
using UnityEngine.UI;

namespace BayatGames.SaveGameFree.Examples
{
    public class SerializerDropdown : Dropdown
    {
        private static readonly ISaveGameSerializer[] m_Serializers =
        {
            new SaveGameXmlSerializer(),
            new SaveGameJsonSerializer(),
            new SaveGameBinarySerializer()
        };

        protected ISaveGameSerializer m_ActiveSerializer;

        public static SerializerDropdown Singleton { get; private set; }

        public ISaveGameSerializer ActiveSerializer
        {
            get
            {
                if (m_ActiveSerializer == null) m_ActiveSerializer = new SaveGameJsonSerializer();
                return m_ActiveSerializer;
            }
        }

        protected override void Awake()
        {
            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }

            Singleton = this;
            base.Awake();
            options = new List<OptionData>
            {
                new OptionData("XML"),
                new OptionData("JSON"),
                new OptionData("Binary")
            };
            onValueChanged.AddListener(OnValueChanged);
            value = SaveGame.Load("serializer", 0, new SaveGameJsonSerializer());
        }

        protected virtual void OnValueChanged(int index)
        {
            m_ActiveSerializer = m_Serializers[index];
        }

        protected virtual void OnApplicationQuit()
        {
            SaveGame.Save("serializer", value, new SaveGameJsonSerializer());
        }
    }
}