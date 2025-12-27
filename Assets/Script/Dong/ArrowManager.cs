using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    [Header("프리팹 설정")]
    public GameObject[] arrowPrefabs;
    public Transform arrowContainer;

    [Header("배치 설정")]
    public Vector3 startOffset = new Vector3(-3f, 0f, 0f);
    public Vector3 spacingVector = new Vector3(1.5f, 0f, 0f);

    private List<int> arrowSequence = new List<int>();
    private List<GameObject> spawnedArrows = new List<GameObject>();
    private int inputIndex = 0;

    // PlayerController에서 참조할 속성
    public bool CanInput => inputIndex < arrowSequence.Count;

    void Start()
    {
        if (arrowContainer == null) arrowContainer = this.transform;
        CreateNewStage();
    }

    public void CreateNewStage()
    {
        foreach (var arrow in spawnedArrows)
        {
            if (arrow != null) Destroy(arrow);
        }
        spawnedArrows.Clear();
        arrowSequence.Clear();
        inputIndex = 0;

        for (int i = 0; i < 5; i++)
        {
            int rand = Random.Range(0, 4);
            arrowSequence.Add(rand);

            GameObject newArrow = Instantiate(arrowPrefabs[rand]);
            newArrow.transform.SetParent(arrowContainer);
            newArrow.transform.localPosition = startOffset + (spacingVector * i);
            newArrow.transform.localScale = arrowPrefabs[rand].transform.localScale;

            spawnedArrows.Add(newArrow);
        }
    }

    public void CheckAnswer(int inputCode)
    {
        if (arrowSequence[inputIndex] == inputCode)
        {
            SetArrowAlpha(spawnedArrows[inputIndex], 0.3f);
            inputIndex++;

            if (inputIndex >= arrowSequence.Count)
            {
                if (ArrowGameManager.Instance != null)
                {
                    ArrowGameManager.Instance.AddScore();
                }
                Invoke("CreateNewStage", 0.5f);
            }
        }
        else
        {
            // 플레이어를 찾아 오답 연출 실행
            Arrow_PlayerController player = Object.FindAnyObjectByType<Arrow_PlayerController>();
            if (player != null)
            {
                player.OnWrongInput();
            }

            ResetCurrentStage();
        }
    }

    void ResetCurrentStage()
    {
        inputIndex = 0;
        foreach (var arrow in spawnedArrows) SetArrowAlpha(arrow, 1.0f);
    }

    void SetArrowAlpha(GameObject arrow, float alpha)
    {
        if (arrow.TryGetComponent<SpriteRenderer>(out SpriteRenderer sr))
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
}