using UnityEngine;

public class Audio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void PlayAudio()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }
}
