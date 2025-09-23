// UIManager.cs (최종 수정 버전 - 사용자님 코드 기반)

using UnityEngine;
using UnityEngine.UI; // Button 컴포넌트 사용 시 필요
using TMPro; // TextMeshProUGUI 사용 시 필요
using UnityEngine.SceneManagement; // SceneManager.sceneLoaded를 위해 추가

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject optionPanel;
    public GameObject upgradePanel; // ▼▼▼ 새로운 스탯 업그레이드 패널 변수 추가 ▼▼▼

    // UI Panel 내부에 있는 버튼들을 public으로 노출하거나, Find를 통해 찾을 수 있습니다.
    private Button pauseOptionButton; // PausePanel 안에 있는 "Option" 버튼
    private Button optionBackButton;  // OptionPanel 안에 있는 "뒤로가기" 버튼

    // ▼▼▼ UpgradePanel 내부에 있는 버튼들과 텍스트 참조 추가 ▼▼▼
    private Button[] upgradeButtons = new Button[3]; // 3개의 업그레이드 버튼
    private TextMeshProUGUI[] upgradeButtonTexts = new TextMeshProUGUI[3]; // 각 버튼의 텍스트 컴포넌트


    // 게임 HUD 요소들도 Start에서 찾아 연결하도록 변경 가능
    [Header("Game HUD Elements")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI killCountText;
    public Slider bossHealthBar;

    private Boss currentBossForUI;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // 이 스크립트를 Manager 오브젝트에 붙인다면 유지 (기존 코드 주석 유지)
        }
        else
        {
            Destroy(gameObject);
            return; // 중복 인스턴스 파괴 후 즉시 종료
        }

        // ▼▼▼ 씬 로드 이벤트 구독 (필요한 경우) ▼▼▼
        // Start에서 UI 연결 시 씬이 새로 로드되면 UI 참조가 끊길 수 있으므로 Awake/OnEnable에서 SceneLoaded 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy() // 오브젝트 파괴 시 이벤트 구독 해제 (메모리 누수 방지)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때마다 호출될 함수 (UI 요소를 다시 찾아 연결)
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // GameManager에서 이니셜라이즈를 별도로 호출하거나, UIManager가 직접 관리
        // 여기서는 GameManager가 UIManager의 InitializeUIForGameScene()을 호출하도록 설정했으므로
        // 이 부분에서 중복 호출은 피합니다.
        // 다만, UI 요소들이 Hierarchy에 존재하고 public 변수로 할당되어 있다면 OnSceneLoaded에서 다시 연결하는 로직이 필요합니다.
        // 현재 코드는 Start에서 한번만 연결하는 방식이므로, 씬 변경 시 수동 재할당이 필요합니다.
        // 만약 씬이 변경되어도 UI가 유지되게 하려면 DontDestroyOnLoad를 사용하고 UI 요소를 찾아 할당하는 로직을 OnSceneLoaded에 넣어야 합니다.
    }


    void Start()
    {
        // ▼▼▼ HUD 요소들을 Hierarchy에서 찾아 연결 ▼▼▼
        // 이 방식은 UI 요소들이 항상 Hierarchy의 특정 위치/이름으로 존재할 때 유용합니다.
        // Canvas는 씬마다 있을 수 있으므로 FindObjectOfType<Canvas>()로 찾습니다.
        Canvas mainCanvas = FindObjectOfType<Canvas>();
        if (mainCanvas != null)
        {
            // Game HUD Elements
            waveText = mainCanvas.transform.Find("WaveText")?.GetComponent<TextMeshProUGUI>();
            killCountText = mainCanvas.transform.Find("KillCountText")?.GetComponent<TextMeshProUGUI>();
            bossHealthBar = mainCanvas.transform.Find("BossHealthBar")?.GetComponent<Slider>();

            // UI Panels
            pausePanel = mainCanvas.transform.Find("PausePanel")?.gameObject;
            optionPanel = mainCanvas.transform.Find("OptionPanel")?.gameObject;
            upgradePanel = mainCanvas.transform.Find("UpgradePanel")?.gameObject; // ▼▼▼ UpgradePanel 찾기 ▼▼▼
        }
        else
        {
            Debug.LogError("UIManager: 씬에서 Canvas를 찾을 수 없습니다! UI 요소 연결 실패.");
        }


        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            // PausePanel 내부에 있는 "Option" 버튼 찾기
            Transform optionBtnTransform = pausePanel.transform.Find("OptionButton");
            if (optionBtnTransform != null)
            {
                pauseOptionButton = optionBtnTransform.GetComponent<Button>();
                if (pauseOptionButton != null)
                {
                    pauseOptionButton.onClick.RemoveAllListeners();
                    pauseOptionButton.onClick.AddListener(ShowOptionPanel);
                }
            }
            // 다른 PausePanel 버튼들 (Resume, Restart, Lobby 등)도 여기서 연결할 수 있습니다.
            // 예:
            Button resumeBtn = pausePanel.transform.Find("ResumeButton")?.GetComponent<Button>();
            if (resumeBtn != null && GameManager.instance != null)
            {
                resumeBtn.onClick.RemoveAllListeners();
                resumeBtn.onClick.AddListener(GameManager.instance.ResumeGame);
            }
            Button restartBtn = pausePanel.transform.Find("RestartButton")?.GetComponent<Button>();
            if (restartBtn != null && GameManager.instance != null)
            {
                restartBtn.onClick.RemoveAllListeners();
                restartBtn.onClick.AddListener(GameManager.instance.Restart); // GameManager의 Restart 함수 연결
            }
        }

        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
            // OptionPanel 내부에 있는 "뒤로가기" 버튼 찾기
            Transform backBtnTransform = optionPanel.transform.Find("BackButton");
            if (backBtnTransform != null)
            {
                optionBackButton = backBtnTransform.GetComponent<Button>();
                if (optionBackButton != null)
                {
                    optionBackButton.onClick.RemoveAllListeners();
                    optionBackButton.onClick.AddListener(HideOptionPanel);
                }
            }
        }

        // ▼▼▼ UpgradePanel 내부 버튼 및 텍스트 연결 및 이벤트 추가 ▼▼▼
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false); // 초기 비활성화
            for (int i = 0; i < 3; i++)
            {
                // Unity UI 구조에 따라 경로 조정 (예: UpgradePanel/UpgradeOption1Button/Text)
                Transform buttonTransform = upgradePanel.transform.Find($"UpgradeOption{i + 1}Button"); // 버튼 이름 예시: UpgradeOption1Button
                if (buttonTransform != null)
                {
                    upgradeButtons[i] = buttonTransform.GetComponent<Button>();
                    upgradeButtonTexts[i] = buttonTransform.Find("Text")?.GetComponent<TextMeshProUGUI>(); // 버튼 자식에 Text(TMP) 컴포넌트가 있다면

                    if (upgradeButtons[i] != null)
                    {
                        int optionIndex = i; // 클로저 문제 방지
                        upgradeButtons[i].onClick.RemoveAllListeners();
                        // GameManager의 SelectUpgradeOption 함수와 연결
                        if (GameManager.instance != null)
                        {
                            upgradeButtons[i].onClick.AddListener(() => GameManager.instance.SelectUpgradeOption(optionIndex));
                        }
                        else
                        {
                            Debug.LogWarning($"UIManager: GameManager 인스턴스를 찾을 수 없어 UpgradeOption{i + 1}Button 이벤트를 연결할 수 없습니다.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"UIManager: UpgradeOption{i + 1}Button을 찾았으나 Button 컴포넌트가 없습니다.");
                    }
                }
                else
                {
                    Debug.LogWarning($"UIManager: UpgradePanel에서 UpgradeOption{i + 1}Button을 찾을 수 없습니다.");
                }
            }
        }
        else
        {
            Debug.LogError("UIManager: UpgradePanel을 씬에서 찾을 수 없습니다!");
        }

        // HUD 요소 초기 숨김
        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(false);
        }
    }

    // --- PausePanel 관련 함수 ---
    public void ShowPausePanel()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        if (optionPanel != null) optionPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false); // UpgradePanel 숨김
    }
    public void HidePausePanel()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        // optionPanel은 HideOptionPanel에서 알아서 처리하도록 둠
    }

    // --- OptionPanel 관련 함수 ---
    public void ShowOptionPanel()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(true);
        if (upgradePanel != null) upgradePanel.SetActive(false); // UpgradePanel 숨김
    }
    public void HideOptionPanel()
    {
        if (optionPanel != null) optionPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true); // 옵션에서 돌아오면 PausePanel 활성화
    }

    // --- UpgradePanel 관련 함수 ---
    public void ShowUpgradePanel(string[] options)
    {
        if (upgradePanel != null) upgradePanel.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false); // 다른 패널들은 숨김
        if (optionPanel != null) optionPanel.SetActive(false);

        // 버튼 텍스트 업데이트
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
        // 업그레이드 선택 후에는 다른 패널들도 숨김 (게임 재개되므로)
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
    // ▼▼▼ UpgradePanel 활성화 상태 확인 함수 추가 ▼▼▼
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
            // 보스 슬라이더 MaxValue 설정
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
            // float healthRatio = currentBossForUI.health / currentBossForUI.maxHealth; // maxHealth가 0일 경우 문제
            // bossHealthBar.value = healthRatio;
            bossHealthBar.value = currentBossForUI.health; // 슬라이더 value는 보통 현재 체력, max는 최대 체력으로 설정
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