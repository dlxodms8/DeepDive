using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class ActionButton : MonoBehaviour
{

    public int actionID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActionClick()
    {
        GameManager.Instance.StartAction(actionID);
    }
}
