// AudioManager.cs (수정 후)

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // SceneManager 사용을 위해 추가

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixer mainMixer;

    // ▼▼▼ 슬라이더 변수들은 그대로 유지합니다 ▼▼▼
    private Slider masterSlider;
    private Slider bgmSlider;
    private Slider sfxSlider;

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

    // ▼▼▼ Start 함수는 이제 아무것도 하지 않습니다. 로직을 새 함수로 옮길 것입니다. ▼▼▼
    // void Start() { } // 기존 Start 함수 내용은 아래 RefreshSliderValues로 이동

    // ▼▼▼ 슬라이더를 찾고 값을 갱신하는 새로운 public 함수 ▼▼▼
    public void RefreshSliderValues()
    {
        // 현재 활성화된 씬에서 슬라이더들을 다시 찾습니다.
        // 옵션 패널의 이름이나 구조가 바뀌면 이 부분을 수정해야 합니다.
        GameObject optionPanel = GameObject.Find("OptionPanel");
        if (optionPanel)
        {
            masterSlider = optionPanel.transform.Find("MasterVolumeSlider")?.GetComponent<Slider>();
            bgmSlider = optionPanel.transform.Find("BGMVolumeSlider")?.GetComponent<Slider>();
            sfxSlider = optionPanel.transform.Find("SFXVolumeSlider")?.GetComponent<Slider>();
        }
        else
        {
            // 옵션 패널을 못찾았으면 아무것도 하지 않음
            return;
        }

        // 리스너를 추가하기 전에 항상 모두 제거하여 중복 등록을 방지합니다.
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

        // PlayerPrefs에서 저장된 볼륨 값을 불러와 슬라이더에 시각적으로 적용합니다.
        LoadAndApplyVolumeSettings();
    }

    // 슬라이더 값에 따라 마스터 볼륨 조절
    public void SetMasterVolume(float volume)
    {
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    // 슬라이더 값에 따라 BGM 볼륨 조절
    public void SetBGMVolume(float volume)
    {
        mainMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }

    // 슬라이더 값에 따라 SFX 볼륨 조절
    public void SetSFXVolume(float volume)
    {
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    // ▼▼▼ 함수 이름을 LoadAndApplyVolumeSettings로 변경하여 역할을 명확히 함 ▼▼▼
    private void LoadAndApplyVolumeSettings()
    {
        // 저장된 값을 불러옵니다. 저장된 값이 없으면 1 (최대 볼륨)을 기본값으로 사용합니다.
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float bgmVol = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // 현재 씬의 슬라이더'들'의 값을 설정합니다. (시각적 업데이트)
        if (masterSlider) masterSlider.value = masterVol;
        if (bgmSlider) bgmSlider.value = bgmVol;
        if (sfxSlider) sfxSlider.value = sfxVol;

        // 오디오 믹서의 실제 볼륨도 설정합니다. (실제 소리 크기 업데이트)
        // (슬라이더가 없는 씬에서도 소리가 유지되도록 하기 위함)
        mainMixer.SetFloat("MasterVolume", Mathf.Log10(masterVol) * 20);
        mainMixer.SetFloat("BGMVolume", Mathf.Log10(bgmVol) * 20);
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVol) * 20);
    }
}