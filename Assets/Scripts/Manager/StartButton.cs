// StartButton.cs

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// ▼▼▼ 1. AudioSource를 사용하기 위해 RequireComponent 추가 ▼▼▼
[RequireComponent(typeof(AudioSource))]
public class StartButton : MonoBehaviour
{
    // ▼▼▼ 2. 필요한 변수 2개 추가 ▼▼▼
    public AudioClip clickSound; // 인스펙터에서 지정할 클릭 사운드 파일
    private AudioSource audioSource; // 소리를 재생할 컴포넌트

    // 게임 세션 동안 마지막으로 로드한 씬의 이름을 기억할 static 변수
    private static string lastLoadedScene = null;
    public string[] sceneNames = { "autumn", "Spring", "summer", "winter" };

    // ▼▼▼ 3. Awake 함수 추가하여 AudioSource 초기화 ▼▼▼
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnStartButtonClicked()
    {
        // ▼▼▼ 4. 사운드 재생 코드 한 줄 추가 ▼▼▼
        audioSource.PlayOneShot(clickSound);

        // --- (기존 코드 유지) ---
        List<string> availableScenes = new List<string>(sceneNames);
        if (!string.IsNullOrEmpty(lastLoadedScene))
        {
            availableScenes.Remove(lastLoadedScene);
        }
        if (availableScenes.Count == 0)
        {
            availableScenes = new List<string>(sceneNames);
        }
        int randomIndex = Random.Range(0, availableScenes.Count);
        string sceneToLoad = availableScenes[randomIndex];
        lastLoadedScene = sceneToLoad;
        Debug.Log("'" + sceneToLoad + "' 씬을 로드합니다.");
        SceneManager.LoadScene(sceneToLoad);
    }

    public void OnQuitButtonClicked()
    {
        // ▼▼▼ 4. 사운드 재생 코드 한 줄 추가 ▼▼▼
        audioSource.PlayOneShot(clickSound);

        // --- (기존 코드 유지) ---
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnLobbyButtonClicked()
    {
        // ▼▼▼ 4. 사운드 재생 코드 한 줄 추가 ▼▼▼
        audioSource.PlayOneShot(clickSound);

        // --- (기존 코드 유지) ---
        SceneManager.LoadScene("Lobby");
    }

    public void OnButtonClicked()
    {
        // ▼▼▼ 4. 사운드 재생 코드 한 줄 추가 ▼▼▼
        audioSource.PlayOneShot(clickSound);

       
    }
}