using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collider2DChange : MonoBehaviour
{
    private PolygonCollider2D poly;
    private SpriteRenderer renderer1;
    private Sprite lastSprite;

    void Awake()
    {
        if (!TryGetComponent(out poly))
        {
            poly = gameObject.AddComponent<PolygonCollider2D>();
        }

        renderer1 = GetComponent<SpriteRenderer>();
        lastSprite = renderer1.sprite;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (renderer1.sprite != lastSprite) {
            PolygonCollider2D lastPoly = gameObject.AddComponent<PolygonCollider2D>();
            poly.points = lastPoly.points;
            Destroy(lastPoly);

            lastSprite = renderer1.sprite;
        }
    }
}
