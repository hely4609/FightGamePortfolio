using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBallSlot : MonoBehaviour
{
    List<GameObject> shieldBallList = new List<GameObject>(); // 실드 게이지가 들어있을 리스트
    ShieldBall currentShieldBall;
    int ballNumber;
    [SerializeField]Character holder; // 캐릭터

    public void Initialize(Character holder)
    {
        this.holder = holder;
        GameObject prefab = Resources.Load<GameObject>("Prefabs/ShieldBall");
        for (int i = 0; i < this.holder.ShieldBallAmount; i++) // 가진 실드 양 만큼 실드 생성
        {
            GameObject shieldBall = Instantiate(prefab, transform);
            shieldBallList.Add(shieldBall);
        }
        currentShieldBall = SelectBall();
    }
    private void Update()
    {
        if (currentShieldBall != null && currentShieldBall.IsBreak)
        {
            holder.ShieldGauge = holder.ShieldGaugeMax;
            currentShieldBall = SelectBall();
        }
    }
    ShieldBall SelectBall()
    {
        for(int i = 0; i< shieldBallList.Count; i++)
        {
            ShieldBall ball = shieldBallList[i].GetComponent<ShieldBall>();
            if(!ball.IsBreak)
            {
                ball.IsCurrerntBall= true;
                ball.Initialize(holder);
                return ball;
            }
        }
        return null;
    }
    
}
