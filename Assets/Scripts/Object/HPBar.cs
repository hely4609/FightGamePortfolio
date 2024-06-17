using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] protected Character holder;
    [SerializeField] Image hpBarImage;
    public void Initialize(Character holder)
    {
        this.holder = holder;
        //hpBarImage
        hpBarImage.fillAmount = holder.HitPointPercent;
    }
    public void Update()
    {
        hpBarImage.fillAmount = holder.HitPointPercent;
        //Debug.Log(holder.HitPointPercent);
    }
}
