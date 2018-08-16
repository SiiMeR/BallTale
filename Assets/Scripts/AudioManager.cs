using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    //  public static AudioManager instance;


    public List<AudioClip> audioClips;


    private Dictionary<string, AudioClip> audioMap;

    public int maxAudios = 16;

    public float musicVolume = 1;
    public float soundVolume = 1;
    private List<AudioSource> sourcePool;

    public Dictionary<string, AudioClip> AudioMap
    {
        get
        {
            if (audioMap == null) CreateAudioMap();

            return audioMap;
        }
        set { audioMap = value; }
    }


    private void CreateAudioMap()
    {
        audioMap = new Dictionary<string, AudioClip>();

        AudioMap = new Dictionary<string, AudioClip>();

        foreach (var clip in audioClips) AudioMap.Add(clip.name, clip);

        sourcePool = new List<AudioSource>();
        var parent = new GameObject("Audios");

        for (var i = 0; i < maxAudios; i++)
        {
            var gob = new GameObject("Audio");
            gob.transform.parent = parent.transform;
            var audioSource = gob.AddComponent<AudioSource>();
            sourcePool.Add(audioSource);
        }

        SetMusicVolume(PlayerPrefs.GetInt("MusicVolume") / 10f);
        SetSoundVolume(PlayerPrefs.GetInt("SoundVolume") / 10f);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (sourcePool != null)
            foreach (var source in sourcePool)
                if (source.isPlaying && source.loop)
                    source.volume = volume;
    }

    public void SetSoundVolume(float volume)
    {
        soundVolume = volume;
    }

    public void StopAllMusic()
    {
        if (sourcePool != null)
            foreach (var source in sourcePool)
                if (source.isPlaying && source.loop)
                    source.Stop();
    }

    public void PlayRandom(string[] audioName, float vol = 1f, bool isLooping = false, Vector3? position = null)
    {
        var i = Random.Range(0, audioName.Length);
        Play(audioName[i], vol, isLooping, position);
    }

    public void Stop(string audioName)
    {
        if (string.IsNullOrEmpty(audioName)) return;
        AudioClip clip;
        var succ = AudioMap.TryGetValue(audioName, out clip);
        if (succ)
            foreach (var source in sourcePool)
                if (source.clip == clip)
                {
                    source.Stop();
                    return;
                }
                else
                    Debug.LogWarning("Could not find audio: " + audioName);
    }

    public IEnumerator FadeToNextMusic(string nextMusicName, float time = 2.0f)
    {
        if (string.IsNullOrEmpty(nextMusicName)) yield break;

        var startAudioVol = musicVolume;

        var timer = 0f;

        while ((timer += Time.unscaledDeltaTime) < 0.5f)
        {
            SetMusicVolume(Mathf.Lerp(startAudioVol, 0, timer / 0.5f));

            yield return null;
        }

        SetMusicVolume(0);

        StopAllMusic();

        Play(nextMusicName, isLooping: true);
        timer = 0f;

        while ((timer += Time.unscaledDeltaTime) < 0.4f)
        {
            SetMusicVolume(Mathf.Lerp(0, startAudioVol, timer / 0.4f));
            yield return null;
        }

        SetMusicVolume(startAudioVol);
    }


    public void Play(string audioName, float vol = 1f, bool isLooping = false, Vector3? position = null)
    {
        if (audioName == null || audioName == "") return;
        AudioClip clip;
        var succ = AudioMap.TryGetValue(audioName, out clip);
        if (succ)
        {
            foreach (var source in sourcePool)
                if (!source.isPlaying)
                {
                    source.clip = clip;
                    source.volume = vol * (isLooping ? musicVolume : soundVolume);
                    source.loop = isLooping;
                    source.Play();

                    if (position != null)
                        source.transform.position = (Vector3) position;
                    else
                        source.transform.localPosition = Vector3.zero;

                    break;
                }
        }

        else
        {
            Debug.LogWarning("Could not find audio: " + audioName);
        }
                    
    }

    public void Play(AudioClip clip, float vol = 1f, bool isLooping = false, Vector3? position = null)
    {
        foreach (var source in sourcePool)
            if (!source.isPlaying)
            {
                source.clip = clip;
                source.volume = vol * (isLooping ? musicVolume : soundVolume);
                source.loop = isLooping;
                source.Play();

                if (position != null)
                    source.transform.position = (Vector3) position;
                else
                    source.transform.localPosition = Vector3.zero;

                break;
            }
    }
}