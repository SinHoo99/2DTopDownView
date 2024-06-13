using System;
using UnityEngine;

public class TopDownContactEnemyController : TopDownEnemyController
{
    [SerializeField][Range(0f, 100f)] private float followRange;
    [SerializeField] private string targetTag = "Player";
    private bool isCollidingWithTarget;

    [SerializeField] private SpriteRenderer characterRenderer;

    HealthSystem healthSystem;
    private HealthSystem colldingTargetHealthSystem;
    private TopDownMovement collidingMovement;
    protected override void Start()
    {
        base.Start();
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.OnDamage += OnDamage;
    }

    private void OnDamage()
    {
        followRange = 100f;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isCollidingWithTarget)
        {
            ApplyHealthChange();
        }
        Vector2 direction = Vector2.zero;
        if (DistanceToTarget() < followRange)
        {
            direction = DirectionToTarget();
        }

        CallMoveEvent(direction);
        Rotate(direction);
    }

    private void ApplyHealthChange()
    {
        AttackSO attackSO = stats.CurrentStat.attackSO;
        bool isattackable = colldingTargetHealthSystem.ChangeHealth(-attackSO.power);

        if (isattackable && attackSO.isOnKnockback && collidingMovement != null)
        {
            collidingMovement.ApplyKnockback(transform, attackSO.knockbackPower, attackSO.knockbackTime);
        }
    }

    private void Rotate(Vector2 direction)
    {

        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        characterRenderer.flipX = Mathf.Abs(rotZ) > 90f;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject reciver = collision.gameObject;

        if (!reciver.CompareTag(targetTag))
        {
            return;
        }

        colldingTargetHealthSystem = collision.GetComponent<HealthSystem>();
        if (colldingTargetHealthSystem != null)
        {
            isCollidingWithTarget = true;
        }

        collidingMovement = collision.GetComponent<TopDownMovement>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(targetTag)) { return; }
        isCollidingWithTarget = false;
    }
}