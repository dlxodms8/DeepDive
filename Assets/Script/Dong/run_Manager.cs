using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.PointerEventData;
using UnityEngine.UI;
public class run_Manager : MonoBehaviour
{
    public static run_Manager Instance;

    [Header("타임바")]
    public run_TimeBar timeBar;

    [Header("경고 스프라이트")]
    public GameObject sprite1;
    public GameObject sprite2;

    [Header("스폰 좌표 (직접 입력)")]
    public Vector3 spawnPos1;
    public Vector3 spawnPos2;

    [Header("이동 속도")]
    public float moveSpeed = 2f;

    [Header("게임 시간")]
    public float gameTime = 60f;

    private bool isShown = false;
    private bool isGameStopped = false;

    //결과창
    public Image Dategauge;
    public GameObject backGroundPanel;
    public float maxGauge = 100;
    public Text GaugeText;
    private bool InputButton = true;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        sprite1.SetActive(false);
        sprite2.SetActive(false);
    }

    void Update()
    {
            
        if (isGameStopped) return;


        // 🔹 게임 시간 감소
        gameTime -= Time.deltaTime;
        if (gameTime <= 0f)
        {
            gameTime = 0f;
            GameStop();
            return;
        }

        float timeLeft = timeBar.GetTimeLeft();

        // 🔹 3초 남았을 때 스프라이트 등장
        if (timeLeft <= 9f && !isShown)          //몇초뒤 생성중지
        {
            sprite1.transform.position = spawnPos1;
            sprite2.transform.position = spawnPos2;

            sprite1.SetActive(true);
            sprite2.SetActive(true);

            isShown = true;
        }

        // 🔹 등장 후 왼쪽 이동
        if (isShown)
        {
            Vector3 move = Vector3.left * moveSpeed * Time.deltaTime;
            sprite1.transform.Translate(move);
            sprite2.transform.Translate(move);
        }
    }

    public bool IsGameStopped()
    {
        return isGameStopped;
    }

    // 🔹 장애물 생성 가능 여부 (4초 이하 ❌)
    public bool CanSpawnObstacle()
    {
        return !isGameStopped && gameTime > 4f;
    }

    // 🔹 sprite1에 Collider2D + IsTrigger 필요
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isGameStopped) return;

        if (collision.CompareTag("Player"))
        {
            GameStop();
        }
    }

    public void GameStop() //클리어
    {
        if (isGameStopped) return;
        isGameStopped = true;

        Result();
      

        // 1. 시간 흐름 완전 정지 (배경, 이동, 애니메이션 모두 멈춤)
        Time.timeScale = 0f;

        // 2. 기존 장애물들 비활성화
        Obstacle[] obstacles = Object.FindObjectsByType<Obstacle>(FindObjectsSortMode.None);
        foreach (Obstacle obs in obstacles)
        {
            obs.gameObject.SetActive(false);
        }

        // 3. 플레이어 상태 변경
        Run_PlayerController player = Object.FindAnyObjectByType<Run_PlayerController>();
        if (player != null) player.OnTimeUp();


    }

    public void Result()
    {
        backGroundPanel.SetActive(true);
        GameManager.Instance.AddGauge("Date", 10);
        Dategauge.fillAmount = GameManager.Instance.currentDateGauge / maxGauge;
        GaugeText.text = GameManager.Instance.currentDateGauge + " / " + maxGauge;
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
