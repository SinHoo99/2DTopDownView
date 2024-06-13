using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class CharacterStatHandler : MonoBehaviour
{
    // �⺻ ���Ȱ� ���� ���ȵ��� �ɷ�ġ�� �����ؼ� ������ ����ϴ� ������Ʈ
    [SerializeField] private CharacterStat baseStats;
    public CharacterStat CurrentStat { get; private set; } = new();
    public List<CharacterStat> statsModifiers = new List<CharacterStat>();

    private readonly float MinAttackDelay = 0.03f;
    private readonly float MinAttackPower = 0.5f;
    private readonly float MinAttackSize = 0.4f;
    private readonly float MinAttackSpeed = .1f;

    private readonly float MinSpeed = 0.8f;

    private readonly int MinMaxHealth = 5;

    private void Awake()
    {
        if (baseStats.attackSO != null)
        {
            baseStats.attackSO = Instantiate(baseStats.attackSO);
            CurrentStat.attackSO = Instantiate(baseStats.attackSO);
        }
        UpdateCharacterStat();
    }

    // �ܺο��� ���� ��ȭ ����
    public void AddStatModifier(CharacterStat statModifier)
    {
        statsModifiers.Add(statModifier);
        UpdateCharacterStat();
    }

    // Stat ��ȭ ����
    public void RemoveStatModifier(CharacterStat statModifier)
    {
        statsModifiers.Remove(statModifier);
        UpdateCharacterStat();
    }

    private void UpdateCharacterStat()
    {
        // ���̽� ���� ���� �����մϴ�.
        ApplyStatModifiers(baseStats);

        // ����Ǵ� ��ġ���� �ݿ��մϴ�.
        // �̶� statsChangeType�� ���� 0 : Add, 1 : Multiple, 2 : Override�� �ݿ��Ͽ� 0, 1, 2 ������ �����մϴ�.
        foreach (var modifier in statsModifiers.OrderBy(o => o.statsChangeType))
        {
            ApplyStatModifiers(modifier);
        }
    }

    private void ApplyStatModifiers(CharacterStat modifier)
    {
        // Func�� ������ ���׸� Ÿ���� ����ڷ������� �ϴ� ��������Ʈ�� ���մϴ�. (Action�� ������ �����ϱ��!)
        // ���� ���, Func<float, float, float>�� float 2���� �޾� float�� �����ϴ� ��������Ʈ�� ������!
        // �Ʒ� switch���� switch�� ��������, modifier.statsChangeType�� ���� ���� �Ʒ��� ���� 
        Func<float, float, float> operation = modifier.statsChangeType switch
        {
            // �Ʒ� ������ ���� �Լ��� ó���� ������, (�޼ҵ� �Ķ����) => ��� ó�� ��Ÿ���ϴ�.
            // �� modifier.statsChangeType�� Add���, operation�� current�� change �� ���� float�� �޾� current + change(���״�� Add)�ϴ� �Լ��� �����ִ� ���Դϴ�.
            // �̷��� �ϴ� ������ modifier�� Ÿ�Կ� ���� ó���ϴ� ��� ��ü�� �޶����� ������ �̸� �޼ҵ�� �Ѱ��ִ� ���̶�� �����ϸ� �ǰڽ��ϴ�!
            StatsChangeType.Add => (current, change) => current + change,
            StatsChangeType.Multiple => (current, change) => current * change,
            // _�� default case�� ���� ��Ȳ�Դϴ�. Add, Multiple�� �ƴϰ� Override��� �ǹ��ε�, ���� Ÿ���� �� �þ �� �ִٸ� �����ؾߵǰ���?
            _ => (current, change) => change,
        };

        // �̷��� �޼ҵ带 �Ķ����ó�� �Ѱ��ִ� ��! Action�� Ȱ���ؼ� �̹� ���� �غ�����?
        UpdateBasicStats(operation, modifier);

        UpdateAttackStats(operation, modifier);
        // CurrentStat.attackSO�� RangedAttackSO���� Ȯ���ϸ鼭 ���� ��� �̸� currentRanged�� �����ϴ� �����Դϴ�.
        if (CurrentStat.attackSO is RangedAttackSO currentRanged && modifier.attackSO is RangedAttackSO newRanged)
        {
            UpdateRangedAttackStats(operation, currentRanged, newRanged);
        }

        // �Ʒ��� ���� ��Ÿ���� �ڵ带 ���� ���� ��Ÿ�´ٰ� �����ϸ� �� �� �����ϴ�.
        //if (CurrentStat.attackSO is RangedAttackSO && modifier.attackSO is RangedAttackSO)
        //{
        //    UpdateRangedAttackStats(operation, CurrentStat.attackSO as RangedAttackSO, modifier.attackSO as RangedAttackSO);
        //}
    }

    private void UpdateBasicStats(Func<float, float, float> operation, CharacterStat modifier)
    {
        CurrentStat.maxHealth = Mathf.Max((int)operation(CurrentStat.maxHealth, modifier.maxHealth), MinMaxHealth);
        CurrentStat.speed = Mathf.Max(operation(CurrentStat.speed, modifier.speed), MinSpeed);
    }

    private void UpdateAttackStats(Func<float, float, float> operation, CharacterStat modifier)
    {
        if (CurrentStat.attackSO == null || modifier.attackSO == null) return;

        var currentAttack = CurrentStat.attackSO;
        var newAttack = modifier.attackSO;

        // ������ �����ϵ�, �ּҰ��� �����մϴ�.
        currentAttack.delay = Mathf.Max(operation(currentAttack.delay, newAttack.delay), MinAttackDelay);
        currentAttack.power = Mathf.Max(operation(currentAttack.power, newAttack.power), MinAttackPower);
        currentAttack.size = Mathf.Max(operation(currentAttack.size, newAttack.size), MinAttackSize);
        currentAttack.speed = Mathf.Max(operation(currentAttack.speed, newAttack.speed), MinAttackSpeed);
    }

    private void UpdateRangedAttackStats(Func<float, float, float> operation, RangedAttackSO currentRanged, RangedAttackSO newRanged)
    {
        currentRanged.multipleProjectilesAngle = operation(currentRanged.multipleProjectilesAngle, newRanged.multipleProjectilesAngle);
        currentRanged.spread = operation(currentRanged.spread, newRanged.spread);
        currentRanged.duration = operation(currentRanged.duration, newRanged.duration);
        currentRanged.numberofProjectilesPerShot = Mathf.CeilToInt(operation(currentRanged.numberofProjectilesPerShot, newRanged.numberofProjectilesPerShot));
        currentRanged.projectileColor = UpdateColor(operation, currentRanged.projectileColor, newRanged.projectileColor);
    }

    private Color UpdateColor(Func<float, float, float> operation, Color current, Color modifier)
    {
        return new Color(
            operation(current.r, modifier.r),
            operation(current.g, modifier.g),
            operation(current.b, modifier.b),
            operation(current.a, modifier.a));
    }
}