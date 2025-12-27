using UnityEngine;
using UnityEngine.UI; // UI 사용을 위해 필수

public class arrow_TimeBar : MonoBehaviour
{
    public Slider timerSlider;
    public float maxTime = 60f;
    private float timeLeft;
    private bool isGameOver = false; // 게임 오버 중복 실행 방지

    void Start()
    {
        timeLeft = maxTime;
        timerSlider.maxValue = maxTime;
        timerSlider.value = maxTime;
    }

    void Update()
    {
        if (isGameOver) return; // 이미 게임 오버라면 중단

        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerSlider.value = timeLeft;
        }
        else
        {
            timeLeft = 0;
            TimeOver();
        }
    }

    void TimeOver()
    {
        isGameOver = true;
        Debug.Log("게임 오버!");

        // 1. 씬에서 Arro_PlayerController를 찾아서 OnTimeUp 함수 호출
        Arrow_PlayerController player = Object.FindAnyObjectByType<Arrow_PlayerController>();
        if (player != null)
        {
            player.OnTimeUp();
        }

        // 2. 게임을 바로 멈추고 싶다면 아래 코드를 쓰지만, 
        // 캐릭터가 바뀌는 걸 보여주고 싶다면 Invoke로 약간 뒤에 멈추는 게 좋습니다.
        // Invoke("StopGame", 0.5f);
    }

    void StopGame()
    {
        Time.timeScale = 0; // 필요하다면 여기서 게임 정지
    }

}