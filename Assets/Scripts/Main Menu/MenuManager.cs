using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class MenuManager : MonoBehaviourPunCallbacks
{
    private void Start()
    {
    }

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
