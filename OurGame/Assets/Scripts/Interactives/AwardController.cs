using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AwardController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Text")]
    public TextMeshProUGUI pickTipText;
    [Tooltip("Player tag")]
    public string playerTag = "Player";
    [Tooltip("Detect Radius")]
    public float detectRadius = 1.5f;
    [Tooltip("Interact Key")]
    public KeyCode pickKey = KeyCode.E;

    private CircleCollider2D detectCollider;
    private Chest parentChest;
    private PlayerWeaponManager playerWeaponManager;

    void Start()
    {
        if (pickTipText != null)
        {
            pickTipText.gameObject.SetActive(false);
        }
        parentChest = GetComponentInParent<Chest>();
        playerWeaponManager = GameObject.FindGameObjectWithTag(playerTag).GetComponent<PlayerWeaponManager>();

        detectCollider = GetComponent<CircleCollider2D>();
        if (detectCollider == null)
        {
            detectCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        detectCollider.isTrigger = true;
        detectCollider.radius = detectRadius;
        detectCollider.offset = Vector2.zero;

        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null) rb2d.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && pickTipText != null)
        {
            pickTipText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && pickTipText != null)
        {
            pickTipText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        transform.Translate(Vector2.up * Mathf.Sin(Time.time) * 0.1f * Time.deltaTime);

        if (pickTipText != null && pickTipText.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                PickUpAward();
            }
        }
    }

    private void PickUpAward()
    {
        if (parentChest == null || playerWeaponManager == null) return;
        WeaponData rewardWeapon = parentChest.GetRandomRewardWeapon();
        if (rewardWeapon == null) return;

        playerWeaponManager.AddNewWeapon(rewardWeapon);

        if (pickTipText != null) pickTipText.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
