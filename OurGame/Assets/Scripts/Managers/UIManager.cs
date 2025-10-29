using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private PlayerStats stats;

    [Header("Bars")]
    [SerializeField] private Image HBar;
    [SerializeField] private Image EBar;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI levelTMP;
    [SerializeField] private TextMeshProUGUI healthTMP;
    [SerializeField] private TextMeshProUGUI expTMP;

    void Start()
    {
        stats.ResetPlayer();
    }

    void Update()
    {
        UpdatePlayerGUI();
    }

    private void UpdatePlayerGUI ()
    {

        levelTMP.text = $"Level {stats.Level}";
        healthTMP.text = $"{stats.Health} / {stats.MaxHealth}";
        expTMP.text = $"{stats.CurrentExp} / {stats.NextLevelExp}";

        HBar.fillAmount = stats.Health / stats.MaxHealth;
        EBar.fillAmount = stats.CurrentExp / stats.NextLevelExp;
    }
}
