using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject optionPanel;
    public GameObject upgradePanel;

    [Header("HUD Elements")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI killCountText;
    public Slider bossHealthBar;

    [Header("Sound")]
    public AudioClip clickSound;

    private AudioSource audioSource;
    private GameManager gameManager;
    private Boss currentBossForUI;

    // GameManager가 호출하여 초기화해주는 함수
    public void Initialize(GameManager gm)
    {
        gameManager = gm;
        audioSource = GetComponent<AudioSource>();
        SetupButtonListeners();

        pausePanel.SetActive(false);
        optionPanel.SetActive(false);
        upgradePanel.SetActive(false);
        bossHealthBar.gameObject.SetActive(false);
    }

    void SetupButtonListeners()
    {
        // 각 패널의 버튼들을 찾아 리스너를 연결합니다.
        // 버튼 이름이 다를 경우, "ResumeButton" 부분을 실제 이름으로 바꿔주세요.
        pausePanel.transform.Find("ResumeButton")?.GetComponent<Button>().onClick.AddListener(() => { PlayClickSound(); gameManager.ResumeGame(); });
        pausePanel.transform.Find("RestartButton")?.GetComponent<Button>().onClick.AddListener(() => { PlayClickSound(); gameManager.Restart(); });
        pausePanel.transform.Find("OptionButton")?.GetComponent<Button>().onClick.AddListener(ShowOptionPanel);

        optionPanel.transform.Find("BackButton")?.GetComponent<Button>().onClick.AddListener(HideOptionPanel);

        for (int i = 0; i < 3; i++)
        {
            int index = i;
            upgradePanel.transform.Find($"UpgradeOption{i + 1}Button")?.GetComponent<Button>().onClick.AddListener(() => { PlayClickSound(); gameManager.SelectUpgradeOption(index); });
        }
    }

    private void PlayClickSound()
    {
        if (clickSound != null) audioSource.PlayOneShot(clickSound);
    }

    public void ShowPausePanel()
    {
        PlayClickSound();
        pausePanel.SetActive(true);
        optionPanel.SetActive(false);
    }

    public void HidePausePanel()
    {
        PlayClickSound();
        pausePanel.SetActive(false);
    }

    public void ShowOptionPanel()
    {
        PlayClickSound();
        pausePanel.SetActive(false);
        optionPanel.SetActive(true);
        if (AudioManager.instance != null) AudioManager.instance.RefreshSliderValues();
    }

    public void HideOptionPanel()
    {
        PlayClickSound();
        optionPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ShowUpgradePanel(string[] options)
    {
        upgradePanel.SetActive(true);
        for (int i = 0; i < options.Length; i++)
        {
            // 버튼 자식의 Text(TMP) 컴포넌트를 찾아 텍스트 변경
            upgradePanel.transform.Find($"UpgradeOption{i + 1}Button/Text (TMP)")?.GetComponent<TextMeshProUGUI>().SetText(options[i]);
        }
    }

    public void HideUpgradePanel()
    {
        upgradePanel.SetActive(false);
    }

    public bool IsOptionPanelActive() => optionPanel.activeSelf;
    public bool IsPausePanelActive() => pausePanel.activeSelf;
    public bool IsUpgradePanelActive() => upgradePanel.activeSelf;

    public void UpdateGameHUD(int currentWave, int killsThisWave, int[] killsToNextWave, int bossWave)
    {
        if (waveText) waveText.text = "Wave: " + currentWave;
        if (killCountText)
        {
            if (currentWave >= bossWave) killCountText.text = "BOSS WAVE";
            else
            {
                int targetKills = killsToNextWave[Mathf.Min(currentWave - 1, killsToNextWave.Length - 1)];
                killCountText.text = "Kills: " + killsThisWave + " / " + targetKills;
            }
        }
    }

    public void ShowBossHealthBar(Boss boss)
    {
        currentBossForUI = boss;
        if (bossHealthBar)
        {
            bossHealthBar.gameObject.SetActive(true);
            bossHealthBar.maxValue = currentBossForUI.maxHealth;
            UpdateBossHealthBar();
        }
    }

    public void UpdateBossHealthBar()
    {
        if (currentBossForUI && bossHealthBar) bossHealthBar.value = currentBossForUI.health;
    }

    public void HideBossHealthBar()
    {
        if (bossHealthBar) bossHealthBar.gameObject.SetActive(false);
        currentBossForUI = null;
    }
}