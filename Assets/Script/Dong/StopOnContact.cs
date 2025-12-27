using UnityEngine;

public class StopOnContact : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 부딪힌 대상이 Player인지 확인
        if (collision.CompareTag("Player"))
        {
            // 2. 매니저의 GameStop 함수를 즉시 호출
            if (run_Manager.Instance != null)
            {
                run_Manager.Instance.GameStop();
            }
        }
    }
}