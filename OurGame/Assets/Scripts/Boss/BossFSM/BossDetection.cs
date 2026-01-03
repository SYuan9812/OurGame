using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDetection : MonoBehaviour
{
    private BossFSM bossFSM;

    void Awake()
    {
        bossFSM = transform.parent.GetComponent<BossFSM>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bossFSM.parameter.target = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bossFSM.parameter.target = null;
        }
    }
}
