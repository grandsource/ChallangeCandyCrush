using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonControler : MonoBehaviour
{
    
    public void showText(string text)
    {
        Text txt = transform.Find("Text").GetComponent<Text>();
        txt.text = text;
    }

    public void ChangeSense(int senseNum)
    {
        SceneManager.LoadScene(senseNum);
    }
}
