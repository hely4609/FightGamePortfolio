using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCamera : MonoBehaviour
{
    void Update()
    {
        // 나의 방향 =  카메라가 나를 바라보는 방향.
        transform.rotation = Camera.main.transform.rotation;
    }
}
