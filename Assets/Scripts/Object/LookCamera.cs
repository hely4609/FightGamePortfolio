using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCamera : MonoBehaviour
{
    void Update()
    {
        // ���� ���� =  ī�޶� ���� �ٶ󺸴� ����.
        transform.rotation = Camera.main.transform.rotation;
    }
}
