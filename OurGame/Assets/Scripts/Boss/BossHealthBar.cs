using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("Reference Components")]
    [SerializeField] private Image healthFillImage;
    [SerializeField] private BossFSM bossFSM;
    [SerializeField] private BossBase bossBase;
    [SerializeField] private Image healthBackgroundImage;

    private void Awake()
    {
        if (healthFillImage == null)
        {
            healthFillImage = GetComponentInChildren<Image>();
            if (healthFillImage == null)
            {
                return;
            }
        }

        if (healthBackgroundImage == null)
        {
            healthBackgroundImage = GetComponent<Image>();
        }

        HideHealthBarVisual();
    }

    private void Update()
    {
        if (bossFSM == null || bossBase == null || healthFillImage == null) return;

        bool isShowHealthBar = bossFSM.CurrentStateType != StateType.Idle;

        if (isShowHealthBar)
        {
            ShowHealthBarVisual();
            UpdateHealthBar();
        }
        else
        {
            HideHealthBarVisual();
        }
    }

    private void ShowHealthBarVisual()
    {
        healthFillImage.enabled = true;
        if (healthBackgroundImage != null)
        {
            healthBackgroundImage.enabled = true;
        }
    }

    private void HideHealthBarVisual()
    {
        healthFillImage.enabled = false;
        if (healthBackgroundImage != null)
        {
            healthBackgroundImage.enabled = false;
        }
    }

    private void UpdateHealthBar()
    {
        if (bossBase.maxHealth <= 0)
        {
            healthFillImage.fillAmount = 0;
            return;
        }
        float healthRatio = (float)bossBase.currentHealth / bossBase.maxHealth;

        healthFillImage.fillAmount = Mathf.Clamp01(healthRatio);
    }
}