using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBall : MonoBehaviour
{
    [SerializeField]Character holder;
    [SerializeField] Image fillImage;
    [SerializeField] Image deltaFillImage;
    public bool IsCurrerntBall { get; set; }
    public bool IsBreak { get; set; }


    public void Initialize(Character holder)
    {
        this.holder = holder;
        IsBreak= false;
    }

    private void Update()
    {
        if (IsCurrerntBall)
        {
            fillImage.fillAmount = holder.ShieldGaugePercent;
            if(holder.ShieldGauge <= 0)
            {
                IsBreak= true;
                holder.GuardCrush();
                holder.shieldBallCurrent--;
                IsCurrerntBall= false;
            }
        }
        deltaFillImage.fillAmount = Mathf.SmoothStep(deltaFillImage.fillAmount, fillImage.fillAmount, Time.deltaTime * 10f);
        deltaFillImage.fillAmount = Mathf.Max(fillImage.fillAmount, deltaFillImage.fillAmount);
    }

}
