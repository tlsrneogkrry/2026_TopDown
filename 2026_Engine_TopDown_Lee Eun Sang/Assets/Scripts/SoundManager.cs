using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("오디오 소스 (컴포넌트)")]
    private AudioSource bgmSource;
    private AudioSource sfxSource;

    [Header("오디오 클립 등록")]
    public AudioClip bgmClip;
    public AudioClip attackSound;
    public AudioClip hitSound;
    public AudioClip expGetSound;
    public AudioClip levelUpSound;

    [Header("실시간 볼륨 값 (0.0 ~ 1.0)")]
    public float masterVolume = 1f;
    public float bgmVolume = 0.5f;
    public float sfxVolume = 0.7f;

    private void Awake()
    {
        // ★ 이미 instance가 존재하면 자신(새로 생성된 것)을 즉시 제거
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();

        LoadVolumeSettings();
    }

    private void Start()
    {
        PlayBGM(bgmClip);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;
        bgmSource.clip = clip;
        UpdateBGMVolume();
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        float finalVolume = sfxVolume * masterVolume;
        sfxSource.PlayOneShot(clip, finalVolume);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateBGMVolume();
        SaveVolumeSettings();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        UpdateBGMVolume();
        SaveVolumeSettings();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        SaveVolumeSettings();
    }

    private void UpdateBGMVolume()
    {
        if (bgmSource != null)
            bgmSource.volume = bgmVolume * masterVolume;
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVol", masterVolume);
        PlayerPrefs.SetFloat("BgmVol", bgmVolume);
        PlayerPrefs.SetFloat("SfxVol", sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVol", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BgmVol", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SfxVol", 0.7f);
        UpdateBGMVolume();
    }
}