using System;
using UnityEngine;

// Add 먼저하고, Multiple하고, 마지막에 Override하는 개념임
// enum에는 각각 정수가 매핑되어있기 때문에 가능 (0, 1, 2,...) 
// => 차후에 정렬 활용하면 오름차순으로 정렬활용해서 체계적으로 버프효과 적용순서 관리 가능
// Q: Override가 마지막에 있으면 무슨 효과가 있을까요?
// A: 공격력을 고정해야하는 특정 로직이나 기본 공격력 적용에 활용 가능
public enum StatsChangeType
{
    Add, // 0
    Multiple, // 1
    Override, // 2
}

// 클래스가 Serializable한 멤버로만 구성되어 있으면 [Serializable]을 붙여 에디터에서 확인 가능해요!
[Serializable]
public class CharacterStat
{
    public StatsChangeType statsChangeType;
    [Range(0, 100)] public int maxHealth;
    [Range(0f, 20f)] public float speed;
    public AttackSO attackSO;
}