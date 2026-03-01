using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources audio")]
    [SerializeField] private AudioSource sourceMusique;
    [SerializeField] private AudioSource sourceSFX;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// Lance une musique en boucle. Remplace la musique en cours si besoin.
    public void JouerMusique(AudioClip clip)
    {
        if (sourceMusique.clip == clip && sourceMusique.isPlaying) return;

        sourceMusique.clip = clip;
        sourceMusique.loop = true;
        sourceMusique.Play();
    }

    /// Arrête la musique en cours.
    public void ArreterMusique()
    {
        sourceMusique.Stop();
        sourceMusique.clip = null;
    }

    /// Joue un effet sonore court (one-shot).
    public void JouerSFX(AudioClip clip)
    {
        if (clip == null) return;
        sourceSFX.PlayOneShot(clip);
    }
}
