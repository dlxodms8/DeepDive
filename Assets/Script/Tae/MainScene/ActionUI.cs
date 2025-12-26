using System.Collections;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.UI;

public class ActionUI : MonoBehaviour
{

    public RectTransform phoneRect; //휴대폰 이미지
    public GameObject backGroundPanel; //패널

    public float animationTime = 0.8f; //올라오는 시간
    public float showPosY; //올라왔을 때의 Y
    public float hidePosY; //내려갔을 때의 Y

    public Coroutine currentCoroutine;

    public void ShowPhone()
    {
        backGroundPanel.SetActive(true);
        MovePhone(showPosY);
    }

    public void HidePhone()
    {
        MovePhone(hidePosY);
        StartCoroutine(PanelAfterMove());
    }

    void MovePhone(float targetY)
    {
        if(currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(MoveCoroutine(targetY));
    }

    IEnumerator MoveCoroutine(float targetY)
    {
        Vector2 startPos = phoneRect.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, targetY);
        float time = 0;

        while (time < animationTime)
        {
            // 부드럽게 이동 (Lerp)
            phoneRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, time / animationTime);
            time += Time.deltaTime;
            yield return null;
        }

        phoneRect.anchoredPosition = targetPos;
    }

    IEnumerator PanelAfterMove()
    {
        yield return new WaitForSeconds(animationTime);
        backGroundPanel.SetActive(false); // 다 내려가면 배경 끄기
    }
}
