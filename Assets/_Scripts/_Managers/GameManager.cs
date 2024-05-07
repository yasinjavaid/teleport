using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    [Header("Object Refrences")]
    public GameObject currentPlayer;
    public VariableJoystick joyStick;

    public Enemy[] allEnemies;

    [Header("Misc")]
    public GameState gameState;
    public enum GameState { Loading, Start, Won, Fail }

    [HideInInspector] public bool pauseControls;

    public string lastTeleportSide;
    public string lastTeleportPos;


    private void Start()
    {
        lastTeleportSide = "";
        lastTeleportPos = "";

        InputManager.instance.InitRefrence(currentPlayer.GetComponent<PlayerTeleport>());
        currentPlayer.GetComponent<PlayerController>().Init(joyStick);

        CameraFollow.instance.Init(currentPlayer);
        UIManager.instance.Init();
        ComboManager.instance.Init();

        for (int i = 0; i < allEnemies.Length; i++)
            allEnemies[i].Init();
    }

    public void ChangePlayer(GameObject newPlayer)
    {
        currentPlayer = newPlayer;
        CameraFollow.instance.ChangePlayer(newPlayer.transform);
        InputManager.instance.InitRefrence(currentPlayer.GetComponent<PlayerTeleport>());
        currentPlayer.GetComponent<PlayerController>().Init(joyStick);
    }

    public void ActiveControls()
    {
        pauseControls = false;
    }

    public void DisableControls()
    {
        pauseControls = true;
    }

    public GameObject GetCurrentPlayer()
    {
        return currentPlayer;
    }


    public void OnPlayClick()
    {
        if(gameState != GameState.Start)
        {
            gameState = GameState.Start;
            UIManager.instance.OnPlay();
        }
    }

    public void OnGameFail()
    {
        if (gameState != GameState.Fail)
        {
            print("Game Failed");
            gameState = GameState.Fail;

            UIManager.instance.OnGameFailed();
        }
    }

    public void OnGameWon()
    {
        if(gameState != GameState.Won)
        {
            print("Game Won");
            gameState = GameState.Won;
            
            currentPlayer.GetComponent<PlayerController>().OnWon();

            UIManager.instance.OnGameWon();
        }
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlayNew");
    }
}
