// UIManager.cs (수정본 - UI 요소들을 자동으로 찾아 연결)

using UnityEngine;
using UnityEngine.UI; // Button 컴포넌트 사용 시 필요
using TMPro; // TextMeshProUGUI 사용 시 필요

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject optionPanel;

    // UI Panel 내부에 있는 버튼들을 public으로 노출하거나, Find를 통해 찾을 수 있습니다.
    // 여기서는 PausePanel 내 OptionButton과 OptionPanel 내 BackButton을 찾도록 구현.
    private Button pauseOptionButton; // PausePanel 안에 있는 "Option" 버튼
    private Button optionBackButton;  // OptionPanel 안에 있는 "뒤로가기" 버튼

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
            // DontDestroyOnLoad(gameObject); // 이 스크립트를 Manager 오브젝트에 붙인다면 유지
        }
        else
        {
            Destroy(gameObject);
            return; // 중복 인스턴스 파괴 후 즉시 종료
        }
    }

    void Start()
    {
        // 모든 UI 패널들은 기본적으로 비활성화 상태여야 합니다.
        // UIManager에서 직접 찾아서 연결하고 비활성화합니다.

        // 씬에서 "PausePanel"과 "OptionPanel"을 이름으로 찾습니다.
        // 주의: Find는 비활성화된 오브젝트는 찾지 못하므로, Canvas 자체를 비활성화하지 말고
        // Panel 오브젝트들만 비활성화 상태로 유지해야 합니다.
        // 또는 Canvas가 활성화되어 있는 상태에서 Panel을 찾고, 해당 Panel을 비활성화합니다.

        // 이름으로 찾거나, public 변수로 할당해주는게 가장 확실합니다.
        // 이 예시에서는 public 변수로 할당한다는 전제로 계속 진행합니다.
        // 만약 public 변수로 할당하기 싫다면, GameObject.Find("패널이름")으로 찾아서 할당해야 합니다.

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            // PausePanel 내부에 있는 "Option" 버튼 찾기 (자식 오브젝트 중 "OptionButton" 이름의 버튼을 찾음)
            // 실제 게임 오브젝트 이름에 따라 수정 필요
            Transform optionBtnTransform = pausePanel.transform.Find("OptionButton"); // PausePanel 자식으로 "OptionButton"이 있다면
            if (optionBtnTransform != null)
            {
                pauseOptionButton = optionBtnTransform.GetComponent<Button>();
                if (pauseOptionButton != null)
                {
                    // 기존 OnClick 리스너 제거 후 추가 (중복 방지)
                    pauseOptionButton.onClick.RemoveAllListeners();
                    pauseOptionButton.onClick.AddListener(ShowOptionPanel);
                }
            }
        }

        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
            // OptionPanel 내부에 있는 "뒤로가기" 버튼 찾기
            // 실제 게임 오브젝트 이름에 따라 수정 필요
            Transform backBtnTransform = optionPanel.transform.Find("BackButton"); // OptionPanel 자식으로 "BackButton"이 있다면
            if (backBtnTransform != null)
            {
                optionBackButton = backBtnTransform.GetComponent<Button>();
                if (optionBackButton != null)
                {
                    // 기존 OnClick 리스너 제거 후 추가 (중복 방지)
                    optionBackButton.onClick.RemoveAllListeners();
                    optionBackButton.onClick.AddListener(HideOptionPanel);
                }
            }
        }

        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(false);
        }

        // HUD 텍스트와 슬라이더도 마찬가지로 Find 등으로 찾아서 연결할 수 있습니다.
        // 예: waveText = GameObject.Find("WaveText_UI").GetComponent<TextMeshProUGUI>();
        // 하지만 HUD 요소들은 보통 GameManager에 public으로 연결해두고 UIManager가 그 값을 받아오는 방식도 많이 씁니다.
        // 여기서는 UIManager가 직접 관리하므로 public 변수로 할당하는 것이 편리합니다.
    }

    // --- PausePanel 관련 함수 ---
    public void ShowPausePanel()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        if (optionPanel != null) optionPanel.SetActive(false);
    }
    public void HidePausePanel()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(false);
    }

    // --- OptionPanel 관련 함수 ---
    public void ShowOptionPanel()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(true);
    }
    public void HideOptionPanel()
    {
        if (optionPanel != null) optionPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);
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

    // ... (나머지 HUD, Boss Health Bar 관련 함수들) ...
    public void UpdateGameHUD(int currentWave, int killsThisWave, int[] killsToNextWave, int bossWave)
    {
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

    public void ShowBossHealthBar(Boss boss)
    {
        currentBossForUI = boss;
        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(true);
            UpdateBossHealthBar();
        }
    }

    public void UpdateBossHealthBar()
    {
        if (currentBossForUI != null && bossHealthBar != null)
        {
            float healthRatio = currentBossForUI.health / currentBossForUI.maxHealth;
            bossHealthBar.value = healthRatio;
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