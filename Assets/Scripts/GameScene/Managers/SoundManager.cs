using UnityEngine;

[System.Serializable]
public class AudioClipDict : SerializableDictionary<string, AudioClip>
{

}

public class SoundManager : MonoBehaviour
{
    static private SoundManager instance;

    private AudioSource bgmPlayer;
    private AudioSource sfxPlayer;

    [SerializeField]
    private AudioClipDict audioClips;

    public static SoundManager Instance
    {
        get
        {
            return instance;
        }
    }

    public float BGMVolume
    {
        get
        {
            return bgmPlayer.volume;
        }

        set
        {
            if (value < 0.0f)
            {
                bgmPlayer.volume = 0.0f;
            }
            else if (value > 1.0f)
            {
                bgmPlayer.volume = 1.0f;
            }
            else
            {
                bgmPlayer.volume = value;
            }
        }
    }

    public float SFXVolume
    {
        get
        {
            return sfxPlayer.volume;
        }

        set
        {
            if (value < 0.0f)
            {
                sfxPlayer.volume = 0.0f;
            }
            else if (value > 1.0f)
            {
                sfxPlayer.volume = 1.0f;
            }
            else
            {
                sfxPlayer.volume = value;
            }
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            AudioSource[] audioSources = GetComponents<AudioSource>();

            if (audioSources.Length > 0)
            {
                bgmPlayer = audioSources[0];
                sfxPlayer = audioSources[1];
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(string clipName)
    {
        if (!audioClips.ContainsKey(clipName))
        {
            return;
        }

        if (bgmPlayer.isPlaying)
        {
            bgmPlayer.Stop();
        }

        bgmPlayer.clip = audioClips[clipName];
        bgmPlayer.Play();
    }

    public void PlaySFX(string clipName)
    {
        if (!audioClips.ContainsKey(clipName))
        {
            return;
        }

        sfxPlayer.PlayOneShot(audioClips[clipName]);
    }
}
