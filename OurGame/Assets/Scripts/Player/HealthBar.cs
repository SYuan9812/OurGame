using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image HBar;
    [SerializeField] private PlayerStats stats;

    void Start()
    {
        stats.Health = stats.MaxHealth;
    }

    void Update()
    {
        BarFiller();
    }

    private void BarFiller()
    {
        HBar.fillAmount = stats.Health / stats.MaxHealth;
    }
}
