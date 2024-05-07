using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTeleport : MonoBehaviour
{
    public Transform[] posParents;
    private List<Transform> teleportPositions;

    private FieldOfView fieldOfView;
    private Timer timer;

    private Transform selectedPos;

    public void Init()
    {
        fieldOfView = GetComponentInChildren<FieldOfView>();
        fieldOfView.Init();

        timer = GetComponent<Timer>();
        timer.Init();
    }

    #region Timer

    public void HideTimer()
    {
        timer.StopTimer();
        timer.HideTimer();
    }

    public void ShowTimer()
    {
        timer.StartTimer();
        timer.ShowTimer();
    }

    public bool IsTimeAvailable()
    {
        return timer.IsTimeAvailable();
    }

    #endregion

    #region Teleport

    public void OnPlayerTeleport()
    {
        fieldOfView.HideFOV();
        HideTimer();
    }

    public Transform GetUniqueTeleportPosition()
    {
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);

        Transform positionsParent = GetUniqueRandomTeleportSide();

        teleportPositions = new List<Transform>();

        for (int i = 0; i < positionsParent.childCount; i++)
            if (positionsParent.GetChild(i).name.Contains("TP"))
                teleportPositions.Add(positionsParent.GetChild(i));

        selectedPos = GetUniqueRandomTeleportPositionInedx();//Random.Range(0, teleportPositions.Count - 1);

        return selectedPos;
    }

    public Transform GetCurrentTeleportPositon()
    {
        return selectedPos;
    }

    public int GetAnimationIndex()
    {
        int value = (int)Char.GetNumericValue(selectedPos.name[2]);
        return value - 1;
    }

    private Transform GetUniqueRandomTeleportSide()
    {
        Transform transformToReturn = null;

        List<Transform> clonedList = new List<Transform>();

        for (int i = 0; i < posParents.Length; i++)
            clonedList.Add(posParents[i]);

        if (GameManager.instance.lastTeleportSide != "")
        {
            int indexToDelete = 0;
            for(int i = 0; i < clonedList.Count; i++)
                if (clonedList[i].gameObject.name == GameManager.instance.lastTeleportSide)
                    indexToDelete = i;

            clonedList.RemoveAt(indexToDelete);
        }

        transformToReturn = clonedList[UnityEngine.Random.Range(0, clonedList.Count)];
        GameManager.instance.lastTeleportSide = transformToReturn.name;

        return transformToReturn;
    }


    private Transform GetUniqueRandomTeleportPositionInedx()
    {
        Transform transformToReturn = null;

        List<Transform> clonedList = new List<Transform>();

        for (int i = 0; i < teleportPositions.Count - 1; i++)
            clonedList.Add(teleportPositions[i]);

        if(GameManager.instance.lastTeleportPos != "")
        {
            int indexToDelete = 0;
            for (int i = 0; i < clonedList.Count; i++)
                if (clonedList[i].gameObject.name == GameManager.instance.lastTeleportPos)
                    indexToDelete = i;

            clonedList.RemoveAt(indexToDelete);
        }

        transformToReturn = clonedList[UnityEngine.Random.Range(0, clonedList.Count)];
        GameManager.instance.lastTeleportPos = transformToReturn.name;

        return transformToReturn;
    }

    #endregion
}
