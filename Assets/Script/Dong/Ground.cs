using UnityEngine;

public class ScrollingGround : MonoBehaviour
{
    [Header("설정")]
    public float speed = 9f;        // 이동 속도
    public float endX = -20f;       // 왼쪽 화면 밖 지점 (사라지는 곳)
    public float startX = 20f;      // 오른쪽 다시 나타날 지점 (생성되는 곳)

    void Update()
    {
        // 1. 매 프레임마다 왼쪽으로 이동
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // 2. 만약 설정한 왼쪽 끝(endX)보다 더 멀리 갔다면?
        if (transform.position.x <= endX)
        {
            // 3. 위치를 오른쪽 끝(startX)으로 옮겨줍니다.
            // y축과 z축은 현재 위치를 유지해서 위아래로 튀지 않게 합니다.
            //transform.position = new Vector3(startX, transform.position.y, transform.position.z);
            transform.position += new Vector3(startX * 2, 0, 0);
        }
    }
}