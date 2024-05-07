using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ComboManager : MonoBehaviour
{
    public static ComboManager instance = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }

    public Image fillImage;
    public Text comboText;

    private int comboCount;
    private bool isInCombo;

    public float timeForCombo;

    public void Init()
    {
        Reset();
    }

    public void Reset()
    {
        isInCombo = false;
        comboCount = 0;

        fillImage.fillAmount = 0;

        fillImage.transform.parent.gameObject.SetActive(false);
    }

    public void OnCombo()
    {
        isInCombo = true;

        comboCount++;
        comboText.text = "X" + comboCount.ToString();

        fillImage.transform.parent.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(StartTimeForCombo());
    }

    IEnumerator StartTimeForCombo()
    {
        float counter = 0f;

        while(counter < timeForCombo)
        {
            fillImage.fillAmount = Mathf.Lerp(1, 0, counter/ timeForCombo);

            counter += Time.deltaTime;
            yield return null;
        }

        Reset();
    }

}
