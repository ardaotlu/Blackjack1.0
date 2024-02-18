using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] GameObject table;
    private bool isOn = false;
    public Text InfoT;

    public void ShowSettings()
    {
        if (!isOn)
        {
            table.SetActive(true);
            isOn = true;
            InfoT.text = "";
        }
        else if(isOn)
        {
            table.SetActive(false);
            isOn = false;
        }
    }

}
