using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportEffect : MonoBehaviour
{
    public float DurationToFade;


    [Header("Refrences")]
    public GameObject teleportParticles;
    public SkinnedMeshRenderer playerRend;
    public SkinnedMeshRenderer capeRend;
    public SkinnedMeshRenderer swordRend;

    public void FadePlayer(GameObject playerRef)
    {
        GameObject currentRef = playerRef;

        Vector3 particlePos = new Vector3(playerRef.transform.position.x, playerRef.transform.position.y - 0.6f, playerRef.transform.position.z);
        Quaternion particleRot = Quaternion.Euler(new Vector3(-90, 0, 0));

        GameObject particleRef = Instantiate(teleportParticles, particlePos, particleRot);


        //Clonning Materials
        Material playerMat0 = new Material(playerRend.material);
        Material capMat = new Material(capeRend.material);
        Material swordMat = new Material(swordRend.materials[0]);
        Material swordMat1 = new Material(swordRend.materials[1]);

        playerRend.materials[0] = playerMat0;
        capeRend.material = capMat;

        swordRend.materials[0] = swordMat;
        swordRend.materials[1] = swordMat1;


        ChangeBlendModeToFade(playerRend.materials[0]);
        ChangeBlendModeToFade(capeRend.material);
        ChangeBlendModeToFade(swordRend.materials[0]);
        ChangeBlendModeToFade(swordRend.materials[1]);

        StartCoroutine(FadeMatRoutine(playerRend.materials[0]));
        StartCoroutine(FadeMatRoutine(capeRend.material));
        StartCoroutine(FadeMatRoutine(swordRend.materials[0]));
        StartCoroutine(FadeMatRoutine(swordRend.materials[1]));

        Destroy(currentRef, DurationToFade + 0.1f);
        Destroy(particleRef, DurationToFade + 0.1f);
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

    IEnumerator FadeMatRoutine(Material mat)
    {
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
}
