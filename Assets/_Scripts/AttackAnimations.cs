using UnityEngine;

public class AttackAnimations : StateMachineBehaviour
{
    public float slowDownFactor = 0.2f;
    public float slowDownDuration = 1.5f;

    private float currentTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TimeManager.instance.DoSlowMotion(slowDownFactor, slowDownDuration);

        animator.GetComponentInParent<PlayerController>().Attacking();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentTime = stateInfo.normalizedTime - Mathf.Floor(stateInfo.normalizedTime);
        if (currentTime > 0.7f)
            animator.GetComponentInParent<PlayerController>().HideWeaponVfx();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameManager.instance.ActiveControls();

        animator.GetComponentInParent<PlayerController>().AttackEnd();
    }
}
