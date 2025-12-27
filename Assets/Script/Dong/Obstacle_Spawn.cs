using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("장애물 프리팹")]
    public GameObject obstaclePrefab1;
    public GameObject obstaclePrefab2;

    [Header("생성 위치")]
    public float spawnX = 9f;
    public float prefab1Y = 3f;
    public float prefab2Y = -2f;

    [Header("소환 간격")]
    public float minInterval = 1.0f;
    public float maxInterval = 3.0f;

    private float targetInterval;
    private float timer = 0f;

    void Start()
    {
        SetRandomInterval();
    }

    void Update()
    {
        // 1. 매니저가 없으면 동작 안 함
        if (run_Manager.Instance == null) return;

        // 2. 게임이 종료되었거나 정지 상태라면 동작 안 함
        if (run_Manager.Instance.IsGameStopped()) return;

        // 3. ✅ 남은 시간이 13초 이하이면 장애물 생성을 중단
        // run_Manager에 연결된 timeBar에서 실시간 남은 시간을 체크합니다.
        if (run_Manager.Instance.timeBar.GetTimeLeft() <= 9f)
        {
            return;
        }

        // --- 장애물 생성 타이머 로직 ---
        timer += Time.deltaTime;

        if (timer >= targetInterval)
        {
            SpawnRandomObstacle();
            timer = 0f;
            SetRandomInterval();
        }
    }

    void SpawnRandomObstacle()
    {
        int randomIndex = Random.Range(0, 2);
        GameObject selectedPrefab;
        float finalY;

        if (randomIndex == 0)
        {
            selectedPrefab = obstaclePrefab1;
            finalY = prefab1Y;
        }
        else
        {
            selectedPrefab = obstaclePrefab2;
            finalY = prefab2Y;
        }

        if (selectedPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(spawnX, finalY, 0f);
            Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        }
    }

    void SetRandomInterval()
    {
        targetInterval = Random.Range(minInterval, maxInterval);
    }
}