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
        Application.runInBackground = true;
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
    public string mainSceneName = "MainScene";

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
    

    private bool isNewDayStart = false;

    public PopupUi uiPrefab;
    private PopupUi uiInstance;

    [Header("엔딩 씬 이름 설정")]
    public string perfectEndingScene = "Ending_Perfect"; // 모두 성공
    public string loveEndingScene = "Ending_Love";       // 연애만 성공
    public string singerEndingScene = "Ending_Singer";   // 가수만 성공
    public string badEndingScene = "Ending_Bad";         // 백수 (모두 실패)

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
            if (D_Day == 0) dayText.text = "D-Day";
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
    //돌발 이벤트
    void CheckAndTriggerEvent(int action)
    {
        // 1. 오늘 이 행동을 몇 번 했는지 카운트
        int repeatCount = 0;
        foreach (int history in dailyActionHistory)
        {
            if (history == action) repeatCount++;
        }

        // 2. 기록에 추가
        dailyActionHistory.Add(action);

        // 3. 확률 설정 (0회:0%, 1회:15%, 2회이상:50%)
        float failChance = 0f;
        if (repeatCount == 1) failChance = 15f;
        else if (repeatCount >= 2) failChance = 50f;

        // 4. 주사위 굴리기
        float dice = Random.Range(0f, 100f);

        if (dice < failChance)
        {
            // 꽝! 부정 이벤트 발생
            TriggerNegativeEvent(action);
        }
    }

    // 실제 페널티 예약 함수
    void TriggerNegativeEvent(int action)
    {
        // 75% 가벼운 페널티 / 25% 무거운 페널티
        // Random.Range(0, 4)는 0, 1, 2, 3 중 하나. 2가 나올 확률은 25%
        bool isSevere = (Random.Range(0, 4) == 2);

        switch (action)
        {
            case 3: // 기타 연습
                if (!isSevere)
                {
                    Debug.Log("이벤트 예약: 손가락 통증");
                    nextGuitarPain = true;
                }
                else
                {
                    Debug.Log("이벤트 예약: 기타줄 파손");
                    nextGuitarBroken = true;
                }
                break;

            case 2: // 작곡
                if (!isSevere)
                {
                    Debug.Log("이벤트 예약: 아이디어 고갈");
                    nextIdeaDrought = true;
                }
                else
                {
                    Debug.Log("이벤트 예약: 노트북 다운");
                    nextLaptopCrash = true;
                }
                break;

            case 1: // SNS
                if (!isSevere)
                {
                    Debug.Log("이벤트 예약: 악플 폭주");
                    nextMaliciousComment = true;
                }
                else
                {
                    Debug.Log("이벤트 예약: 게시물 삭제");
                    nextPostDeleted = true;
                }
                break;

            case 0: // 데이트
                if (!isSevere)
                {
                    Debug.Log("이벤트 예약: 피로 누적");
                    nextFatigue = true;
                }
                else
                {
                    Debug.Log("이벤트 예약: 말다툼 (-5점)");
                    nextBrawl = true;
                    // 말다툼은 점수 즉시 차감
                    // AddGauge("Date", -5f); 
                }
                break;
        }
    }

    void CheckPositiveEvent()
    {
        // 1. 하루 3번 행동 미만이면 패스
        if (dailyActionHistory.Count < 3) return;

        // 2. 행동이 모두 다른지 확인 (Distinct로 중복 제거 후 개수 확인)
        int uniqueActions = dailyActionHistory.Distinct().Count();

        // 3. 3가지 행동이 모두 다르다면 5% 확률 도전
        if (uniqueActions == 3)
        {
            float dice = Random.Range(0f, 100f);
            if (dice < 5f) // 5% 당첨
            {
                TriggerPositiveEvent();
            }
        }
    }

    void TriggerPositiveEvent()
    {
        int randomEvent = Random.Range(0, 3); // 0, 1, 2 중 하나

        switch (randomEvent)
        {
            case 0: // 연애 총합 상승
                // 다음날 아침 팝업을 위해 예약 변수를 켭니다.
                nextLove = true;
                // 점수도 미리 올려줍니다.
                AddGauge("Date", 10f); 
                break;

            case 1: // 컨디션 상승
                // 다음날 아침 적용을 위해 바로 켜지 않고 예약 변수가 없다면...
                // 여기서는 로직상 isConditionGood을 바로 켜도 되지만, 
                // "다음날 적용" 원칙을 위해 보통 next 변수를 씁니다.
                // 일단 기존 변수대로 isConditionGood을 켜두고, 초기화 로직에서 관리합니다.
                isConditionGood = true;
                break;

            case 2: // 추가 보너스 (행운의 날)
                isLuckyDay = true;
                break;
        }
    }

    void FindAndShowEventUI()
    {
        // 1. 항상 켜져 있는 'Canvas'를 먼저 찾습니다.
        GameObject canvas = GameObject.Find("Canvas");

        if (canvas == null) return; // 캔버스조차 없으면 중단

        // 2. Canvas의 자식들 중에서 이름으로 찾습니다. (꺼져 있어도 찾을 수 있음!)
        // ★★★ Hierarchy에 있는 이름과 토씨 하나 틀리지 않고 똑같아야 합니다! ★★★
        Transform panelTr = canvas.transform.Find("EventBackGroundPanel");

        // (만약 이름을 DailyEventPanel로 하셨다면 위 코드를 "DailyEventPanel"로 고치세요)

        if (panelTr == null) return; // 못 찾았으면 중단

        // 3. 찾은 패널에서 스크립트를 가져옵니다.
        PopupUi popup = panelTr.GetComponent <PopupUi>();

        if (popup == null) return;

        // 우선순위: 행동불가 > 긍정 > 일반 디버프 순으로 하나만 띄움

        // 1. 행동 불가 (치명적)
        if (isGuitarBlocked)
        {
            popup.Show("기타줄 파손!", "오늘 하루\n기타 연습 불가능");
        }
        else if (isCompositionBlocked)
        {
            popup.Show("노트북 다운!", "오늘 하루\n작곡 불가능");
        }
        else if (isSnsBlocked)
        {
            popup.Show("게시물 삭제!", "오늘 하루\nSNS 관리 불가능");
        }
        // 2. 긍정 이벤트
        else if (isLove)
        {
            popup.Show("연애 총합 상승!", "연인 관계\n10 상승");
        }
        else if (isLuckyDay)
        {
            popup.Show("행운의 날!", "다음 행동\n경험치 +2");
        }
        else if (isConditionGood)
        {
            popup.Show("컨디션 상승", "오늘 하루\n경험치 +1");
        }
        // 3. 일반 디버프
        else if (isGuitarPainDay)
        {
            popup.Show("손가락 통증!", "오늘 하루\n연습 경험치\n50% 감소");
        }
        else if (isIdeaDrought)
        {
            popup.Show("아이디어 고갈!", "오늘 하루\n작곡 경험치\n50% 감소");
        }
        else if (isMaliciousComment)
        {
            popup.Show("악플 폭주!", "오늘 하루\nSNS 경험치\n50% 감소");
        }
        else if (isFatigue)
        {
            popup.Show("피로 누적!", "오늘 하루\n데이트 체력 감소");
        }
        else if (isBrawl)
        {
            popup.Show("연인과 말다툼!", "연인 관계\n5 하락");
        }
    }

    void ApplyNextDayEffects()
    {
        // 1. 하루 행동 기록 초기화
        dailyActionHistory.Clear();

        // 2. 예약된(next) 디버프를 오늘(current)로 적용
        // (MoveToMiniGame에서 가져온 코드들)
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
        nextFatigue = false; nextBrawl = false;

        // 4. 긍정 효과 초기화 (주의: isLove 등은 유지해야 할 수도 있음 상황에 따라)
        // 일단 기존 로직대로 초기화하되, 예약된 게 있다면 적용
        // (여기서는 단순화를 위해 false 처리하셨던 기존 코드 유지)
        isLove = false;
        isConditionGood = false;
        isLuckyDay = false;

        // 만약 nextLove 등이 있다면 여기서 적용 로직 추가 가능
        // 예: isLove = nextLove; nextLove = false;
    }

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
                CheckAndTriggerEvent(actionIdx);
                // A. 시간/날짜 데이터 먼저 갱신 (UseTime 로직의 일부만 가져옴)
                D_Time++;
                if (D_Time > 2)
                {
                    D_Time = 0;
                    D_Day--;

                    if (D_Day <= 0)
                    {
                        Debug.Log("엔딩 조건 도달");
                        
                    }

                    // ★★★ [수정] 여기서 디버프를 적용하던 코드를 싹 다 삭제했습니다! ★★★
                    // (isGuitarPainDay = nextGuitarPain... 등등 삭제)
                    // (예약 변수 초기화 nextGuitarPain = false... 등등 삭제)

                    // 단지 "새 날이 되었다"는 깃발만 들어올립니다.
                    isNewDayStart = true;

                    // 긍정 이벤트 추첨 (예약만 함)
                    CheckPositiveEvent();
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

        // 페이드 효과용 패널 켜기
        if (screenFader != null)
        {
            screenFader.gameObject.SetActive(true);

            screenFader.ForceFadeOut(() =>
            {
                // 현재 씬이 '메인 화면'일 때만
                if (scene.name == mainSceneName)
                {
                    // 새 날이 시작되었다면?
                    if (isNewDayStart)
                    {
                        // ★★★ [중요] 여기서 디버프/버프를 실제로 적용합니다! ★★★
                        // 이제서야 적용되므로, 아까 했던 저녁 미니게임은 무사합니다.
                        ApplyNextDayEffects();

                        // 적용된 상태를 바탕으로 팝업을 띄웁니다.
                        FindAndShowEventUI();

                        isNewDayStart = false; // 깃발 내리기
                    }
                }
            });
        }
    }
    #endregion

    public void CheckEnding()
    {
        bool isLoveSuccess = currentDateGauge >= 80;
        int singerSuccessCount = 0;
        if (currentPracticeGauge >= 80) singerSuccessCount++;    // 숙련도
        if (currentCompositionGauge >= 80) singerSuccessCount++; // 곡 완성도
        if (currentSnsGauge >= 80) singerSuccessCount++;         // 인지도

        if (isLoveSuccess && singerSuccessCount == 3)
        {
            Debug.Log("결과: 진엔딩 (일과 사랑 모두 잡다!)");
            
        }
        // [연애 성공, 가수 실패] : 연애 O + (가수 요소가 2개 이하일 때)
        // *위의 if문에서 3개인 경우는 걸러졌으므로, 여기는 자연스럽게 2개 이하가 됩니다.
        else if (isLoveSuccess)
        {
            Debug.Log("결과: 연애 엔딩 (가수는 실패했지만 사랑은 남았다)");
            
        }
        // [연애 실패, 가수 성공] : 연애 X + 가수 요소 2개 이상 성공
        else if (!isLoveSuccess && singerSuccessCount >= 2)
        {
            Debug.Log("결과: 가수 엔딩 (사랑은 잃었지만 탑스타가 되었다)");
           
        }
        // [나머지 모든 경우] : 연애 X + 가수 요소 1개 이하
        else
        {
            Debug.Log("결과: 배드 엔딩 (백수... 이도 저도 아니다)");
            
        }
    }

    public void ENDING()
    {
        SceneManager.LoadScene("Ending");
    }
}

