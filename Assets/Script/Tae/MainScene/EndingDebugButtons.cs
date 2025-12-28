using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingDebugButtons : MonoBehaviour
{
    // 버튼을 눌렀을 때 실행될 함수들
    public void ShowEnding(int endingNum)
    {
        if (GameManager.Instance != null)
        {
            // 1. GameManager의 EndingNum을 강제로 설정합니다.
            GameManager.Instance.EndingNum = endingNum;

            // 2. 엔딩 씬으로 이동합니다.
            // 씬 이름이 "Ending"인지 확인하세요.
            SceneManager.LoadScene("Ending");
        }
        else
        {
            Debug.LogError("GameManager 인스턴스를 찾을 수 없습니다!");
        }
    }

    

    // 인스펙터 버튼 연결용 함수들
    public void OnClickBadEnding() => ShowEnding(0);    // 백수 엔딩
    public void OnClickLoveEnding() => ShowEnding(1);   // 연애 엔딩
    public void OnClickSingerEnding() => ShowEnding(2); // 가수 엔딩
    public void OnClickPerfectEnding() => ShowEnding(3); // 진엔딩
}   