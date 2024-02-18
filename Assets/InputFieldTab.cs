using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldTab : MonoBehaviour
{
    public InputField usernameText;
    public InputField passwordText;

    public int inputSelected;

    void Start()
    {
        usernameText.Select(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab)) 
        { 
        passwordText.Select();
        }
        if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
        {
            usernameText.Select();
        }

    }
}
