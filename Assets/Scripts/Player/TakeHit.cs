using UnityEngine;

public class CharacterHit : MonoBehaviour
{
    // Este campo vai aparecer no Inspector
    public AudioClip hitSound;

    private AudioSource audioSource;

    void Start()
    {
        // Obtém o AudioSource ligado ao GameObject
        audioSource = GetComponent<AudioSource>();
    }

    // Esta função pode ser chamada quando o personagem é atingido
    public void TakeHit()
    {
        if (hitSound != null && audioSource != null)
        {
            // Toca o som apenas uma vez
            audioSource.PlayOneShot(hitSound);
        }
    }
}

