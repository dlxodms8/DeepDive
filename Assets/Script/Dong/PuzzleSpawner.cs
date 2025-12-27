using UnityEngine;
using System.Collections.Generic;

public class PuzzleSpawner : MonoBehaviour
{
    [Header("퍼즐 조각 리스트 (9개를 각각 넣어주세요)")]
    public GameObject[] puzzlePrefabs;

    [Header("배치 설정")]
    public float spacingX = 5.0f; // 가로 간격
    public float spacingY = 3.0f; // 세로 간격



    void Start()
    {
        // 프리팹이 정확히 9개인지 확인
        if (puzzlePrefabs.Length != 9)
        {
            Debug.LogWarning("조각 프리팹을 9개 채워주세요! 현재: " + puzzlePrefabs.Length);
        }

        SpawnPuzzles();
    }

    void SpawnPuzzles()
    {
        // 1. 3x3 격자 좌표 미리 생성 (총 9칸)
        List<Vector3> gridPositions = new List<Vector3>();
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                // x 간격과 y 간격을 다르게 적용
                Vector3 pos = new Vector3((x - 1) * spacingX, (y - 1) * spacingY, 0);
                gridPositions.Add(pos);
            }
        }

        // 2. 위치 리스트만 무작위로 섞기 (좌표만 섞음)
        for (int i = 0; i < gridPositions.Count; i++)
        {
            Vector3 temp = gridPositions[i];
            int randomIndex = Random.Range(i, gridPositions.Count);
            gridPositions[i] = gridPositions[randomIndex];
            gridPositions[randomIndex] = temp;
        }

        // 3. 9개의 프리팹을 섞인 9개의 위치에 하나씩 매칭하여 생성
        // i번째 조각은 i번째로 섞인 좌표에 생성되므로 절대 중복되지 않습니다.
        for (int i = 0; i < puzzlePrefabs.Length; i++)
        {
            if (puzzlePrefabs[i] != null)
            {
                // 위치에 아주 미세한 랜덤 오차를 줘서 더 자연스럽게 배치 (선택사항)
                Vector3 finalPos = gridPositions[i] + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);

                Instantiate(puzzlePrefabs[i], finalPos, Quaternion.identity);
            }
        }
    }



}