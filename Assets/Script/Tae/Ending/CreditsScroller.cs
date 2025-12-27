using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필요

public class CreditsScroller : MonoBehaviour
{
    [Header("연결 설정")]
    public RectTransform contentRect; // 움직일 대상 (CreditsContainer)

    [Header("속도 설정")]
    public float scrollSpeed = 100f; // 올라가는 속도 (초당 픽셀)

    [Header("종료 설정")]
    public float endPositionY = 1500f; // 이 높이를 넘어가면 종료 (직접 확인 필요)
    public string nextSceneName = "MainMenuScene"; // 크레딧 종료 후 이동할 씬 이름
    public bool enableSkip = true; // 클릭해서 빨리 감기 가능 여부

    private bool isEnding = false;

    void Update()
    {
        // 종료 처리 중이면 더 이상 실행 안 함
        if (isEnding) return;

        // 1. 위로 이동시키기
        // 현재 위치에 (위쪽 방향 * 속도 * 시간)을 더해줍니다.
        contentRect.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // (선택 기능) 스킵 기능: 아무 키나 누르면 속도 5배
        if (enableSkip && Input.anyKey)
        {
            contentRect.anchoredPosition += Vector2.up * scrollSpeed * 4f * Time.deltaTime;
        }

        // 2. 종료 조건 확인
        // 컨테이너의 현재 Y 위치가 설정한 목표치보다 높아지면 종료
        if (contentRect.anchoredPosition.y > endPositionY)
        {
            EndCredits();
        }
    }

    // 크레딧 종료 시 실행될 함수
    void EndCredits()
    {
        isEnding = true;
        Debug.Log("크레딧 종료! 다음 씬으로 이동합니다.");

        // 여기에 다음 씬으로 이동하는 코드 작성
        // SceneManager.LoadScene(nextSceneName); 

        // 만약 게임 자체를 종료하고 싶다면
        // Application.Quit();
    }
}