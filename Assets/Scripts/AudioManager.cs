using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
  //  public static AudioManager instance;


    public List<AudioClip> audioClips;


    private Dictionary<string, AudioClip> audioMap;
    
    public Dictionary<string, AudioClip> AudioMap
    {
        get
        {
            if (audioMap == null)
            {
                CreateAudioMap();
            }

            return audioMap;
        }
        set { audioMap = value; }
    }

    public float musicVolume = 1;
    public float soundVolume = 1;

    public int maxAudios = 16;
    private List<AudioSource> sourcePool;

    public AudioManager()
    {
      //  instance = this;
    }


    void CreateAudioMap()
    {
        audioMap = new Dictionary<string, AudioClip>();

        AudioMap = new Dictionary<string, AudioClip>();

        foreach (AudioClip clip in audioClips)
        {
            AudioMap.Add(clip.name, clip);
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
        
        SetMusicVolume(PlayerPrefs.GetInt("MusicVolume") / 10f);
        SetSoundVolume(PlayerPrefs.GetInt("SoundVolume") / 10f);

    }
    void Awake()
    {
  //      CreateAudioMap();

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
        bool succ = AudioMap.TryGetValue(audioName, out clip);
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

        var startAudioVol = musicVolume;

        float timer = 0f;

        while ((timer += Time.unscaledDeltaTime) < 0.5f)
        {
            
            SetMusicVolume(Mathf.Lerp(startAudioVol,0, timer / 0.5f));
    
            yield return null;
        }

        SetMusicVolume(0);
        
        StopAllMusic();
        
        Play(nextMusicName, isLooping:true);
        timer = 0f;

        while ((timer += Time.unscaledDeltaTime) < 0.4f)
        {
            SetMusicVolume(Mathf.Lerp(0,startAudioVol, timer / 0.4f));
            yield return null;
        }

        SetMusicVolume(startAudioVol);


    }
    
    
    
    public void Play(string audioName, float vol = 1f, bool isLooping = false, Vector3? position = null)
    {
        if (audioName == null || audioName == "")
        {
            return;
        }
        AudioClip clip;
        bool succ = AudioMap.TryGetValue(audioName, out clip);
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
