using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed = 100;
    private run_TimeBar timeBar; // 타임바 스크립트 참조

    void Start()
    {
        // 씬에서 run_TimeBar 스크립트를 찾아 연결합니다.
        timeBar = Object.FindAnyObjectByType<run_TimeBar>();
    }

    void Update()
    {
        // 1. 타임바를 못 찾았거나, 남은 시간이 0 이하라면 동작 중지
        if (timeBar == null || timeBar.GetTimeLeft() <= 0)
        {
            // 움직임을 멈추고 싶다면 여기서 return
            // 아예 오브젝트를 없애고 싶다면 아래 주석을 해제하세요.
            gameObject.SetActive(false); 
            return;
        }

        // 2. 왼쪽으로 이동
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // 3. 화면 왼쪽 밖으로 완전히 사라지면 삭제
        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}