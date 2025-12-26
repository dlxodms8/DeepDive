using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ScreenFader : MonoBehaviour
{
    [Header("연결할 것들")]
    public Image fadeImage;

    [Header("속도 설정")]
    public float closeSpeed = 0.5f; // 닫힐 때 속도
    public float openSpeed = 1.0f;  // 열릴 때 속도

    private int cutoffPropertyID;
    private Material currentMaterial;
    private float maxCutoff = 0.8f;

    // ★ [핵심 추가] 모든 페이더가 공유하는 "현재 이동 중인가?" 상태 변수
    public static bool isTransitioning = false;

    private void Start()
    {
        cutoffPropertyID = Shader.PropertyToID("_Cutoff");
        if (fadeImage != null)
        {
            currentMaterial = fadeImage.material;

            // ★★★ [여기가 수정되었습니다] ★★★
            // 만약 씬 이동 중이라면, 새로 태어난 페이더는 화면을 밝게 열지 말고
            // 얌전히 검은 화면(0)인 상태로 켜져 있어야 합니다.
            if (isTransitioning)
            {
                SetCutoff(0f);
                fadeImage.raycastTarget = true;
                fadeImage.gameObject.SetActive(true);
            }
            else
            {
                // 평소(처음 게임 켰을 때)에만 밝게 시작
                SetCutoff(maxCutoff);
                fadeImage.raycastTarget = false;
                // fadeImage.gameObject.SetActive(false); // (주석 유지)
            }
        }
    }

    public void PlayTransition(Action onDark)
    {
        if (fadeImage == null) return;

        isTransitioning = true; // ★ "이동 시작한다!" 깃발 들기

        StopAllCoroutines();
        fadeImage.gameObject.SetActive(true);
        fadeImage.raycastTarget = true;
        StartCoroutine(TransitionRoutine(onDark));
    }

    public void ForceFadeOut(Action onFinished = null)
    {
        if (fadeImage == null) return;

        StopAllCoroutines();

        // 1. 강제로 검게 만듦 (번쩍임 방지 2차 안전장치)
        SetCutoff(0f);
        if (fadeImage.material != null)
        {
            fadeImage.material.SetFloat(cutoffPropertyID, 0f);
        }

        fadeImage.gameObject.SetActive(true);

        StartCoroutine(FadeOutOnlyRoutine(onFinished));
    }

    private void SetCutoff(float value)
    {
        if (currentMaterial != null) currentMaterial.SetFloat(cutoffPropertyID, value);
    }

    IEnumerator TransitionRoutine(Action onDark)
    {
        float currentCutoff = maxCutoff;

        while (currentCutoff > 0f)
        {
            currentCutoff -= Time.unscaledDeltaTime * closeSpeed;
            SetCutoff(currentCutoff);
            yield return null;
        }
        SetCutoff(0f);

        if (onDark != null) onDark.Invoke();

        yield return new WaitForSecondsRealtime(0.5f);

        yield return StartCoroutine(FadeOutOnlyRoutine(null));
    }

    //IEnumerator FadeOutOnlyRoutine(Action onFinished)
    //{
    //    // 1초 대기 (검은 화면 유지)
    //    yield return new WaitForSecondsRealtime(1f);

    //    float currentCutoff = 0f;

    //    if (currentMaterial != null)
    //    {
    //        currentMaterial.SetFloat(cutoffPropertyID, 0f);
    //        currentCutoff = 0f;
    //    }

    //    while (currentCutoff < maxCutoff)
    //    {
    //        currentCutoff += Time.unscaledDeltaTime * openSpeed;
    //        SetCutoff(currentCutoff);
    //        yield return null;
    //    }
    //    SetCutoff(maxCutoff);

    //    isTransitioning = false; // ★ "이동 끝났다!" 깃발 내리기

    //    fadeImage.raycastTarget = false;
    //    fadeImage.gameObject.SetActive(false);

    //    if (onFinished != null) onFinished.Invoke();
    //}

    IEnumerator FadeOutOnlyRoutine(Action onFinished)
    {
        // 1. 검은 화면 상태로 대기 (이 시간은 필요하면 0.5f 등으로 줄이세요)
        yield return new WaitForSecondsRealtime(1f);

        // ★★★ [위치 변경] ★★★
        // 원래 맨 아래에 있던 이 코드를 여기로 가져옵니다.
        // 이제 화면이 열리기 '시작함'과 동시에 게임 시간도 흐릅니다! (반응 속도 UP)
        if (onFinished != null) onFinished.Invoke();

        float currentCutoff = 0f;

        if (currentMaterial != null)
        {
            currentMaterial.SetFloat(cutoffPropertyID, 0f);
            currentCutoff = 0f;
        }

        // 2. 화면이 서서히 열림 (이제 게임이 진행되는 중에 화면이 열립니다)
        while (currentCutoff < maxCutoff)
        {
            currentCutoff += Time.unscaledDeltaTime * openSpeed;
            SetCutoff(currentCutoff);
            yield return null;
        }
        SetCutoff(maxCutoff);

        isTransitioning = false;

        fadeImage.raycastTarget = false;
        fadeImage.gameObject.SetActive(false);

        // (원래 여기 있던 코드를 위로 올렸습니다)
    }
}