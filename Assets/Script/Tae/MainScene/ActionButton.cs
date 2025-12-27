using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.SceneManagement;
using System.Collections;

public class ActionButton : MonoBehaviour
{

    public int actionID;
    public RectTransform curtainPanel;
    public float moveSpeed = 1500f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //if (curtainPanel != null)
        //{
        //    // Y축 위치를 -2500 정도로 설정해서 화면 아래로 완전히 치워버림
        //    // (화면 해상도가 높아도 안 보이게 넉넉하게 내립니다)
        //    curtainPanel.anchoredPosition = new Vector2(0, -2500f);
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActionClick()
    {
        GameManager.Instance.StartAction(actionID);
    }

    public void EndingClick()
    {
        StartCoroutine(MoveUpRoutine());
    }

    IEnumerator MoveUpRoutine()
    {
        // ★ 2. 패널이 화면 중앙(y=0)보다 아래에 있는 동안 계속 반복
        while (curtainPanel.anchoredPosition.y < 0)
        {
            // 위로 올리기
            curtainPanel.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;

            // ★ 중요: 여기서 한 프레임을 쉽니다. (이게 없으면 멈추거나 바로 넘어감)
            yield return null;
        }

        // 3. 다 올라왔으면 빈틈없게 0으로 딱 맞춤
        curtainPanel.anchoredPosition = Vector2.zero;

        // 잠깐(0.5초) 검은 화면을 유지했다가 씬 이동 (여운)
        yield return new WaitForSeconds(0.5f);

        // 4. 씬 이동
        GameManager.Instance.ENDING();
    }
}
