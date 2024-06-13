using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class CharacterStatHandler : MonoBehaviour
{
    // 기본 스탯과 버프 스탯들의 능력치를 종합해서 스탯을 계산하는 컴포넌트
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

    // 외부에서 스탯 변화 얻음
    public void AddStatModifier(CharacterStat statModifier)
    {
        statsModifiers.Add(statModifier);
        UpdateCharacterStat();
    }

    // Stat 변화 해제
    public void RemoveStatModifier(CharacterStat statModifier)
    {
        statsModifiers.Remove(statModifier);
        UpdateCharacterStat();
    }

    private void UpdateCharacterStat()
    {
        // 베이스 스텟 먼저 적용합니다.
        ApplyStatModifiers(baseStats);

        // 변경되는 수치들을 반영합니다.
        // 이때 statsChangeType의 순서 0 : Add, 1 : Multiple, 2 : Override를 반영하여 0, 1, 2 순으로 정렬합니다.
        foreach (var modifier in statsModifiers.OrderBy(o => o.statsChangeType))
        {
            ApplyStatModifiers(modifier);
        }
    }

    private void ApplyStatModifiers(CharacterStat modifier)
    {
        // Func는 마지막 제네릭 타입을 결과자료형으로 하는 델리게이트를 말합니다. (Action은 리턴이 없으니까요!)
        // 예를 들어, Func<float, float, float>는 float 2개를 받아 float를 리턴하는 델리게이트인 것이죠!
        // 아래 switch문은 switch식 구문으로, modifier.statsChangeType의 값에 따라 아래와 같이 
        Func<float, float, float> operation = modifier.statsChangeType switch
        {
            // 아래 문법은 람다 함수로 처리한 것으로, (메소드 파라미터) => 결과 처럼 나타냅니다.
            // 즉 modifier.statsChangeType이 Add라면, operation은 current와 change 두 개의 float를 받아 current + change(말그대로 Add)하는 함수를 던져주는 것입니다.
            // 이렇게 하는 이유는 modifier의 타입에 따라 처리하는 방법 자체가 달라지기 때문에 이를 메소드로 넘겨주는 것이라고 생각하면 되겠습니다!
            StatsChangeType.Add => (current, change) => current + change,
            StatsChangeType.Multiple => (current, change) => current * change,
            // _는 default case에 대한 상황입니다. Add, Multiple이 아니고 Override라는 의미인데, 만약 타입이 더 늘어날 수 있다면 조심해야되겠죠?
            _ => (current, change) => change,
        };

        // 이렇게 메소드를 파라미터처럼 넘겨주는 것! Action을 활용해서 이미 많이 해보셨죠?
        UpdateBasicStats(operation, modifier);

        UpdateAttackStats(operation, modifier);
        // CurrentStat.attackSO가 RangedAttackSO인지 확인하면서 맞을 경우 이를 currentRanged로 저장하는 문법입니다.
        if (CurrentStat.attackSO is RangedAttackSO currentRanged && modifier.attackSO is RangedAttackSO newRanged)
        {
            UpdateRangedAttackStats(operation, currentRanged, newRanged);
        }

        // 아래와 같이 나타내던 코드를 위와 같이 나타냈다고 생각하면 될 것 같습니다.
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

        // 변경을 적용하되, 최소값을 적용합니다.
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