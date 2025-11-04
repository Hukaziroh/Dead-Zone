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

    // ▼▼▼ 기본 창 모드 해상도를 설정합니다 ▼▼▼
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
        LoadScreenSettings(); // ▼▼▼ 함수 이름 변경 (LoadAndApplyScreenSettings -> LoadScreenSettings)
    }

    public void RefreshOptionPanelUI()
    {
        GameObject optionPanel = GameObject.Find("OptionPanel");
        if (optionPanel == null) return;

        // ... (슬라이더 관련 코드는 기존과 동일) ...
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
        ApplySavedVolumeToSliders();

        // --- (토글 관련 코드는 기존과 동일) ---
        fullscreenToggle = optionPanel.transform.Find("FullscreenToggle")?.GetComponent<Toggle>();
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveAllListeners();
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
            fullscreenToggle.isOn = Screen.fullScreenMode != FullScreenMode.Windowed;
        }
    }

    // --- (볼륨 함수들은 기존과 동일) ---
    public void SetMasterVolume(float volume) { /* ... */ }
    public void SetBGMVolume(float volume) { /* ... */ }
    public void SetSFXVolume(float volume) { /* ... */ }
    private void ApplySavedVolumeToSliders() { /* ... */ }
    private void ApplySavedVolumeToMixer() { /* ... */ }


    // --- ▼▼▼ 화면 설정 함수들 수정 ▼▼▼ ---

    private void LoadScreenSettings()
    {
        bool isFullscreen = PlayerPrefs.GetInt("FullscreenPreference", 1) == 1;

        if (isFullscreen)
        {
            // 전체 화면 (테두리 없는 창 모드)으로 설정
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            // 지정된 기본 해상도의 창 모드로 설정
            Screen.SetResolution(defaultWindowWidth, defaultWindowHeight, FullScreenMode.Windowed);
        }
    }

    // 토글 클릭 시 호출될 함수
    public void SetFullscreen(bool isFullscreen)
    {
        if (isFullscreen)
        {
            // 전체 화면 (테두리 없는 창 모드)으로 전환
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            // 지정된 기본 해상도의 창 모드로 전환
            Screen.SetResolution(defaultWindowWidth, defaultWindowHeight, FullScreenMode.Windowed);
        }

        PlayerPrefs.SetInt("FullscreenPreference", isFullscreen ? 1 : 0);
    }
}