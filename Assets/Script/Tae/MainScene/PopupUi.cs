using UnityEngine;
using UnityEngine.UI;
public class PopupUi : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject panelRoot; // 팝업창 전체 (부모 패널)
    public Text titleText;       // 제목
    public Text descText;        // 설명

    void Start()
    {
        // 게임 시작 시 팝업창 숨기기
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    // 팝업 띄우기 (시간 멈춤 없음)
    public void Show(string title, string desc)
    {
        titleText.text = title;
        descText.text = desc;

        if (panelRoot != null)
            panelRoot.SetActive(true);
    }

    // 닫기 버튼용 함수
    public void Close()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }
}
