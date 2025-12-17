//Damage Manager

using UnityEngine;


public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance;

    [Header("Config")]
    [SerializeField] private DamageText damageTextPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowDamageText(float damageAmount, Transform parent)
    {
        DamageText text = Instantiate(damageTextPrefab, parent);
        text.transform.position += Vector3.right * 0.5f;
        text.SetDamageText(damageAmount);
    }
}
// In player health class, take damage method
// if(stats.Health <= 0f) return;
// DamageManager.Instance.ShowDamageText(amount, transform); 

// Delete eveything inside the Update method and add the following instead
// if(stats.Health <= 0f)
// { 
//    PlayerDead();
// }