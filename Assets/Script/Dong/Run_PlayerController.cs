using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class Run_PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpForce = 12f;

    [Header("Sliding Settings")]
    public float slideSpeed = 5f;
    public float slideYDownPos = 0.5f;
    private float defaultY;

    [Header("References")]
    public HealthManager healthManager;
    public Sprite slidingDamageSprite;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D capsuleCollider;

    private Vector2 originalColliderSize;
    private bool isGrounded;
    private bool isSliding = false;
    private bool isJumping = false;
    private bool isInvincible = false;
    public float invincibilityDuration = 1.5f;
    private bool isDamaged = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        originalColliderSize = capsuleCollider.size;
        defaultY = transform.position.y;
    }

    void Update()
    {
        if (isDamaged) return;

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isSliding)
        {
            Jump();
        }

        // 슬라이딩 시작 (Shift를 누르는 순간)
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && !isJumping && !isSliding)
        {
            StartCoroutine(SlideRoutine());
        }
    }

    void Jump()
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
        isJumping = true;
        
    }

    IEnumerator SlideRoutine()
    {
        isSliding = true;

        // ✅ 단일 트리거 "Slide" 호출
        anim.SetTrigger("Slide");

        capsuleCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.5f);
        float targetY = defaultY - slideYDownPos;

        // Shift를 꾹 누르고 있는 동안 루프 유지
        while (Input.GetKey(KeyCode.LeftShift))
        {
            if (isDamaged) break;

            if (transform.position.y > targetY)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - slideSpeed * Time.deltaTime, transform.position.z);
            }
            yield return null;
        }

        // Shift를 떼거나 데미지를 입었을 때 처리
        if (!isDamaged)
        {
            ExitSlide();
        }
    }

    void ExitSlide()
    {
        // 원래 높이로 복구
        transform.position = new Vector3(transform.position.x, defaultY, transform.position.z);
        capsuleCollider.size = originalColliderSize;

        // ✅ 다시 "Slide" 트리거를 작동시켜 Run 상태로 전이하거나, 
        // 애니메이터 설정에 따라 "Run" 트리거를 명시적으로 호출합니다.
        anim.SetTrigger("Run");

        isSliding = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
            defaultY = transform.position.y; // 현재 지면 높이 갱신
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle") && !isInvincible)
        {
            HandleDamage();
            Destroy(collision.gameObject);
        }
    }

    void HandleDamage()
    {
        if (healthManager != null) healthManager.TakeDamage();
        StartCoroutine(BecomeInvincible());

        if (isSliding)
            StartCoroutine(SlideDamageEffect());
        else
            StartCoroutine(RunDamageEffect());
    }

    IEnumerator RunDamageEffect()
    {
        isDamaged = true;
        anim.SetTrigger("Run_Damage");
        yield return new WaitForSeconds(1.0f);
        isDamaged = false;
        anim.SetTrigger("Run");
    }

    IEnumerator SlideDamageEffect()
    {
        isDamaged = true;
        isSliding = false;

        anim.enabled = false;
        spriteRenderer.sprite = slidingDamageSprite;

        yield return new WaitForSeconds(0.5f);

        anim.enabled = true;
        isDamaged = false;

        ExitSlide();
    }

    IEnumerator BecomeInvincible()
    {
        isInvincible = true;
        float timer = 0;
        while (timer < invincibilityDuration)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            timer += 0.2f;
        }
        isInvincible = false;
    }

    public void OnTimeUp() 
    {
        // 1️⃣ 입력 차단용 상태 변경
        isDamaged = true;
        isSliding = false;
        isJumping = false;

        // 2️⃣ 물리 이동 완전 정지
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        // 3️⃣ 애니메이션 정지 (선택)
        if (anim != null)
            anim.enabled = false;
    }
}