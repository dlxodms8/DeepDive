using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
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

    [Header("이벤트 시스템")]
    // 오늘 한 행동들을 기록하는 리스트 (0:데이트, 1:SNS, 2:작곡, 3:연습)
    public List<int> dailyActionHistory = new List<int>();
    #region 디버프
    [Header("현재(오늘) 적용 중인 디버프")]
    public bool isGuitarPainDay = false;     // 손가락 통증 (연습 경험치 50% 감소)
    public bool isIdeaDrought = false;       // 아이디어 고갈 (작곡 경험치 50% 감소)
    public bool isMaliciousComment = false;  // 악플 폭주 (SNS 경험치 50% 감소)
    public bool isFatigue = false;           // 피로 누적 (데이트 체력 감소 - 미니게임에서 처리 필요)
    public bool isBrawl = false;             // 말다툼 (연애 관계 -5)

    [Header("다음 날 적용될 예약된 디버프")]
    public bool nextGuitarPain = false;      // 손가락 통증
    public bool nextGuitarBroken = false;    // 기타줄 파손 (내일 연습 불가)
    public bool nextIdeaDrought = false;     // 아이디어 고갈
    public bool nextLaptopCrash = false;     // 노트북 다운 (내일 작곡 불가)
    public bool nextMaliciousComment = false;// 악플 폭주
    public bool nextPostDeleted = false;     // 게시물 삭제 (내일 SNS 불가)
    public bool nextFatigue = false;         // 피로 누적
    public bool nextBrawl = false;           // 말다툼
    public bool nextLove = false;            // 연애 총합 상승


    // 행동 금지 플래그 (오늘 적용)
    public bool isGuitarBlocked = false; // 기타줄 파손
    public bool isCompositionBlocked = false; // 노트북 다운
    public bool isSnsBlocked = false; // 게시물 삭제
    #endregion


    [Header("긍정 이벤트 효과 (다음날 적용)")]
    public bool isConditionGood = false; // 컨디션 상승 (모든 경험치 +1)
    public bool isLuckyDay = false;      // 긍정 이벤트 (추가 보너스 +2)
    public bool isLove = false;          // 연애 총합 상승(연애 관계 +10)
    //[Header("일일 알림 팝업 UI")]
    //public GameObject eventPopupPanel; // 팝업창 전체 (패널)
    //public Text eventTitleText;        // 제목 텍스트
    //public Text eventDescText;         // 설명 텍스트
    private bool isNewDayStart = false;

    public PopupUi uiPrefab;
    private PopupUi uiInstance;

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
            else dayText.text = "D-" + D_Day;
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

    #region 돌발이벤트
    /*//돌발 이벤트
    void CheckAndTriggerEvent(int action)
    {
        // 1. 오늘 이 행동을 몇 번 했는지 카운트
        int repeatCount = 0;
        foreach (int history in dailyActionHistory)
        {
            if (history == action) repeatCount++;
        }

        // 2. 기록에 추가 (이번 행동 포함)
        dailyActionHistory.Add(action);

        // 3. 확률 설정 (기획서 기준)
        // 1회차(repeatCount 0) : 0%
        // 2회차(repeatCount 1) : 15%
        // 3회차(repeatCount 2이상) : 50%
        float failChance = 0f;
        if (repeatCount == 1) failChance = 15f;
        else if (repeatCount >= 2) failChance = 50f;

        // 4. 주사위 굴리기 (0 ~ 100)
        float dice = UnityEngine.Random.Range(0f, 100f);

        if (dice < failChance)
        {
            // 꽝! 부정 이벤트 발생
            TriggerNegativeEvent(action);
        }
    }

    // 하루가 끝날 때 긍정 이벤트 발생 여부를 체크하는 함수
    void CheckPositiveEvent()
    {
        // 1. 하루 3번의 행동이 다 찼는지 확인
        if (dailyActionHistory.Count < 3) return;

        // 2. 행동이 모두 다른지 확인 (중복 제거 후 개수 확인)
        // Distinct(): 리스트에서 중복된 요소를 제거하는 기능
        int uniqueActions = dailyActionHistory.Distinct().Count();

        // 3. 만약 3개 행동이 모두 다르다면? (개수가 3개면 모두 다른 것)
        if (uniqueActions == 3)
        {
            // 4. 5% 확률 도전
            float dice = UnityEngine.Random.Range(0f, 100f);
            if (dice < 5f) // 5% 당첨
            {
                TriggerPositiveEvent();
            }
        }
    }

    void TriggerPositiveEvent()
    {
        int randomEvent = UnityEngine.Random.Range(0, 3); // 0, 1, 2 중 랜덤

        switch (randomEvent)
        {
            case 0: // 연애 총합 상승

                AddGauge("Date", 10f); // 즉시 적용

                break;

            case 1: // 컨디션 상승

                isConditionGood = true; // 다음 날 적용 플래그 켬
                break;

            case 2: // 긍정 이벤트 (추가 보너스)

                isLuckyDay = true; // 다음 날 적용 플래그 켬
                break;
        }
    }

    // 실제 페널티를 부여하는 함수
    void TriggerNegativeEvent(int action)
    {
        // 확률 반반으로 가벼운 페널티 / 무거운 페널티 나눔 (임의 설정)
        // 기획서에 "손가락 통증"과 "기타줄 파손" 두 가지가 이며 75:25
        bool isSevere = UnityEngine.Random.Range(0, 4) == 2;

        switch (action)
        {
            case 3: // 기타 연습
                if (!isSevere)
                {
                    Debug.Log("이벤트 발생: 손가락 통증! (내일 경험치 절반)");
                    nextGuitarPain = true;
                }
                else
                {
                    Debug.Log("이벤트 발생: 기타줄 파손! (내일 연습 불가)");
                    nextGuitarBroken = true;
                }
                break;

            case 2: // 작곡
                if (!isSevere)
                {
                    Debug.Log("이벤트 발생: 아이디어 고갈! (내일 경험치 절반)");
                    nextIdeaDrought = true;
                }
                else
                {
                    Debug.Log("이벤트 발생: 노트북 다운! (내일 작곡 불가)");
                    nextLaptopCrash = true;
                }
                break;

            case 1: // SNS
                if (!isSevere)
                {
                    Debug.Log("이벤트 발생: 악플 폭주! (내일 경험치 절반)");
                    nextMaliciousComment = true;
                }
                else
                {
                    Debug.Log("이벤트 발생: 게시물 삭제됨! (내일 SNS 불가)");
                    nextPostDeleted = true;
                }
                break;

            case 0: // 데이트
                if (!isSevere)
                {
                    Debug.Log("이벤트 발생: 피로 누적! (미니게임 체력 감소)");
                    nextFatigue = true;
                }
                else
                {
                    Debug.Log("이벤트 발생: 말다툼! (관계도 -5)");
                    nextBrawl = true;
                    // 말다툼은 즉시 적용
                    AddGauge("Date", -5f);
                }
                break;
        }
    }

    //void ShowDailyPopup(string title, string desc)
    //{
    //    if (eventPopupPanel == null) return;

    //    eventTitleText.text = title;
    //    eventDescText.text = desc;
    //    eventPopupPanel.SetActive(true); // 패널 켜기
    //    Debug.Log("패널켜기작동");
    //}

    //public void closePopup()
    //{
    //    eventPopupPanel.SetActive(false);
    //    Time.timeScale = 1f;
    //}
    //bool CheckAndShowEventUI()
    //{
    //    // 1. 지금 살아있는 UI가 있는지 확인
    //    if (uiInstance == null)
    //    {
    //        // 없으면, 혹시 씬에 숨어있는지 한번 찾아보고
    //        uiInstance = GameObject.Find("EventBackGroundPanel").GetComponent<PopupUi>();

    //        // 그래도 없으면, 프리팹으로 새로 만듭니다! (소환!)
    //        if (uiInstance == null && uiPrefab != null)
    //        {
    //            // 소환!
    //            uiInstance = Instantiate(uiPrefab);

    //            // ★ 중요: 씬 이동해도 사라지지 않게 만듦
    //            DontDestroyOnLoad(uiInstance.gameObject);
    //        }
    //    }

    //    // 2. 만약 프리팹 연결도 안 해서 진짜로 없으면 포기
    //    if (uiInstance == null)
    //    {
    //        Debug.LogWarning("UI 프리팹이 연결되지 않았습니다!");
    //        return false;
    //    }

    //    if (isGuitarPainDay)
    //    {
    //        uiInstance.Show("손가락 통증!", "오늘 하루\n연습 경험치\n50% 감소");
    //        return true;
    //    }
    //    else  if (isGuitarBlocked)
    //    {
    //        uiInstance.Show("기타줄 파손!", "오늘 하루\n기타 연습 불가능");
    //        return true;
    //    }
    //    else if (isIdeaDrought)
    //    {
    //        uiInstance.Show("아이디어 고갈!", "오늘 하루\n작곡 경험치\n50% 감소");
    //        return true;
    //    }
    //    else if (isCompositionBlocked)
    //    {
    //        uiInstance.Show("노트북 다운!", "오늘 하루\n작곡 불가능");
    //        return true;
    //    }
    //    else if (isMaliciousComment)
    //    {
    //        uiInstance.Show("악플 폭주!", "오늘 하루\nSNS 경험치\n50% 감소");
    //        return true;
    //    }
    //    else if (isSnsBlocked)
    //    {
    //        uiInstance.Show("게시물 삭제!", "오늘 하루\nSNS 관리 불가능");
    //        return true;
    //    }
    //    else if (isFatigue)
    //    {
    //        uiInstance.Show("피로 누적!", "오늘 하루\n데이트 체력 감소");
    //        return true;
    //    }
    //    else if (isBrawl)
    //    {
    //        uiInstance.Show("연인과 말다툼!", "연인 관계\n5 하락");
    //        return true;
    //    }
    //    else if (isLove)
    //    {
    //        uiInstance.Show("연애 총합 상승!", "연인 관계\n10 상승");
    //        return true;
    //    }
    //    else if (isConditionGood)
    //    {
    //        uiInstance.Show("컨디션 상승", "오늘 하루\n경험치 +1");
    //        return true;
    //    }
    //    else if (isLove)
    //    {
    //        uiInstance.Show("긍정 이벤트", "다음 행동\n경험치 +2");
    //        return true;
    //    }

    //    return false;
    //}*/

    #endregion

    // 행동
    public void StartAction(int action)
    {
        if (action == 3 && isGuitarBlocked)
        {
            return; // 함수 종료 (아무 일도 안 일어남)
        }
        else if (action == 2 && isCompositionBlocked)
        {
            return;
        }
        else if (action == 1 && isSnsBlocked)
        {
            return;
        }

        switch (action)
        {
            case 0:
                UseTime();
                AddGauge("Date", 10);
                //MoveToMiniGame("Practice");
                break;
            case 1:
                //UseTime();
                //AddGauge("Sns", 10);
                MoveToMiniGame("arrow", action);
                break;
            case 2:
                UseTime();
                AddGauge("Composition", 10);
                //MoveToMiniGame("Practice");
                break;
            case 3:
                MoveToMiniGame("GuitarPractice", action);
                break;
        }
    }

    void MoveToMiniGame(string targetScore, int actionIdx)
    {
        if (screenFader != null)
        {
            screenFader.PlayTransition(() =>
            {
                // 화면이 어두워진 뒤 실행될 내용:
                //CheckAndTriggerEvent(actionIdx);
                // A. 시간/날짜 데이터 먼저 갱신 (UseTime 로직의 일부만 가져옴)
                D_Time++;
                if (D_Time > 2)
                {
                    
                    D_Time = 0;
                    D_Day--;
                    if (D_Day <= 0)
                    {
                        //Ending(); 
                        Debug.Log("엔딩 조건 도달");
                        return;
                    }
                    else
                    {
                        // 1. 하루 행동 기록 초기화
                        dailyActionHistory.Clear();

                        // 2. 예약된 디버프를 오늘자로 적용
                        isGuitarPainDay = nextGuitarPain;
                        isGuitarBlocked = nextGuitarBroken;

                        isIdeaDrought = nextIdeaDrought;
                        isCompositionBlocked = nextLaptopCrash;

                        isMaliciousComment = nextMaliciousComment;
                        isSnsBlocked = nextPostDeleted;

                        isFatigue = nextFatigue;
                        isBrawl = nextBrawl;

                        // 3. 예약 변수들은 다시 초기화 (일회성)
                        nextGuitarPain = false; nextGuitarBroken = false;
                        nextIdeaDrought = false; nextLaptopCrash = false;
                        nextMaliciousComment = false; nextPostDeleted = false;
                        nextFatigue = false;
                        nextBrawl = false;

                        //긍정 효과 초기화
                        isLove = false;
                        isConditionGood = false;
                        isLuckyDay = false;

                        isNewDayStart = true;
                    }
                    //CheckPositiveEvent();

                }
                UpdateUI();
                // B. 날짜가 바뀌었든 안 바뀌었든 '무조건' 씬 이동
                SceneManager.LoadScene(targetScore);
            });
        }
        else
        {
            // 페이더 없을 때 비상용
            SceneManager.LoadScene(targetScore);
        }
        
    }

    // 목표치
    public void AddGauge(string gaugeName, float amount)
    {
        //버프 적용
        if (isConditionGood)
        {
            Debug.Log("컨디션 좋음! 경험치 +1");
            amount += 1f;
        }

        if (isLuckyDay)
        {
            Debug.Log("행운의 날! 추가 보너스 +2");
            amount += 2f;
            isLuckyDay = false;
        }

        // 디버프 적용
        if (gaugeName == "Practice" && isGuitarPainDay)
        {
            amount *= 0.5f;
        }
        else if (gaugeName == "Composition" && isIdeaDrought)
        {
            amount *= 0.5f;
        }
        else if (gaugeName == "Sns" && isMaliciousComment)
        {
            amount *= 0.5f;
        }

        //목표치 계산
        if (gaugeName == "Date")
        {
            currentDateGauge += (int)amount;
            if(currentDateGauge < 0)
            {
                currentDateGauge = 0;
            }
            if (currentDateGauge > maxGauge)
            {
                currentDateGauge = maxGauge;
            }
        }
        else if (gaugeName == "Sns")
        {
            currentSnsGauge += (int)amount;
            if (currentSnsGauge > maxGauge)
            {
                currentSnsGauge = maxGauge;
            }
        }
        else if (gaugeName == "Composition")
        {
            currentCompositionGauge += (int)amount;
            if (currentCompositionGauge > maxGauge)
            {
                currentCompositionGauge = maxGauge;
            }
        }
        else if (gaugeName == "Practice")
        {
            currentPracticeGauge += (int)amount;
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
        Time.timeScale = 0f;

        if (screenFader != null)
        {
            screenFader.gameObject.SetActive(true);
            screenFader.ForceFadeOut(() =>
            {
                if (isNewDayStart)
                {
                    //bool isPopupShown = CheckAndShowEventUI();
                    isNewDayStart = false;
                    Time.timeScale = 1f;
                    //if (!isPopupShown) Time.timeScale = 1f;
                }
                else
                {
                    Time.timeScale = 1f;
                }
            });
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
    #endregion
}

