using UnityEngine;

public class Arrow_PlayerController : MonoBehaviour
{
    [Header("컴포넌트 설정")]
    public SpriteRenderer spriteRenderer;
    public ArrowManager arrowManager;

    [Header("스프라이트 설정")]
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite idleSprite;
    public Sprite failSprite; // 추가: 시간 종료(실패) 시 보여줄 스프라이트
    public Sprite errorSprite; // 추가: 틀렸을 때 잠깐 보여줄 스프라이트

    [Header("이동 설정")]
    public float moveDistance = 0.6f;
    private Vector3 originPosition;

    private bool isTimeUp = false;
    private bool isClear = false; // 추가: 게임 클리어 여부 확인



    void Start()
    {
        originPosition = transform.localPosition;
    }

    void Update()
    {
        // 시간이 종료되었다면 입력 처리를 하지 않음
        if (ArrowGameManager.Instance.isGameOver) return;

        if (arrowManager != null && arrowManager.CanInput)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) HandleInput(0, upSprite, Vector3.up);
            else if (Input.GetKeyDown(KeyCode.DownArrow)) HandleInput(1, downSprite, Vector3.down);
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) HandleInput(2, leftSprite, Vector3.left);
            else if (Input.GetKeyDown(KeyCode.RightArrow)) HandleInput(3, rightSprite, Vector3.right);
        }
    }

    void HandleInput(int direction, Sprite moveSprite, Vector3 moveDir)
    {
        ChangeSpriteAndPosition(moveSprite, moveDir);
        arrowManager.CheckAnswer(direction);
    }

    void ChangeSpriteAndPosition(Sprite newSprite, Vector3 moveDir)
    {
        if (newSprite == null) return;

        spriteRenderer.sprite = newSprite;
        transform.localPosition = originPosition + (moveDir * moveDistance);

        if (GetComponent<Animator>() != null)
            GetComponent<Animator>().enabled = false;

        CancelInvoke("ResetPlayer");
        Invoke("ResetPlayer", 0.3f);
    }

    void ResetPlayer()
    {
        // 만약 리셋 도중에 시간이 종료되었다면 Idle로 돌아가지 않음
        if (isTimeUp) return;

        spriteRenderer.sprite = idleSprite;
        transform.localPosition = originPosition;

        if (GetComponent<Animator>() != null)
            GetComponent<Animator>().enabled = true;
    }

    public void OnWrongInput()
    {
        if (isTimeUp || isClear) return;

        // 1. 에러 이미지로 교체
        spriteRenderer.sprite = errorSprite;

        // 2. 애니메이터가 있다면 일시 정지
        if (GetComponent<Animator>() != null)
            GetComponent<Animator>().enabled = false;

        // 3. 0.2초 정도만 보여주고 다시 Idle로 복구 (Invoke 활용)
        CancelInvoke("ResetPlayer");
        Invoke("ResetPlayer", 0.4f);
        transform.localPosition = originPosition + new Vector3(0, 1f, 0);
    }


    public void OnGameClear()
    {
        isClear = true;
        Debug.Log("Game Clear! OnTimeUp will be ignored.");
    }

    // 추가: 시간이 종료되었을 때 매니저에서 호출할 함수
    public void OnTimeUp()
    {
        if (isClear) return; // 이미 성공했다면 무시

        isTimeUp = true;
        CancelInvoke("ResetPlayer"); // 진행 중인 리셋 예약 취소

        // [수정] 위치 설정: 원래 위치(originPosition)에서 Y축으로 2만큼 내림
        transform.localPosition = originPosition + new Vector3(0, -1f, 0);
        if (ArrowGameManager.Instance.isGameOver && !ArrowGameManager.Instance.isGameclear)
        {
            // 실패 스프라이트 적용
            spriteRenderer.sprite = failSprite;
        }

        // 애니메이터 끄기 (실패 이미지가 유지되도록)
        if (GetComponent<Animator>() != null)
            GetComponent<Animator>().enabled = false;

        Debug.Log("Time Up! Player moved down and changed to fail sprite.");
    }
}