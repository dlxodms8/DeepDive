using UnityEngine;

public class Obstacle : MonoBehaviour
{

    public float speed = 7f; // 바닥(Ground)의 speed와 똑같이 맞추세요!

    void Update()
    {
        // 왼쪽으로 이동
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // 화면 왼쪽 밖으로 완전히 사라지면 삭제 (X 좌표 -15 정도)
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}