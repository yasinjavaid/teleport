using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public enum BehaviourType { Idle, IdleRotate }
    public BehaviourType behaviourType;

    [Header("IdleRotate")]
    public Vector3 pointA;
    public Vector3 pointB;

    public float idleRotateSpeed;
    private bool idleRotate;

    public void Init()
    {
        if(behaviourType == BehaviourType.IdleRotate)
            idleRotate = true;
    }

    private void Update()
    {
        if (idleRotate)
        {
            float time = Mathf.PingPong(Time.time * idleRotateSpeed, 1);
            transform.eulerAngles = Vector3.Lerp(pointA, pointB, time);
        }
    }

    public void StopBehaviour()
    {
        idleRotate = false;
    }

    public void ResumeBehaviuor()
    {
        idleRotate = true;
    }
}
