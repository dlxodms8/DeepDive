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

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isSliding)
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && !isJumping && !isSliding)
        {
            StartCoroutine(SlideRoutine());
        }

        // 슬라이딩 도중 점프를 누르면 슬라이딩을 취소하고 점프하게 하려면 아래 로직 추가

        if (Input.GetKeyDown(KeyCode.Space) && isSliding)
        {
            StopAllCoroutines();
            ExitSlide();
            Jump();
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
        anim.SetTrigger("Slide");

        // 콜라이더 크기 조절 및 위치(Offset) 조정
        capsuleCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.5f);
        capsuleCollider.offset = new Vector2(0, originalColliderSize.y * 0.005f); // 중심점을 아래로 내림

        while (Input.GetKey(KeyCode.LeftShift))
        {
            if (isDamaged) break;
            // 위치를 직접 빼는 코드 대신 물리 엔진에 맡기거나 애니메이션으로 처리 권장
            yield return null;
        }

        if (!isDamaged)
        {
            ExitSlide();
        }
    }

    void ExitSlide()
    {
        // 순간이동 코드 제거 (물리 흐름 방해 방지)
        capsuleCollider.size = originalColliderSize;
        capsuleCollider.offset = Vector2.zero; // 오프셋 복구

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
        // 1?? 입력 차단용 상태 변경
        isDamaged = true;
        isSliding = false;
        isJumping = false;

        // 2?? 물리 이동 완전 정지
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        // 3?? 애니메이션 정지 (선택)
        if (anim != null)
            anim.enabled = false;
    }
    }