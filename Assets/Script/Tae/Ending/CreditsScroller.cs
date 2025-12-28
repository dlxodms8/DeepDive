using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsScroller : MonoBehaviour
{
    [Header("연결 설정")]
    public RectTransform contentRect;   // 부모 컨테이너 (CreditsContainer)
    public Image displayImage;         // 자식 이미지 (EndingImage)
    public TextMeshProUGUI endingText; // 자식 텍스트 (EndingText)

    [Header("엔딩 그림들 (4개)")]
    public Sprite[] endingSprites;     // 인스펙터에서 0~3번 순서대로 넣어줘

    [Header("속도 및 종료 설정")]
    public float scrollSpeed = 150f;   // 올라가는 속도
    public float endPositionY = 2000f; // 글자가 다 올라가서 사라질 정도의 높이
    public bool enableSkip = true;

    private bool isEnding = false;
    public GameObject RestartButton;

    void Start()
    {
        // 1. 엔딩 데이터 세팅
        EndingSetting(GameManager.Instance.EndingNum);
        
        // 2. 초기 시작 위치 (화면 밖 아래에서 대기)
        contentRect.anchoredPosition = new Vector2(0, -500f);

        StartCoroutine(StopAndGoRoutine());
    }

    void Update()
    {
        if (isEnding) return;

        // 위로 이동
        float moveStep = scrollSpeed;
        if (enableSkip && Input.anyKey) moveStep *= 5f;

        contentRect.anchoredPosition += Vector2.up * moveStep * Time.deltaTime;

        // 종료 확인
        if (contentRect.anchoredPosition.y > endPositionY)
        {
            isEnding = true;
            EndCredits();
        }
    }

    void EndingSetting(int EndingNum)
    {
        // [그림 설정]
        if (endingSprites != null && endingSprites.Length > EndingNum)
        {
            displayImage.sprite = endingSprites[EndingNum];
        }

        // ★ [강제 위치 조정] 그림과 글자가 겹치지 않게 좌표를 직접 입력
        // 그림은 부모의 정중앙(0, 0)에 둠
        displayImage.rectTransform.anchoredPosition = new Vector2(0, 0f);
        // 글자는 그림보다 800픽셀 아래(-800)에 둠 (겹침 방지 핵심!)
        endingText.rectTransform.anchoredPosition = new Vector2(0, -1000f);

        // [텍스트 내용 설정]
        switch (EndingNum)
        {
            case 0: // 백수
                endingText.text = "…조금 늦어버렸네요.\n사랑도, 무대도,\n아무것도 남지 않았습니다.\n하지만 괜찮아요.\n인생은 언제든\n다시 시작할 수 있으니까요.";
                break;
            case 1: // 연애
                endingText.text = "축하합니다!\n당신은 결국, 가장 소중한\n사람의 마음을 지켜냈습니다.\n무대는 비어 있었지만\n두 사람 사이에는\n음악이 흐르고 있었네요.";
                break;
            case 2: // 가수
                endingText.text = "관객의 함성이 무대를 가득 채웁니다.\n당신은 꿈꾸던 가수가 되었습니다.\n다만,\n객석 한 자리가 비어 있는 것 같네요.";
                break;
            case 3: // 진엔딩
                endingText.text = "환호, 조명, 그리고 당신의 노래.\n그 모든 순간을,\n그 사람을 위해 그리고 가장 사랑하는\n사람과 함께 나누고 있습니다.";
                break;
        }
    }

    IEnumerator StopAndGoRoutine()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.8f);
        Time.timeScale = 1f;
    }

    void EndCredits()
    {
        RestartButton.SetActive(true);
    }
}