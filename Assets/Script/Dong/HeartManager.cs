using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI를 다루기 위해 필요
using static UnityEngine.EventSystems.PointerEventData;

public class HealthManager : MonoBehaviour
{
    public GameObject[] hearts; // 인스펙터에서 하트 이미지 5개를 여기에 넣습니다.
    private int life;           // 현재 남은 생명력

    //결과창
    public Image Dategauge;
    public GameObject backGroundPanel;
    public float maxGauge = 100;
    public Text GaugeText;
    private bool InputButton = true;

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
        Result();

    }

    public void Result()
    {
        backGroundPanel.SetActive(true);
        GameManager.Instance.AddGauge("Date", 0);
        Dategauge.fillAmount = GameManager.Instance.currentDateGauge / maxGauge;
        GaugeText.text = GameManager.Instance.currentDateGauge + " / " + maxGauge;
    }

    public void Exit()
    {
        if (InputButton)
        {
            InputButton = false;
            GameManager.Instance.mainSceneName = "SampleScene";
            //SceneManager.LoadScene("SampleScene");

            if (GameManager.Instance != null && GameManager.Instance.screenFader != null)
            {
                // 1. "화면 좀 가려주세요(PlayTransition)"라고 부탁합니다.
                GameManager.Instance.screenFader.PlayTransition(() =>
                {
                    // 2. 이 괄호 안의 내용은 "화면이 완전히 암전된 후"에 실행됩니다.
                    SceneManager.LoadScene("SampleScene");
                });
            }
            else
            {
                // 만약 매니저가 없거나 페이더가 없으면 그냥 바로 이동 (비상용)
                SceneManager.LoadScene("SampleScene");
            }
        }
    }
}