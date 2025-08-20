using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    // 씬 이름 배열 (씬 빌드 세팅에 등록된 씬 이름 혹은 인덱스 사용 가능)
     public string[] sceneNames = { "autumn", "Spring", "summer", "winter" };

    // UI 버튼에서 이 함수 연결
    public void OnStartButtonClicked()
    {
        // 랜덤 인덱스 뽑기
        int randomIndex = Random.Range(0, sceneNames.Length);

       
        // 씬 로드
        SceneManager.LoadScene(randomIndex);
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