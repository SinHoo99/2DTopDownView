using System;
using UnityEngine;

public class TopDownAnimationController : AnimationController
{

    private static readonly int isWalking = Animator.StringToHash("isWalking");
    private static readonly int isHit = Animator.StringToHash("isHit");
    private static readonly int Attack = Animator.StringToHash("attack");

    private static readonly float magnituteThreshold = 0.5f;
    private HealthSystem healthSystem;
    protected override void Awake()
    {
        base.Awake();
        healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        controller.OnAttackEvent += Attacking;
        controller.OnMoveEvent += Move;
        if(healthSystem != null)
        {
            healthSystem.OnDamage += Hit;
            healthSystem.OnInvincibilityEnd += InvinvibilityEnd;
        }
    }

    private void Move(Vector2 vector)
    {
        animator.SetBool(isWalking, vector.magnitude > magnituteThreshold);
    }

    private void Attacking(AttackSO sO)
    {
        animator.SetTrigger(Attack);
    }

    private void Hit()
    {
        animator.SetBool(isHit, true);
    }

    private void InvinvibilityEnd()
    {
        animator.SetBool(isHit, false);
    }
}
