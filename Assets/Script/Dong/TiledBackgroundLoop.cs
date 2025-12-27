using UnityEngine;

public class TiledBackgroundLoop : MonoBehaviour
{
    public float speed = 2f;
    private float width;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        //  게임 매니저 인스턴스가 존재하고, 
        //  게임이 정지 상태(시간 초과 OR 플레이어 충돌)라면 즉시 리턴
        if (run_Manager.Instance != null && run_Manager.Instance.IsGameStopped())
        {
            return;
        }

        // 이동 로직
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < startPos.x - width)
        {
            transform.position = startPos;
        }
    }
}