using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Drawing.Colors;
using System.Threading;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.Animations;
public class MiniGameGuitar : MonoBehaviour
{

    public Transform buttonContainer; //플레이어 버튼 패널
    public Transform questionContainer; // 문제 패널

    public GameObject questionPrefab; //보여줄 프리팹

    public Sprite[] codeData; // 코드 설정
    private int[] roundquestion = { 3, 4, 5, 6, 8 }; // 라운드 별 맞춰야 하는 개수

    private int currentRound = 0; // 현재 라운드
    private List<GameObject> spawnedImageObjects = new List<GameObject>(); //생성된 오브젝트 관리
    private List<int> questionSequence = new List<int>(); //정답지

    private int inputCodeSum = 0; // 현재 플레이거 눌러야할 정답 위치

    private bool isInputActive = false; // 입력 가능 상태

    public float currentScore = 0f; // 점수

    public Text timerText; // 남은 시간 텍스트
    private float totalGameTime = 30f;  // 전체 제한 시간
    private bool isTimerRunning = false; //정답 보여줄 때 시간 안흐름

    //결과창
    public Image Practicegauge;
    public GameObject backGroundPanel;
    public float maxGauge = 100;
    public Text GaugeText;

    public Text FirstText;
    private float FirstTime = 3.5f;
    private bool FirstRunning = false;
    private bool InputButton = true;

    public Animator animatorPlayer;
    public Animator animatorNPC;

    private bool isRoundClearing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animatorPlayer = GameObject.Find("Player").GetComponent<Animator>();
        animatorNPC = GameObject.Find("Npc").GetComponent<Animator>();
        StartCoroutine(FirstStart());
        InputButton = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerRunning)
        {
            totalGameTime -= Time.deltaTime;

            // 시간이 0보다 작아지면 게임 오버
            if (totalGameTime <= 0)
            {
                totalGameTime = 0;
                isTimerRunning = false;
                isInputActive = false;
                if (timerText) timerText.text = "0";
                GameOver();
                return;
            }
            
        }

        if(FirstRunning)
        {
            FirstTime -= Time.deltaTime;
            if(FirstTime <= 0)
            {
                FirstText.text = "시작!";
            }
            else
            {
                FirstText.text = "" + (int)FirstTime;
            }
        }
        timerText.text = "" + (int)totalGameTime;
    }
    public void ShuffleCode()
    {
        int count = buttonContainer.childCount; //자식의 개수만큼 반복할 변수

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, count); // 0~3 까지 랜덤 변수
            buttonContainer.GetChild(i).SetSiblingIndex(randomIndex); //i번째 버튼을 랜덤한 위치로 이동시킴(Layout Group이 있어 순서만 바꾸면 위치도 바뀜)
        }
    }

    void StartRound()
    {
        isRoundClearing = false;

        StopAllCoroutines();

        // 라운드 끝나면 종료
        if (currentRound >= roundquestion.Length)
        {
            GameClear();
            return;
        }

        // 값 초기화
        inputCodeSum = 0;
        isInputActive = false;
        isTimerRunning = false;
        questionSequence.Clear();
        spawnedImageObjects.Clear();
        questionContainer.gameObject.SetActive(true);

        //버튼 섞기
        ShuffleCode();

        GenerateQuestion(roundquestion[currentRound]);

        StartCoroutine(ShowQuestionRoutine());
    }

    // 정답 배열 생성
    void GenerateQuestion(int count)
    {
        //문제 패널 초기화
        foreach (Transform child in questionContainer)
        {
            Destroy(child.gameObject);
        }
        //라운드에 맞게 문제 배열
        for (int i = 0; i < count; i++)
        {
            int randomID = Random.Range(0, 4);
            questionSequence.Add(randomID);

            GameObject imgObj = Instantiate(questionPrefab, questionContainer);

            // 이미지 교체
            Image targetImage = imgObj.GetComponent<Image>();
            targetImage.sprite = codeData[randomID];
            targetImage.color = Color.white;

            // ★ 생성된 오브젝트를 리스트에 저장 (나중에 숨기거나 다시 켜기 위해)
            spawnedImageObjects.Add(imgObj);
        }
    }

    IEnumerator FirstStart()
    {
        
        FirstRunning = true;
        yield return new WaitForSeconds(4f);
        FirstRunning = false;
        FirstText.gameObject.SetActive(false);
        currentScore = 0f;
        isTimerRunning = false;
        StartRound();
    }

    IEnumerator ShowQuestionRoutine()
    {
        isTimerRunning = false;

        foreach (GameObject obj in spawnedImageObjects)
        {
            obj.SetActive(true);
        }

        questionContainer.gameObject.SetActive(true);
        animatorNPC.SetBool("First", true);
        yield return new WaitForSeconds(3.0f);

        questionContainer.gameObject.SetActive(false);
        animatorNPC.SetBool("First", false);
        isInputActive = true;
        isTimerRunning = true;
    }

    public void CodeInput(int code)
    {
        if(!isInputActive || isRoundClearing)
        {
            return;
        }

        if (questionSequence[inputCodeSum] == code)
        {
            
            Debug.Log("정답!");
            spawnedImageObjects[inputCodeSum].SetActive(false);

            inputCodeSum++;

            if(inputCodeSum >= questionSequence.Count)
            {
                isRoundClearing = true;

                animatorPlayer.SetTrigger("Click");
                animatorNPC.SetTrigger("Good");
                isTimerRunning = false;
                currentScore += 2;
                currentRound++;

                Invoke("StartRound", 1.0f);
            }
        }
        else
        {
            isInputActive = false;

            // 2. Bad 애니메이션 실행
            animatorNPC.SetTrigger("Bad");
            Debug.Log("땡! 틀렸습니다. 라운드 재시작!");

            // 3. ★ 핵심 수정: 바로 StartRound를 하지 않고, 1초(애니메이션 시간) 뒤에 실행합니다.
            Invoke("StartRound", 1.0f);
        }
    }
    void GameClear()
    {
        isTimerRunning = false;
        isInputActive = false;
        Result();
    }

    void GameOver()
    {
        Result();
    }

    public void Result()
    {
        backGroundPanel.SetActive(true);
        GameManager.Instance.AddGauge("Practice", currentScore);
        Practicegauge.fillAmount = GameManager.Instance.currentPracticeGauge / maxGauge;
        GaugeText.text = GameManager.Instance.currentPracticeGauge + " / " + maxGauge;
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
