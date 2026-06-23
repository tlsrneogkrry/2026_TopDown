using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("오디오 소스")]
    private AudioSource audioSource;

    [Header("효과음 오디오 클립 등록")]
    // 유니티 인스펙터 창에서 보유하신 .mp3 나 .wav 파일을 여기에 드래그해서 넣을 것입니다.
    public AudioClip hitSound;       // 몬스터 타격음
    public AudioClip expGetSound;   // 경험치 보석 획득음
    public AudioClip levelUpSound;  // 레벨업 소리

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 사운드가 끊기지 않게 보존

            // 사운드를 재생할 플레이어(AudioSource)를 자동으로 붙여줍니다.
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ⭐ 어디서나 SoundManager.instance.PlaySound(SoundManager.instance.hitSound); 로 호출 가능!
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && audioSource != null)
        {
            // PlayOneShot은 소리가 겹치더라도 끊기지 않고 중첩되어 이쁘게 출력됩니다. (뱀서 필수)
            audioSource.PlayOneShot(clip, volume);
        }
    }
}