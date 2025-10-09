// UIManager.cs (사운드 기능 추가 버전)

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// ▼▼▼ 1. AudioSource를 사용하기 위해 RequireComponent 추가 ▼▼▼
[RequireComponent(typeof(AudioSource))]
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject optionPanel;
    public GameObject upgradePanel;

    private Button pauseOptionButton;
    private Button optionBackButton;

    private Button[] upgradeButtons = new Button[3];
    private TextMeshProUGUI[] upgradeButtonTexts = new TextMeshProUGUI[3];

    [Header("Game HUD Elements")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI killCountText;
    public Slider bossHealthBar;

    private Boss currentBossForUI;

    // ▼▼▼ 2. 사운드 재생을 위한 변수 추가 ▼▼▼
    public AudioClip clickSound; // 인스펙터에서 지정할 클릭 사운드
    private AudioSource audioSource;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // ▼▼▼ 3. AudioSource 컴포넌트 초기화 ▼▼▼
        audioSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드될 때 UI를 다시 찾는 로직이 필요하다면 여기에 작성
        // 현재는 Start에서 처리하므로 비워둡니다.
    }

    // ▼▼▼ 4. 버튼 클릭 시 호출될 사운드 재생 함수 생성 ▼▼▼
    private void PlayClickSound()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }


    void Start()
    {
        Canvas mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas != null)
        {
            waveText = mainCanvas.transform.Find("WaveText")?.GetComponent<TextMeshProUGUI>();
            killCountText = mainCanvas.transform.Find("KillCountText")?.GetComponent<TextMeshProUGUI>();
            bossHealthBar = mainCanvas.transform.Find("BossHealthBar")?.GetComponent<Slider>();
            pausePanel = mainCanvas.transform.Find("PausePanel")?.gameObject;
            optionPanel = mainCanvas.transform.Find("OptionPanel")?.gameObject;
            upgradePanel = mainCanvas.transform.Find("UpgradePanel")?.gameObject;
        }
        else
        {
            Debug.LogError("UIManager: 씬에서 Canvas를 찾을 수 없습니다! UI 요소 연결 실패.");
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            Transform optionBtnTransform = pausePanel.transform.Find("OptionButton");
            if (optionBtnTransform != null)
            {
                pauseOptionButton = optionBtnTransform.GetComponent<Button>();
                if (pauseOptionButton != null)
                {
                    pauseOptionButton.onClick.RemoveAllListeners();
                    pauseOptionButton.onClick.AddListener(ShowOptionPanel);
                    // ▼▼▼ 5. 사운드 재생 리스너 추가 ▼▼▼
                    pauseOptionButton.onClick.AddListener(PlayClickSound);
                }
            }

            Button resumeBtn = pausePanel.transform.Find("ResumeButton")?.GetComponent<Button>();
            if (resumeBtn != null && GameManager.instance != null)
            {
                resumeBtn.onClick.RemoveAllListeners();
                resumeBtn.onClick.AddListener(GameManager.instance.ResumeGame);
                // ▼▼▼ 5. 사운드 재생 리스너 추가 ▼▼▼
                resumeBtn.onClick.AddListener(PlayClickSound);
            }
            Button restartBtn = pausePanel.transform.Find("RestartButton")?.GetComponent<Button>();
            if (restartBtn != null && GameManager.instance != null)
            {
                restartBtn.onClick.RemoveAllListeners();
                restartBtn.onClick.AddListener(GameManager.instance.Restart);
                // ▼▼▼ 5. 사운드 재생 리스너 추가 ▼▼▼
                restartBtn.onClick.AddListener(PlayClickSound);
            }
        }

        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
            Transform backBtnTransform = optionPanel.transform.Find("BackButton");
            if (backBtnTransform != null)
            {
                optionBackButton = backBtnTransform.GetComponent<Button>();
                if (optionBackButton != null)
                {
                    optionBackButton.onClick.RemoveAllListeners();
                    optionBackButton.onClick.AddListener(HideOptionPanel);
                    // ▼▼▼ 5. 사운드 재생 리스너 추가 ▼▼▼
                    optionBackButton.onClick.AddListener(PlayClickSound);
                }
            }
        }

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
            for (int i = 0; i < 3; i++)
            {
                Transform buttonTransform = upgradePanel.transform.Find($"UpgradeOption{i + 1}Button");
                if (buttonTransform != null)
                {
                    upgradeButtons[i] = buttonTransform.GetComponent<Button>();
                    upgradeButtonTexts[i] = buttonTransform.Find("Text")?.GetComponent<TextMeshProUGUI>();

                    if (upgradeButtons[i] != null)
                    {
                        int optionIndex = i;
                        upgradeButtons[i].onClick.RemoveAllListeners();
                        if (GameManager.instance != null)
                        {
                            upgradeButtons[i].onClick.AddListener(() => GameManager.instance.SelectUpgradeOption(optionIndex));
                            // ▼▼▼ 5. 사운드 재생 리스너 추가 ▼▼▼
                            upgradeButtons[i].onClick.AddListener(PlayClickSound);
                        }
                    }
                }
            }
        }

        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(false);
        }
    }

    // --- (이하 기존 코드와 동일) ---

    // --- PausePanel 관련 함수 ---
    public void ShowPausePanel()
    {
        audioSource.PlayOneShot(clickSound);
        if (pausePanel != null) pausePanel.SetActive(true);
        if (optionPanel != null) optionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
    }
    public void HidePausePanel()
    {
        audioSource.PlayOneShot(clickSound);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    // --- OptionPanel 관련 함수 ---
    public void ShowOptionPanel()
    {
        audioSource.PlayOneShot(clickSound);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(true);
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (AudioManager.instance != null)
        {
            AudioManager.instance.RefreshSliderValues();
        }
    }
    public void HideOptionPanel()
    {
        audioSource.PlayOneShot(clickSound);
        if (optionPanel != null) optionPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    // --- UpgradePanel 관련 함수 ---
    public void ShowUpgradePanel(string[] options)
    {
        audioSource.PlayOneShot(clickSound);
        if (upgradePanel != null) upgradePanel.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(false);

        for (int i = 0; i < options.Length && i < upgradeButtonTexts.Length; i++)
        {
            if (upgradeButtonTexts[i] != null)
            {
                upgradeButtonTexts[i].text = options[i];
            }
        }
    }

    public void HideUpgradePanel()
    {

        if (upgradePanel != null) upgradePanel.SetActive(false);
    }

    // --- 현재 패널 활성화 상태 확인 함수 ---
    public bool IsOptionPanelActive()
    {
        return optionPanel != null && optionPanel.activeSelf;
    }
    public bool IsPausePanelActive()
    {
        return pausePanel != null && pausePanel.activeSelf;
    }
    public bool IsUpgradePanelActive()
    {
        return upgradePanel != null && upgradePanel.activeSelf;
    }


    // --- HUD 업데이트 함수 ---
    public void UpdateGameHUD(int currentWave, int killsThisWave, int[] killsToNextWave, int bossWave)
    {
        Debug.Log($"UIManager.UpdateGameHUD() 호출됨! Wave: {currentWave}, Kills: {killsThisWave}");

        if (waveText != null)
        {
            waveText.text = "Wave: " + currentWave;
        }

        if (killCountText != null)
        {
            if (currentWave >= bossWave)
            {
                killCountText.text = "BOSS WAVE";
            }
            else
            {
                int targetKills = 0;
                if (currentWave > 0 && currentWave - 1 < killsToNextWave.Length)
                {
                    targetKills = killsToNextWave[currentWave - 1];
                }
                killCountText.text = "Kills: " + killsThisWave + " / " + targetKills;
            }
        }
    }

    // --- Boss Health Bar 관련 함수 ---
    public void ShowBossHealthBar(Boss boss)
    {
        currentBossForUI = boss;
        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(true);
            if (currentBossForUI != null)
            {
                bossHealthBar.maxValue = currentBossForUI.maxHealth;
            }
            UpdateBossHealthBar();
        }
    }

    public void UpdateBossHealthBar()
    {
        if (currentBossForUI != null && bossHealthBar != null)
        {
            bossHealthBar.value = currentBossForUI.health;
        }
    }

    public void HideBossHealthBar()
    {
        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(false);
        }
        currentBossForUI = null;
    }
}