using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    // 씬 이름 배열 (씬 빌드 세팅에 등록된 씬 이름 혹은 인덱스 사용 가능)
    public string[] sceneNames = { "autumn", "Spring", "summer", "winter" };

    // UI 버튼의 OnClick() 이벤트에 연결할 함수
    public void OnStartButtonClicked()
    {
        // 씬 이름 배열이 비어있지 않은지 확인 (안전장치)
        if (sceneNames == null || sceneNames.Length == 0)
        {
            Debug.LogError("로드할 씬 이름이 배열에 없습니다!");
            return;
        }

        // 1. 0부터 (배열의 크기 - 1) 사이의 랜덤한 숫자(인덱스)를 뽑습니다.
        int randomIndex = Random.Range(0, sceneNames.Length);

        // 2. 뽑힌 랜덤 인덱스를 사용해 배열에서 씬 이름을 가져옵니다.
        string sceneToLoad = sceneNames[randomIndex];

        // 3. 이름으로 씬을 로드합니다.
        Debug.Log("'" + sceneToLoad + "' 씬을 로드합니다.");
        SceneManager.LoadScene(sceneToLoad);
    }

    public void OnQuitButtonClicked()
    {
        // 에디터에서는 종료 안 되고 로그만 출력
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}