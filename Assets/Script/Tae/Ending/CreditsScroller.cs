using TMPro;
using System.Collections;
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

    public TextMeshProUGUI Text;

    public int EndNum = 0;

    public GameObject RestartButton;

    void Start()
    {
        StartCoroutine(StopAndGoRoutine());
        //EndingSetting(GameManager.Instance.EndingNum);
        EndingSetting(EndNum);
    }

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

    IEnumerator StopAndGoRoutine()
    {
        // 1. 게임의 모든 움직임을 즉시 정지시킵니다.
        Time.timeScale = 0f;

        // 2. 정지된 상태로 0.8초(원하는 만큼 조절) 대기합니다.
        // Time.timeScale이 0일 때는 Realtime을 써야 대기가 가능합니다.
        yield return new WaitForSecondsRealtime(0.8f);

        // 3. 시간을 다시 1로 돌려놓아야 다음 씬이 정상적으로 작동합니다.
        Time.timeScale = 1f;
    }

    void EndingSetting(int EndingNum)
    {
        switch (EndingNum)
        {
            case 0: //백수 엔딩
                endPositionY = 250f;
                contentRect.anchoredPosition = new Vector2(0, -400f);
                Text.text = "…조금 늦어버렸네요.\r\n사랑도, 무대도, 아무것도 남지 않았습니다.\r\n하지만 괜찮아요.\r\n인생은 언제든 다시 시작할 수 있으니까요.";
                break;
            case 1: //연애 엔딩
                endPositionY = 250f;
                contentRect.anchoredPosition = new Vector2(0, -500f);
                Text.text = "축하합니다!\r\n당신은 결국, 가장 소중한 사람의 마음을 지켜냈습니다.\r\n무대는 비어 있었지만\r\n두 사람 사이에는 음악이 흐르고 있었네요.\r\n사랑하는 그 사람을 위해, 이 노래는 충분했습니다.\r\n";
                break;
            case 2: //가수 엔딩
                endPositionY = 250f;
                contentRect.anchoredPosition = new Vector2(0, -600f);
                Text.text = "관객의 함성이 무대를 가득 채웁니다.\r\n당신은 꿈꾸던 가수가 되었습니다.\r\n다만,\r\n객석 한 자리가 비어 있는 것 같네요.\r\n그래도…\r\n이 무대는 분명, 당신의 것입니다.\r\n하지만 그 자리는,\r\n끝내 채워지지 않을 것입니다.";
                break;
            case 3: //진엔딩
                endPositionY = 250f;
                contentRect.anchoredPosition = new Vector2(0, -570f);
                Text.text = "환호, 조명, 그리고 당신의 노래.\r\n그 모든 순간을,\r\n그 사람을 위해 그리고 가장 사랑하는\r\n사람과 함께 나누고 있습니다.\r\n이보다 완벽한 엔딩이 있을까요?\r\n축하합니다.\r\n당신은 전부를 이뤄냈습니다.";
                break;

        }
    }

    // 크레딧 종료 시 실행될 함수
    void EndCredits()
    {
        switch (GameManager.Instance.EndingNum) 
        {
            case 0: //백수 엔딩
                RestartButton.SetActive(true);
                break;
            case 1: //연애 엔딩
                
                break;
            case 2: //가수 엔딩
                
                break;
            case 3: //진엔딩
               
                break;

        }
    }
}