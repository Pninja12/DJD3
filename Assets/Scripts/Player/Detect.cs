using UnityEngine;

public class Detected : MonoBehaviour
{
    // Este campo vai aparecer no Inspector
    public AudioClip DetectSound;

    private AudioSource audioSource;

    void Start()
    {
        // Obtém o AudioSource ligado ao GameObject
        audioSource = GetComponent<AudioSource>();
    }

    // Esta função pode ser chamada quando o personagem é detectado
    public void Detect()
    {
        if (DetectSound != null && audioSource != null)
        {
            // Toca o som apenas uma vez
            audioSource.PlayOneShot(DetectSound);
        }
    }
}