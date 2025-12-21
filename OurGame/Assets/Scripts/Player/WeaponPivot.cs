using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPivot : MonoBehaviour
{
    
    void Start()
    {
        
    }

    
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Getting mouse position in main camera to global position
        mousePosition.z = 0; //Ensuring no issues occuring from z rotations

        Vector3 direction = mousePosition - transform.position; //From current position to mouse position

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //Converting vector value to angle using formula; Atan2 and Rad2Deg are constants

        transform.rotation = Quaternion.Euler(0, 0, angle); //Rotating
    }
}
