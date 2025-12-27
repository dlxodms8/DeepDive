using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("장애물 프리팹")]
    public GameObject obstaclePrefab1;
    public GameObject obstaclePrefab2;

    [Header("생성 위치 설정 (Unity Inspector에서 조정)")]
    public float spawnX = 9f;          // 생성될 X축 위치
    public float prefab1Y = 3f;        // 첫 번째 장애물(공중)의 Y축 위치
    public float prefab2Y = -2f;       // 두 번째 장애물(바닥)의 Y축 위치

    [Header("소환 간격 설정")]
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

        // 랜덤하게 선택된 인덱스에 따라 Y값만 다르게 설정
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
            // 인스펙터에서 설정한 spawnX와 각자의 Y값을 사용해 생성
            Vector3 spawnPosition = new Vector3(spawnX, finalY, 0f);
            Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        }
    }

    void SetRandomInterval()
    {
        targetInterval = Random.Range(minInterval, maxInterval);
    }
}