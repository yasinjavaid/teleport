using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    public GameObject mainMenu;
    public GameObject gamePlay;
    public GameObject levelCompleted;
    public GameObject levelFailed;

    public void Init()
    {
        mainMenu.SetActive(true);
    }

    public void OnPlay()
    {
        mainMenu.SetActive(false);
        gamePlay.SetActive(true);
    }

    public void OnGameWon()
    {
        levelCompleted.SetActive(true);
    }

    public void OnGameFailed()
    {
        levelFailed.SetActive(true);
    }
}
