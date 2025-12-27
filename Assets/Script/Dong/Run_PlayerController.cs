using UnityEngine;
using System.Collections;

public class Run_PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpForce = 7f;

    [Header("Sliding Settings")]
    public float slideYOffset = 2.0f;     // 슬라이딩 시 내려갈 깊이
    public float slideDownDuration = 1.2f; // 바닥에 내려가 있을 시간 (이 값을 늘리세요!)

    [Header("References")]
    public HealthManager healthManager;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool isSliding = false;

    private bool isInvincible = false;
    public float invincibilityDuration = 1.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isSliding)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && !isSliding)
        {
            StartCoroutine(Slide());
        }
    }

    void Jump()
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
    }

    IEnumerator Slide()
    {
        isSliding = true;

        // 1. 현재 Y축 높이 저장
        float originalY = transform.position.y;

        // 2. 캐릭터를 아래로 이동시키고 슬라이딩 애니메이션 재생
        transform.position = new Vector3(transform.position.x, originalY - slideYOffset, transform.position.z);
        anim.Play("Slide");

        // 3. 설정한 시간(slideDownDuration)만큼 아래에서 대기
        // 이 시간이 길어질수록 캐릭터가 바닥에 오래 붙어있습니다.
        yield return new WaitForSeconds(slideDownDuration);

        // 4. 시간이 다 되면 다시 원래 높이로 복구
        transform.position = new Vector3(transform.position.x, originalY, transform.position.z);

        // 5. 애니메이션을 다시 달리기로 변경
        anim.Play("Run");

        isSliding = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
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