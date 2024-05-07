using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public enum MyType { Player, Enemy }
    public MyType myType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy" && myType == MyType.Player)
        {
            if (!GetComponentInParent<PlayerController>().isDead && GetComponentInParent<PlayerController>().isAttacking)
            {
                other.GetComponent<Enemy>().OnDeath();
            }
        }

        else if (other.gameObject.tag == "Player" && myType == MyType.Enemy)
        {
            if (!GetComponentInParent<Enemy>().isDead && GetComponentInParent<Enemy>().isAttacking)
            {
                other.GetComponent<PlayerController>().OnDeath();
            }
        }
    }
}
