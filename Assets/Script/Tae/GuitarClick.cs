using UnityEngine;
using UnityEngine.UI;

public class GuitarClick : MonoBehaviour
{

    public int colorId;
    public MiniGameGuitar MiniGameGuitar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MiniGameGuitar = GameObject.Find("Canvas").GetComponent<MiniGameGuitar>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickCode()
    {
        MiniGameGuitar.CodeInput(colorId);
    }
}
