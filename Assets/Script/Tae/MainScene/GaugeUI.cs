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
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            AddGauge("Date", fillAmount);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            AddGauge("Sns", fillAmount);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AddGauge("Composition", fillAmount);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AddGauge("Practice", fillAmount);
        }

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

    public void AddGauge(string gaugeName, float amount)
    {
        if(gaugeName == "Date")
        {
            GameManager.Instance.currentDateGauge += amount;
            if (GameManager.Instance.currentDateGauge > maxGauge)
            {
                GameManager.Instance.currentDateGauge = maxGauge;
            }
        }
        else if (gaugeName == "Sns")
        {
            GameManager.Instance.currentSnsGauge += amount;
            if (GameManager.Instance.currentSnsGauge > maxGauge)
            {
                GameManager.Instance.currentSnsGauge = maxGauge;
            }
        }
        else if (gaugeName == "Composition")
        {
            GameManager.Instance.currentCompositionGauge += amount;
            if (GameManager.Instance.currentCompositionGauge > maxGauge)
            {
                GameManager.Instance.currentCompositionGauge = maxGauge;
            }
        }
        else if (gaugeName == "Practice")
        {
            GameManager.Instance.currentPracticeGauge += amount;
            if (GameManager.Instance.currentPracticeGauge > maxGauge)
            {
                GameManager.Instance.currentPracticeGauge = maxGauge;
            }
        }
    }
}

