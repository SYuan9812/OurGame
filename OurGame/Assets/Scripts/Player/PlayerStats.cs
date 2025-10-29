using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Config")]
    public int Level;

    [Header("Health")]
    public float Health;
    public float MaxHealth;

    [Header("Exp")]
    public float CurrentExp;
    public float NextLevelExp;
    public float InitialNextLevelExp; //Exp needed to reach lv2 (used for initialization)
    [Range(1f, 100f)]public float ExpMultiplier;

    public void ResetPlayer()
    {
        Health = MaxHealth;
        Level = 1;
        CurrentExp = 0f;
        NextLevelExp = InitialNextLevelExp;
    }
}
