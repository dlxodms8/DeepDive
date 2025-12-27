using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public Image myTimeImage;
    public Text myDayText;

    public GameObject myEventPopupPanel; // 팝업창 전체 (패널)
    public Text myEventTitleText;        // 제목 텍스트
    public Text myEventDescText;         // 설명 텍스트 

    void Start()
    {
        // 씬이 시작될 때, 싱글톤 매니저에게 내 이미지를 꽂아줍니다.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.dayText = myDayText;
            GameManager.Instance.timeImage = myTimeImage;
            GameManager.Instance.UpdateUI();
        }
    }
}
