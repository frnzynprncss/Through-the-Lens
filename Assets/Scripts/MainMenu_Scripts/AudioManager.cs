using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    // This allows us to call AudioManager.instance from ANY other script
    public static AudioManager instance;

    [System.Serializable]
    public class Sound
    {
        public string name;     // Name we use to call the sound (e.g., "ClaraVoice")
        public AudioClip clip;  // The actual audio file
        [Range(0f, 1f)] public float volume = 1f;
        [Range(.1f, 3f)] public float pitch = 1f;
    }

    public Sound[] sounds;
    private AudioSource sfxSource;

    void Awake()
    {
        // Singleton pattern: ensures only one AudioManager exists
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);

        // Add a single AudioSource component to this object
        sfxSource = gameObject.AddComponent<AudioSource>();
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        // PlayOneShot allows multiple sounds to overlap without cutting each other off
        sfxSource.pitch = s.pitch;
        sfxSource.PlayOneShot(s.clip, s.volume);
    }
}