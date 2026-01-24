using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFollow : MonoBehaviour
{
    public Transform target;
    void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
            transform.localScale = Vector3.one;
        }
    }
}
