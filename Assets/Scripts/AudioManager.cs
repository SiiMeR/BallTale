using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        //DontDestroyOnLoad(this);
        
        SetMusicVolume(PlayerPrefs.GetInt("MusicVolume") / 10f);
        SetSoundVolume(PlayerPrefs.GetInt("SoundVolume") / 10f);
        
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

    public void Stop(string audioName)
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
                
                if (source.clip == clip)
                {
                    source.Stop();
                    return;
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not find audio: " + succ);
        }
    }

    public IEnumerator FadeToNextMusic(string nextMusicName, float time = 2.0f)
    {
        if (nextMusicName == null || nextMusicName == "")
        {
            yield break;
        }
        
        AudioClip clip;
        AudioSource src = null;
        bool succ = audioMap.TryGetValue(nextMusicName, out clip);
        if (succ)
        {
            foreach (AudioSource source in sourcePool)
            {
                if (source.loop)
                {
                    src = source;


                    break;
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not find audio: " + succ);
        }

        if (src == null)
        {
            
            yield break;
        }

        float timer = 0;

        while ((timer += Time.unscaledDeltaTime) < time/2)
        {
            src.volume = Mathf.Lerp(1,0, timer / (time/2.0f));
            yield return null;
        }

        src.clip = clip;

        timer = 0f;

        while ((timer += Time.unscaledDeltaTime) < time / 2)
        {
            src.volume = Mathf.Lerp(0,1, timer / (time/2.0f));
            yield return null;
        }


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

    public void Play(AudioClip clip, float vol = 1f, bool isLooping = false, Vector3? position = null)
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


}
