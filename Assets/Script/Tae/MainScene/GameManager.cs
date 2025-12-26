using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance = null;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("목표치 목록")]
    public float currentDateGauge = 0f;
    public float currentSnsGauge = 0f;
    public float currentCompositionGauge = 0f;
    public float currentPracticeGauge = 0f;

    [Header("게임 날짜 및 시간")]
    public float D_Day = 15f;
    public int D_Time = 0;

    public Image timeImage;
    public Sprite[] timeIcon;

    public Text dayText;

    public float maxGauge = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame

    public void UseTime()
    {
        D_Time++;
        if(D_Time > 2)
        {
            D_Time = 0;
            D_Day--;

            if(D_Day <= 0)
            {
                //Ending();
            }
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (dayText != null)
        {
            if (D_Day == 0)
            {
                dayText.text = "D - Day";
            }
            else
            {
                dayText.text = "D - " + D_Day;
            }
        }

        // 시간 이미지 교체 로직
        if (timeImage != null && timeIcon.Length > 0)
        {
            // 현재 시간(0,1,2)에 맞는 이미지를 배열에서 꺼내서 넣음
            timeImage.sprite = timeIcon[D_Time];

            // (선택) 이미지 사이즈를 원본 비율로 맞추고 싶으면 아래 주석 해제
            // timeDisplayImage.SetNativeSize(); 
        }
    }

    public void StartAction(int action)
    {
        if (action == 0)
        {
            UseTime();
            AddGauge("Date", 10);
        }
        else if (action == 1)
        {
            UseTime();
            AddGauge("Sns", 10);
        }
        else if(action == 2)
        {
            UseTime();
            AddGauge("Composition", 10);
        }
        else if(action == 3)
        {
            SceneManager.LoadScene("GuitarPractice");
        }

    }

    public void AddGauge(string gaugeName, float amount)
    {
        if(gaugeName == "Date")
        {
            currentDateGauge += amount;
            if (currentDateGauge > maxGauge)
            {
                currentDateGauge = maxGauge;
            }
        }
        else if (gaugeName == "Sns")
        {
            currentSnsGauge += amount;
            if (currentSnsGauge > maxGauge)
            {
                currentSnsGauge = maxGauge;
            }
        }
        else if (gaugeName == "Composition")
        {
            currentCompositionGauge += amount;
            if (currentCompositionGauge > maxGauge)
            {
                currentCompositionGauge = maxGauge;
            }
        }
        else if (gaugeName == "Practice")
        {
            currentPracticeGauge += amount;
            if (currentPracticeGauge > maxGauge)
            {
                currentPracticeGauge = maxGauge;
            }
        }
    }
}
