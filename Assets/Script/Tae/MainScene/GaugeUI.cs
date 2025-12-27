using UnityEngine;
using UnityEngine.UI;

public class GaugeUI : MonoBehaviour
{
    [Header("이미지바")]
    public Image Dategauge;
    public Image Snsgauge;
    public Image Compositiongauge;
    public Image Practicegauge;

    //최대값
    public float maxGauge = 100;

    //테스트 용 변수
    public float fillAmount = 10;

    public GameObject backGroundPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
// Update is called once per frame
    void Update()
    {

        if (maxGauge > 0)
        {
            
            Dategauge.fillAmount = GameManager.Instance.currentDateGauge / maxGauge;
            Snsgauge.fillAmount = GameManager.Instance.currentSnsGauge / maxGauge;
            Compositiongauge.fillAmount = GameManager.Instance.currentCompositionGauge / maxGauge;
            Practicegauge.fillAmount = GameManager.Instance.currentPracticeGauge / maxGauge;
        }
    }
    public void ShowGauge()
    {
        backGroundPanel.SetActive(true);
    }

    public void HideGauge()
    {
        backGroundPanel.SetActive(false);
    }
}

