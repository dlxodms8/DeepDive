using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public Image myTimeImage;
    public Text myDayText;

    public GameObject myEventPopupPanel; // 팝업창 전체 (패널)
    public Text myEventTitleText;        // 제목 텍스트
    public Text myEventDescText;         // 설명 텍스트 

    [Header("UI 연결")]
    public GameObject exitPanel; // ESC 누르면 뜰 '종료 확인 패널' (인스펙터에서 연결)

    private bool isPanelActive = false;

    public GameObject EndingPanel;

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

    void Update()
    {
        // 1. ESC 키를 누르면 패널을 켜거나 끕니다.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleExitPanel();
        }
    }

    public void OnEndingClick()
    {
        EndingPanel.SetActive(true);
    }

    public void OnclickBack()
    {
        EndingPanel.SetActive(false);
    }

    public void ToggleExitPanel()
    {
        isPanelActive = !isPanelActive;

        if (exitPanel != null)
        {
            exitPanel.SetActive(isPanelActive);

            // 2. 패널이 떴을 때 게임을 일시정지하고 싶다면 아래 코드 사용
            // Time.timeScale = isPanelActive ? 0f : 1f;
        }
    }

    // 3. 실제 게임 종료 버튼에 연결할 함수
    public void QuitGame()
    {
        Debug.Log("게임을 종료합니다.");

#if UNITY_EDITOR
        // 유니티 에디터에서 실행 중일 때 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // 빌드된 게임(PC/모바일)에서 실제 종료
            Application.Quit();
        
#endif
    }
}
