using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class ArrowGameManager : MonoBehaviour
{
    public static ArrowGameManager Instance;

    [Header("UI 연결")]
    public Slider scoreSlider;

    [Header("설정")]
    public float maxScore = 15f;
    public float scorePerStage = 3f;
    
    public float currentScore = 0f;
    public bool isGameclear = false;
    public bool isGameOver = false;

    //결과창
    public Image Snsgauge;
    public GameObject backGroundPanel;
    public float maxGauge = 100;
    public Text GaugeText;
    private bool InputButton = true;
    private bool Once = false;

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

    private void Update()
    {
        if (isGameOver && !Once && !isGameclear)
        {
            Once = true;
            Result();
        }
    }

    public void AddScore()
    {
        currentScore += scorePerStage;
        currentScore = Mathf.Clamp(currentScore, 0, maxScore);

        if (scoreSlider != null)
        {
            scoreSlider.value = currentScore;
            
        }

        if (currentScore >= maxScore)
        {
            isGameclear = true;
            //GameWin();
            Result();
        }
    }
    public void Result()
    {
        backGroundPanel.SetActive(true);
        GameManager.Instance.AddGauge("Sns", currentScore);
        Snsgauge.fillAmount = GameManager.Instance.currentSnsGauge / maxGauge;
        GaugeText.text = GameManager.Instance.currentSnsGauge + " / " + maxGauge;
    }

    public void Exit()
    {
        if (InputButton)
        {
            InputButton = false;
            GameManager.Instance.mainSceneName = "SampleScene";
            //SceneManager.LoadScene("SampleScene");

            if (GameManager.Instance != null && GameManager.Instance.screenFader != null)
            {
                // 1. "화면 좀 가려주세요(PlayTransition)"라고 부탁합니다.
                GameManager.Instance.screenFader.PlayTransition(() =>
                {
                    // 2. 이 괄호 안의 내용은 "화면이 완전히 암전된 후"에 실행됩니다.
                    SceneManager.LoadScene("SampleScene");
                });
            }
            else
            {
                // 만약 매니저가 없거나 페이더가 없으면 그냥 바로 이동 (비상용)
                SceneManager.LoadScene("SampleScene");
            }
        }
    }

    //void GameWin()
    //{
    //    Debug.Log("축하합니다! 목표 점수에 도달했습니다.");

    //    // 핵심 추가: 플레이어를 찾아 "승리(성공)" 신호를 보냅니다.
    //    // 이렇게 하면 타이머가 나중에 끝나더라도 실패 모션이 나오지 않습니다.
    //    Arrow_PlayerController player = Object.FindAnyObjectByType<Arrow_PlayerController>();
    //    if (player != null)
    //    {
    //        player.OnGameClear();
    //    }

    //    // 여기에 승리 팝업창 띄우기 등의 로직을 추가하세요.
    //}
}