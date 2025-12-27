using UnityEngine;
using UnityEngine.UI;
public class PopupUi : MonoBehaviour
{
    [Header("팝업 구성요소 연결")]
    public GameObject contentPanel; // 팝업창 패널 (부모)
    public Text titleText;          // 제목
    public Text descText;           // 설명

    // public Image iconImage; <--- 아이콘 변수 삭제됨

    void Start()
    {
        ClosePopup(); // 시작 시 꺼두기
    }

    // ★ 수정됨: 아이콘(Sprite) 매개변수를 없앴습니다.
    public void Show(string title, string desc)
    {
        titleText.text = title;
        descText.text = desc;

        // 아이콘 설정 로직 삭제됨

        contentPanel.SetActive(true);
        Time.timeScale = 0f; // 시간 정지
    }

    public void ClosePopup()
    {
        contentPanel.SetActive(false);
        Time.timeScale = 1f; // 시간 재생
    }
}
