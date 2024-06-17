using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]Character holder;
    public Character Holder { get { return holder; } set { holder = value; 
            shieldBallSlot.Initialize(holder);
            hpBar.Initialize(holder); } }
    [SerializeField] ShieldBallSlot shieldBallSlot;
    [SerializeField] HPBar hpBar;

}
