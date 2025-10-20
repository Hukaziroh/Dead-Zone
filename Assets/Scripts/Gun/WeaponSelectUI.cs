// WeaponSelectUI.cs

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Audio;

public class WeaponSelectUI : MonoBehaviour
{
    // StartButton.cs에서 가져온 랜덤 맵 로드 로직
    private static string lastLoadedScene = null;
    public string[] sceneNames = { "autumn", "Spring", "summer", "winter" };
    [Header("Sound")]
    public AudioClip clickSound;
    private AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void PlayClickSound()
    {
        if (clickSound != null) audioSource.PlayOneShot(clickSound);
    }
    // 버튼에서 호출될 함수. 인스펙터에서 무기 이름을 직접 지정
    public void OnWeaponSelect(string weaponID)
    {
        PlayClickSound();
        // ▼▼▼ instance를 빼고 클래스 이름으로 직접 접근합니다 ▼▼▼
        GameManager.selectedWeaponID = weaponID;
        Debug.Log($"[WeaponSelect] 무기 선택 완료: {GameManager.selectedWeaponID}");

        LoadRandomGameScene();
    }

    void LoadRandomGameScene()
    {
        List<string> availableScenes = new List<string>(sceneNames);
        if (!string.IsNullOrEmpty(lastLoadedScene))
        {
            availableScenes.Remove(lastLoadedScene);
        }
        if (availableScenes.Count == 0)
        {
            availableScenes.Add(sceneNames[0]);
        }
        int randomIndex = Random.Range(0, availableScenes.Count);
        string sceneToLoad = availableScenes[randomIndex];
        lastLoadedScene = sceneToLoad;
        SceneManager.LoadScene(sceneToLoad);
    }
}