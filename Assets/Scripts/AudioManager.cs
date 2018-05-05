using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public List<AudioClip> audioClips;
    public Dictionary<string, AudioClip> audioMap;

    public float musicVolume = 1;
    public float soundVolume = 1;

    public int maxAudios = 16;
    private List<AudioSource> sourcePool;

    public AudioManager()
    {
        instance = this;
    }

    void Awake()
    {
        audioMap = new Dictionary<string, AudioClip>();

        foreach (AudioClip clip in audioClips)
        {
            audioMap.Add(clip.name, clip);
        }

        sourcePool = new List<AudioSource>();
        GameObject parent = new GameObject("Audios");

        for (int i=0; i<maxAudios; i++)
        {
            GameObject gob = new GameObject("Audio");
            gob.transform.parent = parent.transform;
            AudioSource audioSource = gob.AddComponent<AudioSource>();
            sourcePool.Add(audioSource);
        }

    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (sourcePool != null)
        {
            foreach (AudioSource source in sourcePool)
            {
                if (source.isPlaying && source.loop)
                {
                    source.volume = volume;
                }
            }
        }
    }

    public void SetSoundVolume(float volume)
    {
        soundVolume = volume;
    }

    public void StopAllMusic()
    {
        if (sourcePool != null)
        {
            foreach (AudioSource source in sourcePool)
            {
                if (source.isPlaying && source.loop)
                {
                    source.Stop();
                }
            }
        }
    }

    public void PlayRandom(string[] audioName, float vol = 1f, bool isLooping = false, Vector3? position = null)
    {
        int i = Random.Range(0, audioName.Length);
        Play(audioName[i], vol, isLooping, position);
    }

    public void Play(string audioName, float vol = 1f, bool isLooping = false, Vector3? position = null)
    {
        if (audioName == null || audioName == "")
        {
            return;
        }
        AudioClip clip;
        bool succ = audioMap.TryGetValue(audioName, out clip);
        if (succ)
        {
            foreach (AudioSource source in sourcePool)
            {
                if (!source.isPlaying)
                {
                    source.clip = clip;
                    source.volume = vol * (isLooping ? musicVolume : soundVolume);
                    source.loop = isLooping;
                    source.Play();

                    if (position != null)
                    {
                        source.transform.position = (Vector3)position;
                    }
                    else
                    {
                        source.transform.localPosition = Vector3.zero;
                    }

                    break;
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not find audio: " + succ);
        }
    }

    
}
