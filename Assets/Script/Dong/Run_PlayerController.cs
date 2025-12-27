using UnityEngine;
using System.Collections;

public class Run_PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpForce = 12f;  // 점프 힘을 더 크게
    public float jumpDuration = 0.2f; // 점프 높이를 빠르게 올리기 위해 점프 힘을 일정 시간 동안 강하게 유지

    [Header("Sliding Settings")]
    public float slideYOffset = 2.0f;  // 슬라이딩 시 내려갈 깊이
    public float slideDuration = 1f;   // 슬라이딩을 빠르게 종료하려면 짧게 설정

    [Header("References")]
    public HealthManager healthManager;

    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D capsuleCollider;
    private bool isGrounded;
    private bool isSliding = false;
    private bool isJumping = false;
    private bool isInvincible = false;
    public float invincibilityDuration = 1.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        // 점프 입력 (땅에 있을 때만 점프)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isSliding)
        {
            Jump();
        }

        // 슬라이딩 입력 (LeftShift 키를 누르고 있을 때만 슬라이딩 유지)
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded && !isJumping && !isSliding)
        {
            StartCoroutine(Slide());
        }

        // 기본적으로 뛰는 애니메이션
        if (!isSliding && isGrounded && !isJumping)
        {
            anim.Play("Run");  // 뛰는 애니메이션을 계속 유지
        }
    }

    void Jump()
    {
        rb.linearVelocity = Vector2.zero;  // 점프 전 속도 초기화
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
        isJumping = true;
        //anim.SetTrigger("Jump");

        // 빠른 반응을 위한 점프 지속 시간 조정
        StartCoroutine(JumpDuration());
    }

    IEnumerator JumpDuration()
    {
        // 짧은 시간 동안 점프 힘을 계속해서 증가시켜 빠른 반응을 구현
        yield return new WaitForSeconds(jumpDuration);
        isJumping = false;
    }

    IEnumerator Slide()
    {
        isSliding = true;

        // 1. 현재 Y축 높이 저장
        float originalY = transform.position.y;

        // 2. 슬라이딩 애니메이션 시작
        anim.SetTrigger("Slide");

        // 3. 캡슐 콜라이더의 세로 길이를 줄임 (슬라이딩 상태)
        capsuleCollider.size = new Vector2(capsuleCollider.size.x, capsuleCollider.size.y / 2);  // 세로 길이 절반으로 줄이기

        // 4. 슬라이딩 상태 유지 - Y축 감소
        float slideSpeed = 5f;  // 슬라이딩 속도를 빠르게 하기 위해 이동 속도 조정
        while (Input.GetKey(KeyCode.LeftShift))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - slideSpeed * Time.deltaTime, transform.position.z);
            yield return null;  // 슬라이딩 중에는 계속 기다림
        }

        // 5. 슬라이딩 종료 후 원래 Y축 위치로 복원
        transform.position = new Vector3(transform.position.x, originalY, transform.position.z);

        // 6. 캡슐 콜라이더의 세로 길이를 원래대로 복원
        capsuleCollider.size = new Vector2(capsuleCollider.size.x, capsuleCollider.size.y * 2);  // 원래 세로 길이로 복원

        // 7. 뛰는 애니메이션으로 복귀
        //anim.SetTrigger("Run");

        isSliding = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;  // 점프가 끝났으면 isJumping을 false로 설정
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle") && !isInvincible)
        {
            if (healthManager != null)
            {
                healthManager.TakeDamage();
                StartCoroutine(BecomeInvincible());
            }
            Destroy(collision.gameObject);
        }
    }

    IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    public void OnTimeUp()
    {
        Debug.Log("게임 오버!");
    }
}
