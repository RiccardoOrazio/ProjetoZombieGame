using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Music Settings")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)] public float musicVolume = 0.3f;

    [Header("Player Sounds")]
    public AudioClip[] footsteps;
    public AudioClip playerHurt;
    public AudioClip playerDeath;
    public AudioClip flashlightClick;

    [Header("Weapon Sounds")]
    public AudioClip gunshot;
    public AudioClip dryFire;
    public AudioClip reload;
    public AudioClip bulletImpact;
    public AudioClip weaponAim;

    [Header("Enemy Sounds")]
    public AudioClip zombieAggro;
    public AudioClip zombieAttack;
    public AudioClip zombieHurt;
    public AudioClip zombieDeath;

    [Header("World Sounds")]
    public AudioClip ammoPickup;
    public AudioClip fireLoop;

    private AudioSource musicSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeMusic();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeMusic()
    {
        musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.clip = backgroundMusic;
        musicSource.volume = musicVolume;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.playOnAwake = false;

        if (backgroundMusic != null)
        {
            musicSource.Play();
        }
    }

    public void PlayRandomFootstep(AudioSource source)
    {
        if (source == null) return;

        if (footsteps.Length > 0)
        {
            source.pitch = Random.Range(0.9f, 1.1f);
            source.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
        }
    }

    public void PlaySound(AudioSource source, AudioClip clip, float volume = 1f, float pitchVariance = 0f)
    {
        if (source == null) return;

        if (clip != null)
        {
            if (pitchVariance > 0)
            {
                source.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
            }
            source.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning($"AudioManager: Tentou tocar um som, mas o AudioClip está VAZIO (Null).");
        }
    }

    public void PlayAtPoint(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }
        else
        {
            Debug.LogWarning($"AudioManager: Tentou tocar PlayAtPoint, mas o AudioClip está VAZIO (Null).");
        }
    }
}