using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour //used to store references
{
    [Header("Config")]
    [SerializeField] private PlayerStats stats;

    public PlayerStats Stats => stats;
}
