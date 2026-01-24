using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDataPersistence : MonoBehaviour
{
    public static WeaponDataPersistence Instance;
    public List<WeaponData> persistentWeaponList = new List<WeaponData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitPersistentList(WeaponData initWeapon)
    {
        if (persistentWeaponList.Count == 0 && initWeapon != null)
        {
            persistentWeaponList.Add(initWeapon);
        }
    }

    public void AddWeaponToPersistentList(WeaponData newWeapon)
    {
        if (newWeapon == null || persistentWeaponList.Contains(newWeapon)) return;
        persistentWeaponList.Add(newWeapon);
    }

    public void ClearPersistentList()
    {
        persistentWeaponList.Clear();
    }
}
