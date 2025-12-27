using UnityEngine;
using UnityEngine.UI;

public class ArrowGameManager : MonoBehaviour
{
    public static ArrowGameManager Instance;

    [Header("UI 연결")]
    public Slider scoreSlider;

    [Header("설정")]
    public float maxScore = 100f;
    public float scorePerStage = 10f;

    private float currentScore = 0f;

    // 추가: ArrowManager나 다른 곳에서 점수가 다 찼는지 확인할 수 있는 속성
    public bool isGoalReached => currentScore >= maxScore;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        currentScore = 0f;
        if (scoreSlider != null)
        {
            scoreSlider.maxValue = maxScore;
            scoreSlider.value = 0f;
        }
    }

    public void AddScore()
    {
        currentScore += scorePerStage;
        currentScore = Mathf.Clamp(currentScore, 0, maxScore);

        if (scoreSlider != null)
        {
            scoreSlider.value = currentScore;
            Debug.Log($"점수 증가! 현재 점수: {currentScore}");
        }

        if (currentScore >= maxScore)
        {
            GameWin();
        }
    }

    void GameWin()
    {
        Debug.Log("축하합니다! 목표 점수에 도달했습니다.");

        // 핵심 추가: 플레이어를 찾아 "승리(성공)" 신호를 보냅니다.
        // 이렇게 하면 타이머가 나중에 끝나더라도 실패 모션이 나오지 않습니다.
        Arrow_PlayerController player = Object.FindAnyObjectByType<Arrow_PlayerController>();
        if (player != null)
        {
            player.OnGameClear();
        }

        // 여기에 승리 팝업창 띄우기 등의 로직을 추가하세요.
    }
}