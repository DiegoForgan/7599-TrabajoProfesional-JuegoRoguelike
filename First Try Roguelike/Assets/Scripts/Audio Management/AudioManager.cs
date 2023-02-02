using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
  public Sound[] sounds;
  public static AudioManager instance;

  private void Awake() {

      if (instance == null) instance = this;
      else {
          Destroy(gameObject);
          return;
      }
      
      DontDestroyOnLoad(gameObject);

      foreach (Sound sound in sounds)
      {
          sound.source = gameObject.AddComponent<AudioSource>();
          sound.source.clip = sound.clip;
          sound.source.volume = sound.volume;
          sound.source.pitch = sound.pitch;
          sound.source.loop = sound.loop;
      }
  }

    public void StopSound(string name)
    {
      Sound s = findSoundClip(name);
      s.source.Stop();
    }

    private void Start() {
        PlaySound("MainMenuTheme");
    }

    private Sound findSoundClip(string name){
      Sound s = Array.Find(sounds, sound => sound.name == name);
      if (s == null) Debug.LogWarning("Sound with name: "+ name +" was not found!");
      return s;
    }

    public void UpdateVolume(float newVolumeLevel) {

      foreach (Sound sound in sounds)
      {
          sound.source.volume = newVolumeLevel / 100;
      }
    }

    public void PlaySound(string name){
      Sound s = findSoundClip(name);
      s.source.Play();  
  }
}
