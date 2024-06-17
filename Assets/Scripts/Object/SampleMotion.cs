using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleMotion : MonoBehaviour
{
    SkeletonAnimation skeletonAnimation;
    private void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }
    private void Update()
    {
        skeletonAnimation.Update(Time.unscaledDeltaTime);
       
    }
}
