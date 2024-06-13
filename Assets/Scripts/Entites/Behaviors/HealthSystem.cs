using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float healthChangeDelay = 0.5f;
    [SerializeField] private AudioClip DamageClip;
    private CharacterStatHandler statHandler;
    private float timeSinceLastChange = float.MaxValue;
    private bool isAttcked = false;
    public event Action OnDamage;
    public event Action OnHeal;
    public event Action OnDeath;
    public event Action OnInvincibilityEnd;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => statHandler.CurrentStat.maxHealth;

    private void Awake()
    {
        statHandler = GetComponent<CharacterStatHandler>();
    }
    private void Start()
    {
        CurrentHealth = MaxHealth;

    }

    private void Update()
    {
        if (isAttcked && timeSinceLastChange < healthChangeDelay)
        {
            timeSinceLastChange += Time.deltaTime;
            if (timeSinceLastChange >= healthChangeDelay)
            {
                OnInvincibilityEnd?.Invoke();
                isAttcked = false;
            }
        }
    }

    public bool ChangeHealth(float change)
    {
        if (timeSinceLastChange < healthChangeDelay)
        {
            //공격을 하지 않고 끝나는 상황
            return false;
        }

        timeSinceLastChange = 0f;
        CurrentHealth += change;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

        if (CurrentHealth <= 0f)
        {
            CallDeath();
            return true;
        }
        if(change >= 0f)
        {
            OnHeal?.Invoke();
        }
        else
        {
            OnDamage?.Invoke();
            isAttcked = true;

            if (DamageClip) SoundManager.PlayClip(DamageClip);
        }

        return true;
    }

    private void CallDeath()
    {
        OnDeath?.Invoke();
    }
}
