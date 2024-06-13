using UnityEngine;

[CreateAssetMenu(fileName = "DefaultAttackSO", menuName = "TopDownController/Attacks/Default", order = 0)]
public class AttackSO : ScriptableObject
{
    // 공격에 대한 기준 데이터를 유니티 에디터 상에서 편하게 관리할 수 있어요.
    // SO로 들고 있으면 모두가 이 SO를 바라보게 되어 중복된 데이터가 여기저기 흘러다니지 않는 장점이 있어요!
    [Header("Attack Info")]
    public float size;
    public float delay;
    public float power;
    public float speed;
    public LayerMask target;

    [Header("Knock Back Info")]
    public bool isOnKnockback;
    public float knockbackPower;
    public float knockbackTime;
}