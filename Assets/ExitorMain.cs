using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitorMain : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        StartCoroutine(MainMenuStart());
    }

    IEnumerator MainMenuStart()
    {
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene("MainMenu");
    }


}
