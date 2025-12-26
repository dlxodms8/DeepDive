using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class StartSceneScript : MonoBehaviour
{
    [Header("시작 화면에 있는 임시 페이더 연결")]
    public ScreenFader localFader;

    public void OnStartButtonClick()
    {
        // 1. 임시 페이더에게 "화면 가려!" 명령
        // (이때 ScreenFader 내부에서 자동으로 isTransitioning = true가 됩니다)
        localFader.PlayTransition(() =>
        {
            // 2. 화면이 다 어두워지면 씬 이동
            SceneManager.LoadScene("SampleScene");
        });
    }

}
