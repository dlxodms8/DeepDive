using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.PointerEventData;

public class EndingButtonHandler : MonoBehaviour
{
    public string startSceneName = "StartScene";
    private bool InputButton = true;

    void Start()
    {
        InputButton = true;
    }

    public void RestartGame()
    {
        if (InputButton)
        {
            InputButton = false;
            // GameManager와 그 안의 ScreenFader가 있는지 확인
            if (GameManager.Instance != null && GameManager.Instance.screenFader != null)
            {
                // 1. 페이드 효과 시작 (화면이 어두워짐)
                GameManager.Instance.screenFader.PlayTransition(() =>
                {
                    // 2. 화면이 완전히 깜깜해진 순간 실행될 내용들

                    // 데이터 리셋
                    GameManager.Instance.ResetGameData();

                    // 메인 씬 이름 재설정
                    GameManager.Instance.mainSceneName = "SampleScene";

                    // 씬 이동 (이동한 직후 OnSceneLoaded에 의해 페이드 아웃이 자동 실행됨)
                    SceneManager.LoadScene(startSceneName);
                });
            }
            else
            {
                // 예외 상황: 페이더가 없으면 바로 이동
                if (GameManager.Instance != null) GameManager.Instance.ResetGameData();
                SceneManager.LoadScene(startSceneName);
            }
        }
    }
}
