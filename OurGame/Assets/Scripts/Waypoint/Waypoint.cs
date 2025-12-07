//Waypoint
using UnityEngine;

public class Waypoint : MonoBehaviour //patrol system
{
    [Header("Config")]
    [SerializeField] private Vector3[] points; 
    //create positions where enemies are going to move

    public Vector3[] Points => points;
    public Vector3 EntityPosition {get; set; }
    private bool gameStarted;
    private void Start()
    {
        EntityPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        if(gameStarted == false && transform.hasChanged)
        {
            EntityPosition = transform.position;
        }
    }
}