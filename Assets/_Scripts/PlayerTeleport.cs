using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    [Header("Refrences")]
    public float[] attackDistances;

    public List<GameObject> targets;
    private GameObject currentTarget;

    private GameObject playerClone;

    private TeleportEffect teleportEffect;
    private FieldOfView fov;

    private Rigidbody rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();

        playerClone = Resources.Load("Characters/PlayerLatest") as GameObject;
        teleportEffect = GetComponent<TeleportEffect>();
        fov = GetComponentInChildren<FieldOfView>();
        fov.Init();
    }

    public void AddTarget(GameObject targetRef)
    {
        if (!targets.Contains(targetRef))
        {
            targets.Add(targetRef);
            targetRef.GetComponent<EnemyTeleport>().ShowTimer();
        }
    }

    public void InitTargets()
    {
        targets = new List<GameObject>();
    }

    public void OnTeleport()
    {
        print("On TP");
        if (!GetComponent<PlayerController>().isDead)
        {
            print("Not Dead");
            if (targets.Count > 0)
            {
                currentTarget = GetClosestEnemy(targets);

                print("Is Null?");
                if (currentTarget != null)
                {
                    print("Not Null!");
                    currentTarget.layer = LayerMask.NameToLayer("IgnoreObstacles");
                    StartCoroutine(TeleportRoutine());

                    targets.Remove(currentTarget);
                }
            }
        }
    }

    IEnumerator TeleportRoutine()
    {
        TimeManager.instance.ResetTimeScale();
        
        CameraFollow.instance.SetSpeedAtSlowMotion();
        CameraFollow.instance.DeletePlayer();
        teleportEffect.FadePlayer(GameManager.instance.GetCurrentPlayer());
        PlayerController oldPlayer = GetComponent<PlayerController>();
        oldPlayer.Vanish();
        GameManager.instance.DisableControls();

        Enemy currentEnemy = currentTarget.GetComponent<Enemy>();
        Transform teleportPos = currentEnemy.GetTeleportPosition();
        currentEnemy.RemoveBehaviour();

        GameObject newPlayer = Instantiate(playerClone);
        PlayerController newPlayerController = newPlayer.GetComponent<PlayerController>();

        newPlayer.transform.position = new Vector3(teleportPos.position.x, 0.96f, teleportPos.position.z);

        newPlayer.transform.LookAt(currentTarget.transform);
        newPlayer.transform.eulerAngles = new Vector3(0, newPlayer.transform.eulerAngles.y, 0);

        newPlayerController.PlayAttack(currentEnemy.GetTeleportPostionIndex());

        GameManager.instance.ChangePlayer(newPlayer);
        yield return null;
    }

    GameObject GetClosestEnemy(List<GameObject> enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject potentialTarget in enemies)
        {
            if (potentialTarget)
            {
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;

                    if (potentialTarget.GetComponent<EnemyTeleport>().IsTimeAvailable())
                        bestTarget = potentialTarget.transform;
                }
            }
        }

        if (bestTarget == null)
            return null;
        else
            return bestTarget.gameObject;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.gameState != GameManager.GameState.Start) return;

        if(rb.velocity.magnitude > 0 && !GetComponent<PlayerController>().isAttacking && !GetComponent<PlayerController>().isDead)
            CheckIfAnyTargetIsClose();
    }

    public void CheckIfAnyTargetIsClose()
    {
        for(int i = 0; i < targets.Count; i++)
        {
            if(targets[i] != null)
            {
                if(Vector3.Distance(targets[i].transform.position, transform.position) < attackDistances[i])
                {
                    transform.LookAt(targets[i].transform);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                    GetComponent<PlayerController>().PlayAttack(i);
                    targets[i].GetComponent<Enemy>().RemoveBehaviour();

                    targets[i].layer = LayerMask.NameToLayer("IgnoreObstacles");

                    targets.RemoveAt(i);
                }
            }
        }
    }
}
