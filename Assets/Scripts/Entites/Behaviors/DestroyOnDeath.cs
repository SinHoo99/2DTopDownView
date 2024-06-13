using System;
using UnityEngine;

public class DestroyDeath : MonoBehaviour
{
    private HealthSystem healthSystem;
    private Rigidbody2D rb;

    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        rb = GetComponent<Rigidbody2D>();
        healthSystem.OnDeath += OnDeath;
    }

    private void OnDeath()
    {
        rb.velocity = Vector2.zero;

        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            Color color = renderer.color;
            color.a = 0.3f;
            renderer.color = color;
        }

        foreach (Behaviour behaviour in GetComponentsInChildren<Behaviour>())
        {
            behaviour.enabled = false;
        }

        Destroy(gameObject, 2f);
    }
}