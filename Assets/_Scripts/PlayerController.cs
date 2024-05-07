using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region PublicVariables

    [Header("Velocity Parameters")]
    public float velocity;
    public float turnSpeed = 10;

    public float speedMultiplier;
    private float runSpeed;

    [Header("Death")]
    public float onDeadForce;
    public float fadeOutAfter;
    public Material deadMaterial;

    public SkinnedMeshRenderer bodyRend;
    public SkinnedMeshRenderer swordRend;


    [Header("Object Refrences")]
    public Animator animator;
    public GameObject[] WeaponVfx;

    [HideInInspector] public bool isDead;
    [HideInInspector] public bool isAttacking;
    #endregion


    #region Private Variables

    Transform cam;
    Rigidbody rb;
    
    float angle;
    float deadzone = 0.01f;

    Vector2 input;
    Quaternion targetRotation;
    Quaternion lastRotation;

    VariableJoystick variableJoystick;

    #endregion

    public void Init(VariableJoystick joyStickRef)
    {
        isDead = false;
        isAttacking = false;

        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;

        lastRotation = transform.rotation;

        variableJoystick = joyStickRef;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.gameState != GameManager.GameState.Start) return;

        if (!isDead && !GameManager.instance.pauseControls)
        {
            GetInput();


            if (input.magnitude < deadzone)
            {
                input = Vector2.zero;
                rb.velocity = Vector3.zero;
                transform.rotation = lastRotation;

                ResetRunSpeed();
            }
            else
            {
                CalculateDirection();
                Rotate();
                Move();

                IncreseRunSpeed();
            }
        }
    }

    void GetInput()
    {
        input.x = variableJoystick.Direction.x;
        input.y = variableJoystick.Direction.y;
    }

    void CalculateDirection()
    {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += cam.eulerAngles.y;
    }

    void Rotate()
    {
        targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        lastRotation = transform.rotation;
    }

    void Move()
    {
        rb.velocity = transform.forward * velocity;
    }

    public void Attacking()
    {
        isAttacking = true;
    }

    public void AttackEnd()
    {
        isAttacking = false;
    }

    public void OnWon()
    {
        SetRunFloat(0);
        rb.velocity = Vector3.zero;
    }

    #region SpecialEffects

    public void ShowWeaponVfx()
    {
        for (int i = 0; i < WeaponVfx.Length; i++)
        {
            WeaponVfx[i].SetActive(true);
        }
    }

    public void HideWeaponVfx()
    {
        for (int i = 0; i < WeaponVfx.Length; i++)
        {
            WeaponVfx[i].SetActive(false);
        }
    }

    #endregion

    #region Animator

    public void Vanish()
    {
        animator.SetTrigger("vanish");
    }

    public void ResetRunSpeed()
    {
        runSpeed = 0;
        SetRunFloat(runSpeed);
    }

    public void IncreseRunSpeed()
    {
        runSpeed += speedMultiplier * Time.deltaTime;
        runSpeed = Mathf.Clamp(runSpeed, 0, 1);
        SetRunFloat(runSpeed);
    }

    public void SetRunFloat(float value)
    {
        animator.SetFloat("run", value);
    }
    public float GetRunFloat()
    {
        return animator.GetFloat("run");
    }

    public void PlayAttack(int animationIndex)
    {
        animator.SetTrigger("attack");
        animator.SetInteger("attackType", animationIndex + 1);

        ShowWeaponVfx();
    }

    public void PlayStandAttack()
    {
        animator.SetTrigger("standingAttack");
    }

    public void PlayDeath()
    {
        animator.SetTrigger("hit");
    }

    #endregion

    #region Dead

    public void OnDeath()
    {
        if (!isAttacking)
        {
            isDead = true;
            PlayDeath();

            gameObject.layer = LayerMask.NameToLayer("IgnoreObstacles");
            CameraFollow.instance.DeletePlayer();

            GameManager.instance.OnGameFail();

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
}
