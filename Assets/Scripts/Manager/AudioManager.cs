using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer mainMixer;

    // 슬라이더 변수
    private Slider masterSlider;
    private Slider bgmSlider;
    private Slider sfxSlider;
    private Toggle fullscreenToggle;

    public int defaultWindowWidth = 1280;
    public int defaultWindowHeight = 720;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ApplySavedVolumeToMixer();
        LoadScreenSettings();
    }

    public void RefreshOptionPanelUI()
    {
        GameObject optionPanel = GameObject.Find("OptionPanel");
        if (optionPanel == null)
        {
            Debug.LogError("[AudioManager] OptionPanel을 씬에서 찾을 수 없습니다!");
            return;
        }

        // --- 슬라이더 찾기 및 연결 ---
        masterSlider = optionPanel.transform.Find("MasterVolumeSlider")?.GetComponent<Slider>();
        bgmSlider = optionPanel.transform.Find("BGMVolumeSlider")?.GetComponent<Slider>();
        sfxSlider = optionPanel.transform.Find("SFXVolumeSlider")?.GetComponent<Slider>();

        if (masterSlider)
        {
            masterSlider.onValueChanged.RemoveAllListeners();
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        if (bgmSlider)
        {
            bgmSlider.onValueChanged.RemoveAllListeners();
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }
        if (sfxSlider)
        {
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        // 저장된 볼륨 값을 슬라이더에 시각적으로 적용
        ApplySavedVolumeToSliders();

        // --- 토글 찾기 및 연결 ---
        fullscreenToggle = optionPanel.transform.Find("FullscreenToggle")?.GetComponent<Toggle>();
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveAllListeners();
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            fullscreenToggle.isOn = Screen.fullScreenMode != FullScreenMode.Windowed;
        }
    }

    // --- ▼▼▼ 여기에 빠졌던 볼륨 함수들을 모두 채워넣었습니다 ▼▼▼ ---

    public void SetMasterVolume(float volume)
    {
        // 슬라이더 값(0.0001~1)을 데시벨(-80~0)로 변환
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume); // 변경된 값 저장
    }

    public void SetBGMVolume(float volume)
    {
        mainMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    // 저장된 값을 불러와 슬라이더 '위치'에 적용
    private void ApplySavedVolumeToSliders()
    {
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float bgmVol = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (masterSlider) masterSlider.value = masterVol;
        if (bgmSlider) bgmSlider.value = bgmVol;
        if (sfxSlider) sfxSlider.value = sfxVol;
    }

    // 저장된 값을 불러와 실제 오디오 '믹서'에 적용
    private void ApplySavedVolumeToMixer()
    {
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float bgmVol = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        mainMixer.SetFloat("MasterVolume", Mathf.Log10(masterVol) * 20);
        mainMixer.SetFloat("BGMVolume", Mathf.Log10(bgmVol) * 20);
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVol) * 20);
    }

    // --- ▼▼▼ 화면 설정 함수들 (기존과 동일) ▼▼▼ ---

    private void LoadScreenSettings()
    {
        bool isFullscreen = PlayerPrefs.GetInt("FullscreenPreference", 1) == 1;

        if (isFullscreen)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(defaultWindowWidth, defaultWindowHeight, FullScreenMode.Windowed);
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        if (isFullscreen)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(defaultWindowWidth, defaultWindowHeight, FullScreenMode.Windowed);
        }

        PlayerPrefs.SetInt("FullscreenPreference", isFullscreen ? 1 : 0);
    }
}