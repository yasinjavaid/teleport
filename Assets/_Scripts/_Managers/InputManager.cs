using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    public PlayerTeleport playerTeleport;

    public void InitRefrence(PlayerTeleport teleportRef)
    {
        playerTeleport = teleportRef;
    }

    private void Update()
    {
        if (!playerTeleport) return;

        if (GameManager.instance.gameState == GameManager.GameState.Start)
        {
            if (Input.GetMouseButtonUp(0))
                playerTeleport.OnTeleport();
        }
    }
}
