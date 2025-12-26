using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public Image myTimeImage;
    public Text myDayText;


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
