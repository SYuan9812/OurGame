using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Animator")]
    public Animator chestAnimator;
    [Tooltip("Player Tag")]
    public string playerTag = "Player";
    [Tooltip("Random Weapons")]
    public List<WeaponData> rewardWeapons;
    [Tooltip("Award")]
    public GameObject awardObject;
    [Tooltip("Award Renderer")]
    public SpriteRenderer awardSpriteRenderer;

    [Header("Award Scale")]
    [Tooltip("Award Basic")]
    public Vector3 awardBaseScale = new Vector3(1f, 1f, 1f);
    [Tooltip("Brick")]
    public Vector3 brickScaleOffset = new Vector3(0.5f, 0.5f, 1f);
    [Tooltip("Flashlight")]
    public Vector3 flashlightScaleOffset = new Vector3(1f, 1f, 1f);

    [Header("Animator Parameter")]
    [Tooltip("Trigger Name")]
    public string openTriggerName = "OpenChest";


    private bool isChestOpened = false;
    private WeaponData randomRewardWeapon;

    void Start()
    {
        if (awardObject != null) awardObject.SetActive(false);

        if (chestAnimator == null)
        {
            chestAnimator = GetComponent<Animator>();
            if (chestAnimator == null)
            {
                enabled = false;
                return;
            }
        }

        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider != null && !circleCollider.isTrigger)
        {
            circleCollider.isTrigger = true;
        }


        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null && rb2d.bodyType != RigidbodyType2D.Kinematic)
        {
            rb2d.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isChestOpened || !other.CompareTag(playerTag)) return;
        OpenChest();
    }

    private void OpenChest()
    {
        isChestOpened = true;

        chestAnimator.SetTrigger(openTriggerName);

        RandomGenerateRewardWeapon();
        if (awardObject != null) awardObject.SetActive(true);
        SetAwardSpriteByWeapon();
        SetAwardScaleByWeapon();

        GiveRewardToPlayer();
    }

    private void RandomGenerateRewardWeapon()
    {
        if (rewardWeapons == null || rewardWeapons.Count == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, rewardWeapons.Count);
        randomRewardWeapon = rewardWeapons[randomIndex];
    }

    private void SetAwardSpriteByWeapon()
    {
        if (randomRewardWeapon == null || awardSpriteRenderer == null) return;
        if (randomRewardWeapon.weaponModelPrefab != null)
        {
            SpriteRenderer weaponSprite = randomRewardWeapon.weaponModelPrefab.GetComponent<SpriteRenderer>();
            if (weaponSprite != null && weaponSprite.sprite != null)
            {
                awardSpriteRenderer.sprite = weaponSprite.sprite;
            }
        }
    }

    private void SetAwardScaleByWeapon()
    {
        if (awardObject == null || randomRewardWeapon == null) return;
        Vector3 finalScale = awardBaseScale;
        if (randomRewardWeapon.weaponName.Contains("Brick"))
        {
            finalScale = Vector3.Scale(awardBaseScale, brickScaleOffset);
        }
        else if (randomRewardWeapon.weaponName.Contains("Flashlight"))
        {
            finalScale = Vector3.Scale(awardBaseScale, flashlightScaleOffset);
        }
        // More weapons add here
        awardObject.transform.localScale = finalScale;
    }

    private void GiveRewardToPlayer()
    {
        
    }

    public WeaponData GetRandomRewardWeapon()
    {
        return randomRewardWeapon;
    }
}
