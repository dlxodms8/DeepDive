using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.PointerEventData;

public class PuzzleManager : MonoBehaviour
{
    public Image Practicegauge;
    public GameObject backGroundPanel;
    public float maxGauge = 100;
    public Text GaugeText;
    private bool InputButton = true;
    public float currentScore = 0f;

    public static int successPuzzle = 0;

    public bool isTimerRunning = true;
    private float totalGameTime = 30f;
    public Text timerText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerRunning)
        {
            totalGameTime -= Time.deltaTime;

            // 시간이 0보다 작아지면 게임 오버
            if (totalGameTime <= 0)
            {
                totalGameTime = 0;
                isTimerRunning = false;
                if (timerText) timerText.text = "0";
                GameOver();
                return;
            }
            timerText.text = "" + (int)totalGameTime;
        }

        if(successPuzzle == 9 && isTimerRunning)
        {
            isTimerRunning = false;
            GameClear();
        }
    }

    void GameClear()
    {
        currentScore = 10f;
        Result();
    }

    void GameOver()
    {
        currentScore = 0f;
        Result();
    }

    public void Result()
    {
        backGroundPanel.SetActive(true);
        GameManager.Instance.AddGauge("Composition", currentScore);
        Practicegauge.fillAmount = GameManager.Instance.currentCompositionGauge / maxGauge;
        GaugeText.text = GameManager.Instance.currentCompositionGauge + " / " + maxGauge;
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
}
