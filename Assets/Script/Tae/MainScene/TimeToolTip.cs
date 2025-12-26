using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TimeToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("나타날 텍스트 오브젝트")]
    public Text descriptionText; // 설명 텍스트 (또는 패널)

    void Start()
    {
        // 시작할 때는 텍스트를 숨깁니다.
        if (descriptionText != null)
        {
            descriptionText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        SetTimeText();
    }

    // 마우스가 UI 요소 위로 올라왔을 때 실행
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionText != null)
        {
            descriptionText.gameObject.SetActive(true);
        }
    }

    // 마우스가 UI 요소에서 벗어났을 때 실행
    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionText != null)
        {
            descriptionText.gameObject.SetActive(false);
        }
    }

    void SetTimeText()
    {
        if(GameManager.Instance.D_Time == 0)
        {
            descriptionText.text = ("아침");
        }
        else if (GameManager.Instance.D_Time == 1)
        {
            descriptionText.text = ("점심");
        }
        else if (GameManager.Instance.D_Time == 2)
        {
            descriptionText.text = ("저녁");
        }

    }
}
