using UnityEngine;

public class Morto : MonoBehaviour
{
    // Este campo vai aparecer no Inspector
    public AudioClip DeathSound;

    private AudioSource audioSource;

    void Start()
    {
        // Obtém o AudioSource ligado ao GameObject
        audioSource = GetComponent<AudioSource>();
    }

    // Esta função pode ser chamada quando o personagem é atingido
    public void Death()
    {
        if (DeathSound != null && audioSource != null)
        {
            // Toca o som apenas uma vez
            audioSource.PlayOneShot(DeathSound);
        }
    }
}