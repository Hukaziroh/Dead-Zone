using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic; // List를 사용하기 위해 추가!

public class StartButton : MonoBehaviour
{
    // 게임 세션 동안 마지막으로 로드한 씬의 이름을 기억할 static 변수
    private static string lastLoadedScene = null;

    public string[] sceneNames = { "autumn", "Spring", "summer", "winter" };
    public void OnStartButtonClicked()
    {
     
        // 1. 선택 가능한 씬 목록을 새로 만듭니다.
        List<string> availableScenes = new List<string>(sceneNames);

        // 2. 만약 이전에 플레이한 씬이 있다면, 목록에서 제외합니다.
        if (!string.IsNullOrEmpty(lastLoadedScene))
        {
            availableScenes.Remove(lastLoadedScene);
        }

        // 3. 제외했더니 선택할 씬이 하나도 없다면 (예: 총 씬이 1개일 경우),
        //    어쩔 수 없이 다시 전체 목록에서 선택하도록 합니다.
        if (availableScenes.Count == 0)
        {
            availableScenes = new List<string>(sceneNames);
        }

        // 4. '선택 가능한' 씬 목록 중에서 랜덤으로 하나를 고릅니다.
        int randomIndex = Random.Range(0, availableScenes.Count);
        string sceneToLoad = availableScenes[randomIndex];

        // 5. 다음번을 위해, 방금 고른 씬의 이름을 '마지막으로 플레이한 씬'으로 기억시킵니다.
        lastLoadedScene = sceneToLoad;

        // 6. 선택된 씬을 로드합니다.
        Debug.Log("'" + sceneToLoad + "' 씬을 로드합니다.");
        SceneManager.LoadScene(sceneToLoad);
    }

    public void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void OnLobbyButtonClicked()
    {
        SceneManager.LoadScene("Lobby");
    }
}