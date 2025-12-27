using UnityEngine;

public class BackGroundMusic : MonoBehaviour
{
    public AudioSource audioSource;
    // 재생할 소리 파일 (AudioClip)
    public AudioClip startSound;

    void Start()
    {
        // 1. 오디오 소스와 클립이 연결되어 있는지 확인
        if (audioSource != null && startSound != null)
        {
            // 2. 소리 파일 할당
            audioSource.clip = startSound;

            // 3. 오디오 재생!
            audioSource.Play();
        }
    }
}
