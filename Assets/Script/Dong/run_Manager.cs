using UnityEngine;

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

    public void GameStop()
    {
        if (isGameStopped) return;
        isGameStopped = true;

      

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
}
