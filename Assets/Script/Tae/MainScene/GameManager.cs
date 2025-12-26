using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance = null;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("목표치 목록")]
    public float currentDateGauge = 0f;
    public float currentSnsGauge = 0f;
    public float currentCompositionGauge = 0f;
    public float currentPracticeGauge = 0f;

    [Header("게임 날짜 및 시간")]
    public float D_Day = 14f;
    public float D_Time = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
