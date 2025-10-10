using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class StartButton : MonoBehaviour
{
    public AudioClip clickSound;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // 게임 시작 시에는 항상 시간이 흐르도록
        Time.timeScale = 1f;
    }

    public void OnStartButtonClicked()
    {
        audioSource.PlayOneShot(clickSound);
        SceneManager.LoadScene("WeaponSelect");
    }

    public void OnQuitButtonClicked()
    {
        audioSource.PlayOneShot(clickSound);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnLobbyButtonClicked()
    {
        audioSource.PlayOneShot(clickSound);
        SceneManager.LoadScene("Lobby");
    }
}