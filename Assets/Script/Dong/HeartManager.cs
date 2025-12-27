using UnityEngine;
using UnityEngine.UI; // UI를 다루기 위해 필요
using System.Collections.Generic;

public class HealthManager : MonoBehaviour
{
    public GameObject[] hearts; // 인스펙터에서 하트 이미지 5개를 여기에 넣습니다.
    private int life;           // 현재 남은 생명력

    void Start()
    {
        life = hearts.Length; // 시작은 하트 개수만큼 (5개)
    }

    // 하트를 하나 깎는 함수
    public void TakeDamage()
    {
        if (life > 0)
        {
            life--;
            // 가장 오른쪽에 있는 하트부터 비활성화
            hearts[life].SetActive(false);
            

            if (life <= 0)
            {
                GameOver();
            }
        }
    }

    void GameOver()
    {
        Debug.Log("게임 오버!");
        Time.timeScale = 0; // 게임 정지
        // 여기에 재시작 버튼 띄우기 등의 로직 추가
    }
}