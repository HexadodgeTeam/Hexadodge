using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
   [System.Serializable]
   public class Sound
   {
      public string name;
      public AudioClip clip;
      [Range(0f, 1f)] public float volume;
   }

   [SerializeField] private Sound[] SoundEffects;
   [SerializeField] private int PoolSize = 7;
   
   private List<AudioSource> audioSources = new List<AudioSource>();
   private Dictionary<string, AudioClip> soundsDictionary = new Dictionary<string, AudioClip>();
  private float globalVolume =  1.0f;

  public static SoundManager Instance {get; private set;}

  private void Awake()
  {
     if (Instance == null)
     {
        Instance = this;
         DontDestroyOnLoad(gameObject);
     }
     else
     {
        DestroyImmediate(gameObject);
        return;
     }

     for (int i = 0; i < PoolSize; i++)
     {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        audioSources.Add(source);
     }

     foreach (var sound in SoundEffects)
     {
        if (sound.clip != null)
        {
           soundsDictionary[sound.name] = sound.clip;
        }
     }
  }

  private AudioSource GetAvailableAudioSource()
  {
     foreach (AudioSource source in audioSources)
     {
        if (!source.isPlaying)
        {
           return source;
        }
     }
     return audioSources.Count>0?audioSources[1]:null;
  }

  public void PlaySound(string soundName)
  {
     if (!soundsDictionary.TryGetValue(soundName, out AudioClip clip))
     {
        Debug.LogWarning("Sound " + soundName + " not found");
        return;
     }
     AudioSource source = GetAvailableAudioSource();
     if (source != null)
     {
        float soundVolume = 1f;
        foreach (Sound sound in SoundEffects)
        {
           if (sound.name == soundName)
           {
              soundVolume = sound.volume;
              break;
           }
        }
        float finalVolume = soundVolume * globalVolume;
        source.clip = clip;
        source.volume = finalVolume;
        source.Play();
     }
  }
  public void PlaySoundRandomPitch(string soundName)
  {
     if (!soundsDictionary.TryGetValue(soundName, out AudioClip clip))
     {
        Debug.LogWarning("Sound " + soundName + " not found");
        return;
     }
     AudioSource source = GetAvailableAudioSource();
     if (source != null)
     {
        float soundVolume = 1f;
        foreach (Sound sound in SoundEffects)
        {
           if (sound.name == soundName)
           {
              soundVolume = sound.volume;
              break;
           }
        }
        float finalVolume = soundVolume * globalVolume;
        source.clip = clip;
        source.volume = finalVolume;
        source.pitch = Random.Range(0.9f, 1.5f);
        source.Play();
     }
  }
  
 
}
