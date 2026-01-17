using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Weapon Data")]
    public List<WeaponData> weaponList = new List<WeaponData>();
    public int defaultWeaponIndex = 0;
    public Transform attackSpawnPoint;

    [SerializeField] private GameObject brickWeaponObject; 
    [SerializeField] private GameObject flashlightWeaponObject; 

    [Header("Weapon Brick Anim")]
    public Animator brickWeaponAnim;
    public Animator brickSlashEffectAnim;
    public Animator playerAnimator;

    [Header("Flashlight Anim")]
    [SerializeField] private Animator flashlightWeaponAnim;

    [Header("Flashlight Mouse Direction")]
    [SerializeField] private Camera mainCamera; 
    [SerializeField] private Transform flashlightTransform; 
    [SerializeField] private float rayDepth = 10f; 

    [Header("Switching Weapon")]
    public KeyCode[] weaponSwitchKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };
    public Text currentWeaponUIText;

    private WeaponData currentWeapon;
    private int currentWeaponIndex;
    private float lastAttackTime;

    void Start()
    {
        InitWeaponSystem();
    }

    void Update()
    {
        CheckWeaponSwitchInput();
        CheckAttackInput();
        RotateFlashlightToMouse();
    }

    private void InitWeaponSystem()
    {
        if (weaponList.Count == 0)
        {
            enabled = false;
            return;
        }

        defaultWeaponIndex = Mathf.Clamp(defaultWeaponIndex, 0, weaponList.Count - 1);
        EquipWeapon(defaultWeaponIndex);
        ControlWeaponVisibility();
        lastAttackTime = -currentWeapon.attackCooldown;
        UpdateWeaponUI();
    }

    private void CheckWeaponSwitchInput()
    {
        for (int i = 0; i < weaponSwitchKeys.Length; i++)
        {
            if (i >= weaponList.Count) break;

            if (Input.GetKeyDown(weaponSwitchKeys[i]))
            {
                EquipWeapon(i);
                UpdateWeaponUI();
                break;
            }
        }
    }

    private void CheckAttackInput()
    {
        if (currentWeapon == null) return;

        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + currentWeapon.attackCooldown)
        {
            PerformWeaponAttack();
        }
    }

    private void EquipWeapon(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= weaponList.Count)
        {
            return;
        }

        currentWeaponIndex = weaponIndex;
        currentWeapon = weaponList[currentWeaponIndex];

        if (currentWeapon.weaponAnimatorOverride != null && playerAnimator != null)
        {
            AnimatorOverrideController overrideController = new AnimatorOverrideController(playerAnimator.runtimeAnimatorController);
            overrideController = currentWeapon.weaponAnimatorOverride;
            playerAnimator.runtimeAnimatorController = overrideController;
        }

        ControlWeaponVisibility();
    }


    private void ControlWeaponVisibility()
    {
        if (brickWeaponObject != null)
        {
            brickWeaponObject.SetActive(false);
        }
        if (flashlightWeaponObject != null)
        {
            flashlightWeaponObject.SetActive(false);
        }

        if (currentWeapon == null) return;

        if (currentWeapon.weaponName.Contains("Brick"))
        {
            if (brickWeaponObject != null)
            {
                brickWeaponObject.SetActive(true);
            }
        }
        else if (currentWeapon.weaponName.Contains("Flashlight"))
        {
            if (flashlightWeaponObject != null)
            {
                flashlightWeaponObject.SetActive(true);
            }
        }
    }


    private void PerformWeaponAttack()
    {
        if (currentWeapon == null || attackSpawnPoint == null) return;
        lastAttackTime = Time.time;

        if (currentWeapon.weaponName.Contains("Brick"))
        {
            TriggerBrickWeaponDualAnim();
            SpawnAttackRange();
        }
        else if (currentWeapon.weaponName.Contains("Flashlight"))
        {
            TriggerFlashlightAttackAnim();
            SpawnLightBall();
        }
    }

    private void TriggerFlashlightAttackAnim()
    {
        if (currentWeapon == null || flashlightWeaponAnim == null) return;

        string attackTrigger = currentWeapon.attackTriggerName;
        flashlightWeaponAnim.SetTrigger(attackTrigger);
    }

    private void RotateFlashlightToMouse()
    {
        if (mainCamera == null || flashlightTransform == null || currentWeapon == null) return;
        if (!currentWeapon.weaponName.Contains("Flashlight")) return;

        Vector2 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, rayDepth));
        Vector3 direction = mouseWorldPos - flashlightTransform.position;
        direction.z = 0;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        flashlightTransform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    private void SpawnLightBall()
    {
        if (currentWeapon == null || currentWeapon.lightBallPrefab == null || attackSpawnPoint == null || mainCamera == null)
        {
            return;
        }

        GameObject lightBall = Instantiate(
            currentWeapon.lightBallPrefab,
            attackSpawnPoint.position,
            Quaternion.identity,
            null
        );

        Vector2 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, rayDepth));
        Vector3 direction = mouseWorldPos - attackSpawnPoint.position;
        direction.z = 0;
        direction.Normalize();

        Rigidbody2D rb2d = lightBall.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.velocity = direction * currentWeapon.lightBallSpeed;
        }
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            lightBall.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        Destroy(lightBall, currentWeapon.lightBallLifetime);

        PlayerAttackTrigger lightBallTrigger = lightBall.GetComponent<PlayerAttackTrigger>();
        if (lightBallTrigger != null)
        {
            lightBallTrigger.Init(currentWeapon);
        }
    }

    private void TriggerBrickWeaponDualAnim()
    {
        string attackTrigger = currentWeapon.attackTriggerName;
        if (brickWeaponAnim != null) brickWeaponAnim.SetTrigger(attackTrigger);
        if (brickSlashEffectAnim != null) brickSlashEffectAnim.SetTrigger(attackTrigger);
        if (playerAnimator != null) playerAnimator.SetTrigger(attackTrigger);
    }

    private void SpawnAttackRange()
    {
        if (currentWeapon.attackRangePrefab == null)
        {
            return;
        }
        GameObject attackRange = Instantiate(
            currentWeapon.attackRangePrefab,
            attackSpawnPoint.position,
            attackSpawnPoint.rotation,
            null
        );

        PlayerAttackTrigger attackTrigger = attackRange.GetComponent<PlayerAttackTrigger>();
        if (attackTrigger != null)
        {
            attackTrigger.Init(currentWeapon);
            Destroy(attackRange, currentWeapon.attackDuration);
        }
        else
        {
            Destroy(attackRange, currentWeapon.attackDuration);
        }
    }

    private void UpdateWeaponUI()
    {
        if (currentWeaponUIText != null && currentWeapon != null)
        {
            currentWeaponUIText.text = $"Current Weapon£º{currentWeapon.weaponName}";
        }
    }

    public WeaponData GetCurrentWeapon() => currentWeapon;
    public int GetCurrentWeaponIndex() => currentWeaponIndex;
}