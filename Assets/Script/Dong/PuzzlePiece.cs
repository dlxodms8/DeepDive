using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    private bool isDragging;
    private Vector3 offset;
    private bool isLocked = false; // 정답 위치에 고정되었는지 확인하는 변수

    [Header("설정")]
    public Vector3 targetPosition; // Spawner에서 전달받을 정답 좌표
    public float snapDistance = 1.0f; // 자석처럼 붙을 거리 (필요에 따라 조절)

    void OnMouseDown()
    {
        // 1. 이미 정답에 맞춰졌다면 클릭 자체를 무시함
        if (isLocked) return;

        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    void OnMouseDrag()
    {
        // 2. 드래그 중에도 잠겨있지 않을 때만 이동
        if (isDragging && !isLocked)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    void OnMouseUp()
    {
        if (isLocked) return;

        isDragging = false;
        CheckPosition();
    }

    void CheckPosition()
    {
        if (Vector3.Distance(transform.position, targetPosition) < snapDistance)
        {
            transform.position = targetPosition;
            isLocked = true; // 잠금 상태로 변경

            // 시각적 피드백 (선택 사항: 레이어를 바꿔서 다른 조각 아래로 가게 하거나 색상을 바꿈)
            Debug.Log($"{gameObject.name} 조각이 고정되었습니다!");
            PuzzleManager.successPuzzle++;
            Debug.Log(PuzzleManager.successPuzzle);
            // 스크립트를 아예 꺼버리고 싶다면 아래 주석을 해제하세요.
            // this.enabled = false; 
        }
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 10;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}