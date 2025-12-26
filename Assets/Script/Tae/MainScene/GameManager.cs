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
    public string mainSceneName;

    [Header("연출")]
    public ScreenFader screenFader; // 페이드 효과 스크립트
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame

    #region 쉐이더 전용 시간로직
    public void UseTime()
    {
        // 페이드 효과가 연결되어 있다면?
        if (screenFader != null)
        {
            // "화면을 깜깜하게 만든 뒤 -> 실제 시간 변경 로직을 실행해라"
            screenFader.PlayTransition(() =>
            {
                ApplyTimeChange();
            });
        }
        else
        {
            // 페이드 없으면 그냥 바로 변경
            ApplyTimeChange();
        }
    }

    // 실제 데이터 변경 및 씬 이동 판단 로직
    void ApplyTimeChange()
    {
        D_Time++; // 시간 흐름

        bool isDayChanged = false; // 날짜가 바뀌었는지 체크용

        // 저녁(2)이 지났으면 -> 다음날 아침(0)
        if (D_Time > 2)
        {
            D_Time = 0;
            D_Day--;
            isDayChanged = true; // 날짜가 바뀌었다고 표시

            if (D_Day <= 0)
            {
                // Ending(); // 엔딩 로직
                Debug.Log("엔딩 조건 도달");
            }
        }

        UpdateUI(); // 데이터가 바뀌었으니 UI 갱신

        // ★ 핵심 로직: 날짜 변경 여부에 따른 씬 이동 ★
        if (isDayChanged)
        {
            Debug.Log("날짜 변경: 씬 이동 없이 화면 유지");
            // 날짜가 바뀌면(저녁 -> 다음날 아침) 씬을 이동하지 않음
            // 페이드 아웃은 ScreenFader가 알아서 수행함
        }
        else
        {
            Debug.Log("시간 변경: 메인 룸으로 이동");
            // 같은 날 시간만 흐른 경우(아침->점심, 점심->저녁) 메인 씬 재로딩
            SceneManager.LoadScene(mainSceneName);
        }
    }

    public void UpdateUI()
    {
        // 날짜 텍스트 갱신
        if (dayText != null)
        {
            if (D_Day == 0) dayText.text = "D - Day";
            else dayText.text = "D - " + D_Day;
        }

        // 시간 이미지 갱신
        if (timeImage != null && timeIcon.Length > 0)
        {
            // 배열 범위 안전 체크
            if (D_Time < timeIcon.Length)
            {
                timeImage.sprite = timeIcon[D_Time];
            }
        }
    }
    #endregion

    #region 주석
    //public void UseTime()
    //{
    //    D_Time++;
    //    if(D_Time > 2)
    //    {
    //        D_Time = 0;
    //        D_Day--;

    //        if(D_Day <= 0)
    //        {
    //            //Ending();
    //        }
    //    }

    //    UpdateUI();
    //}

    //public void UpdateUI()
    //{
    //    if (dayText != null)
    //    {
    //        if (D_Day == 0)
    //        {
    //            dayText.text = "D - Day";
    //        }
    //        else
    //        {
    //            dayText.text = "D - " + D_Day;
    //        }
    //    }

    //    // 시간 이미지 교체 로직
    //    if (timeImage != null && timeIcon.Length > 0)
    //    {
    //        // 현재 시간(0,1,2)에 맞는 이미지를 배열에서 꺼내서 넣음
    //        timeImage.sprite = timeIcon[D_Time];

    //        // (선택) 이미지 사이즈를 원본 비율로 맞추고 싶으면 아래 주석 해제
    //        // timeDisplayImage.SetNativeSize(); 
    //    }
    //}
    #endregion

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
            //mainSceneName = "GuitarPractice";
            //UseTime();
            if (screenFader != null)
            {
                screenFader.PlayTransition(() =>
                {
                    // 화면이 어두워진 뒤 실행될 내용:

                    // A. 시간/날짜 데이터 먼저 갱신 (UseTime 로직의 일부만 가져옴)
                    D_Time++;
                    if (D_Time > 2)
                    {
                        D_Time = 0;
                        D_Day--;
                        if (D_Day <= 0) Debug.Log("엔딩 조건 도달");
                    }
                    UpdateUI();

                    // B. 날짜가 바뀌었든 안 바뀌었든 '무조건' 씬 이동
                    SceneManager.LoadScene("GuitarPractice");
                });
            }
            else
            {
                // 페이더 없을 때 비상용
                SceneManager.LoadScene("GuitarPractice");
            }
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

    #region 페이드 자동문
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 2. 이 스크립트가 꺼지거나 파괴될 때 "감시"를 중단함 (에러 방지)
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 3. 씬 로딩이 완료되면(도착하면) 자동으로 실행되는 함수
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 0f; // 일단 멈춤

        if (screenFader != null)
        {
            // ★ 죽은 놈(꺼진 놈)도 다시 살려내서 일을 시켜야 합니다.
            screenFader.gameObject.SetActive(true);

            screenFader.ForceFadeOut(() =>
            {
                Time.timeScale = 1f;
            });
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
    #endregion
}

