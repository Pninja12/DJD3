using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioClip Footstep;  // Som de passo
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Chame esta função sempre que quiser tocar um passo
    public void PlayFootstep()
    {
        if (Footstep != null && audioSource != null)
        {
            audioSource.PlayOneShot(Footstep);
        }
    }
}