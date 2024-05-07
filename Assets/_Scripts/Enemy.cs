using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Refrences")]
    public Animator animator;
    private Rigidbody rb;
    private Transform playerTransform;

    [Header("Death")]
    public float onDeadForce;
    public float fadeOutAfter;
    public Material deadMaterial;

    public SkinnedMeshRenderer bodyRend;
    public SkinnedMeshRenderer swordRend;

    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isAttacking;

    [Header("Misc")]
    public float followSpeed;
    public float distanceToStop;

    private EnemyBehaviour enemyBehaviour;
    private EnemyTeleport enemyTeleport;

    public void Init()
    {
        isAttacking = false;
        isDead = false;
        rb = GetComponent<Rigidbody>();

        enemyBehaviour = GetComponent<EnemyBehaviour>();
        enemyBehaviour.Init();

        enemyTeleport = GetComponent<EnemyTeleport>();
        enemyTeleport.Init();
    }


    public void AddTarget(Transform targetRef)
    {
        enemyBehaviour.StopBehaviour();
        playerTransform = targetRef;
    }

    public void RemoveTarget()
    {
        enemyBehaviour.ResumeBehaviuor();
        playerTransform = null;
    }

    private void FixedUpdate()
    {
        if(playerTransform != null)
        {
            if (!isDead)
            {
                float distance = Vector3.Distance(transform.position, playerTransform.position);

                if (distance > distanceToStop)
                {
                    isAttacking = false;
                    PlayRun();

                    transform.LookAt(playerTransform);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                    transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, followSpeed * Time.deltaTime);
                }
                else
                {
                    isAttacking = true;
                    Attack();
                }
            }
        }
        else
        {
            isAttacking = false;
            PlayIdle();
        }
    }

    #region OnTeleport

    public void RemoveBehaviour()
    {
        enemyBehaviour.StopBehaviour();
        enemyTeleport.OnPlayerTeleport();

        transform.GetChild(0).LookAt(enemyTeleport.GetCurrentTeleportPositon());
        transform.GetChild(0).eulerAngles = new Vector3(0, transform.GetChild(0).eulerAngles.y ,0);

        AttackInDefense();
    }

    public Transform GetTeleportPosition()
    {
        return enemyTeleport.GetUniqueTeleportPosition();
    }

    public int GetTeleportPostionIndex()
    {
        return enemyTeleport.GetAnimationIndex();
    }

    #endregion

    #region OnDeath

    public void OnDeath()
    {
        if (!isDead)
        {
            isDead = true;
            PlayDeath();
            ComboManager.instance.OnCombo();

            DeadFunctionality();
        }
    }

    void DeadFunctionality()
    {
        Material[] bodyMats = new Material[bodyRend.sharedMaterials.Length];
        for (int i = 0; i < bodyRend.sharedMaterials.Length; i++)
            bodyMats[i] = deadMaterial;

        bodyRend.sharedMaterials = bodyMats;


        Material[] swordMats = new Material[swordRend.sharedMaterials.Length];
        for (int i = 0; i < swordRend.sharedMaterials.Length; i++)
            swordMats[i] = deadMaterial;

        swordRend.sharedMaterials = swordMats;

        rb.velocity = -transform.forward * onDeadForce;

        Destroy(rb, fadeOutAfter);
        Destroy(GetComponent<Collider>(), fadeOutAfter);


        for (int i = 0; i < bodyRend.materials.Length; i++)
            ChangeBlendModeToFade(bodyRend.materials[i]);

        for (int i = 0; i < swordRend.materials.Length; i++)
            ChangeBlendModeToFade(swordRend.materials[i]);

        for (int i = 0; i < bodyRend.materials.Length; i++)
            StartCoroutine(FadeMatRoutine(bodyRend.materials[i], 0.1f));

        for (int i = 0; i < swordRend.materials.Length; i++)
            StartCoroutine(FadeMatRoutine(swordRend.materials[i], 0.1f));

        Destroy(gameObject, fadeOutAfter + 0.1f);
    }

    IEnumerator FadeMatRoutine(Material mat, float DurationToFade)
    {
        yield return new WaitForSeconds(fadeOutAfter);

        float counter = 0f;
        Color tempColor = Color.black;

        while (counter < DurationToFade)
        {
            tempColor = new Color(mat.color.r, mat.color.g, mat.color.b, Mathf.Lerp(1, 0, counter / DurationToFade));
            mat.SetColor("_Color", tempColor);

            counter += Time.unscaledDeltaTime;
            yield return null;
        }

        tempColor = new Color(mat.color.r, mat.color.g, mat.color.b, 0);
        mat.SetColor("_Color", tempColor);
    }

    void ChangeBlendModeToFade(Material standardShaderMaterial)
    {
        standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        standardShaderMaterial.SetInt("_ZWrite", 0);
        standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
        standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
        standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        standardShaderMaterial.renderQueue = 3000;
    }

    #endregion

    #region AnimatorFunc

    public void PlayIdle()
    {
        animator.SetBool("run",false);
    }

    public void PlayRun()
    {
        animator.SetBool("run", true);
    }

    public void PlayDeath()
    {
        animator.SetTrigger("hit");
    }

    public void Attack()
    {
        animator.SetTrigger("attack");
    }

    public void AttackInDefense()
    {
        animator.SetTrigger("attackinDefense");
    }

    #endregion

}
