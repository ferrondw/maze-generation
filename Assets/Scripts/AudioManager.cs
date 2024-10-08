using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] songs; // yet to implement, random multiple songs
    [Space(20)]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip[] clips;

    public static AudioManager Instance;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void UpdateSources(float music, float sfx)
    {
        musicSource.volume = music;
        sfxSource.volume = sfx;
    }

    public void PlayClip(int id, float pitch = 0)
    {
        var randomNegative = Random.value > 0.5 ? 1f : -1f;
        sfxSource.pitch = 1 + pitch * randomNegative;
        sfxSource.PlayOneShot(clips[id]);
    }
}