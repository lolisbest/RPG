using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gauge : MonoBehaviour
{
    public Image Background;
    public Image Middle;
    public Image Front;
    public float SpeedMultiply = 1f;
    public bool IsReducing;

    void Awake()
    {
        Init();
    }

    void Start()
    {
 
    }

    void Update()
    {
        if(!IsReducing)
        {
            if(Front.fillAmount < Middle.fillAmount)
            {
                StartCoroutine(LeadMiddle());
            }
            else if (Middle.fillAmount < Front.fillAmount)
            {
                Middle.fillAmount = Front.fillAmount;
            }
        }
    }

    public void SetCurretRate(float rate)
    {
        Front.fillAmount = rate;
        //if (!IsReducing)
        //{
        //    StartCoroutine(LeadMiddle());
        //}
    }

    private IEnumerator LeadMiddle()
    {
        IsReducing = true;

        yield return new WaitForSeconds(0.5f);

        while (Front.fillAmount < Middle.fillAmount)
        {
            Middle.fillAmount -= SpeedMultiply * Time.deltaTime;

            if(Middle.fillAmount < Front.fillAmount)
            {
                Middle.fillAmount = Front.fillAmount;
            }
            yield return new WaitForEndOfFrame();
        }

        IsReducing = false;
    }

    public void Init()
    {
        Background.fillAmount = 1f;
        Middle.fillAmount = 1f;
        Front.fillAmount = 1f;
        IsReducing = false;
    }
}
