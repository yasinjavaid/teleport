using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignPositions : MonoBehaviour
{
    public Vector3[] positions;
    public Vector3[] rotations;


    private void OnEnable()
    {
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
            transform.GetChild(i).localPosition = positions[i];

        for (int i = 0; i < childCount; i++)
            transform.GetChild(i).localEulerAngles = rotations[i];
    }
}
