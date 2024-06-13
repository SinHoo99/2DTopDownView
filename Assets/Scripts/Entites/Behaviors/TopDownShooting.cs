using UnityEngine;

public class TopDownShooting : MonoBehaviour
{
    private TopDownController contoller;

    [SerializeField] private Transform projectileSpawnPosition;
    private Vector2 aimDirection = Vector2.right;

    [SerializeField] private AudioClip ShootingClip;

    private void Awake()
    {
        contoller = GetComponent<TopDownController>();
       
    }

    void Start()
    {
        contoller.OnAttackEvent += OnShoot;
        // OnLookEvent에 이제 두개가 등록되는 것(하나는 지난 시간에 등록했었죠? TopDownAimRotation.OnAim(Vec2)
        // 한 개의 델리게이트에 여러 개의 함수가 등록되어있는 것을 multicast delegate라고 함.
        // Action이나 Func도 델리게이트의 일종인 것 기억하시죠..?
        contoller.OnLookEvent += OnAim;
    }

    private void OnAim(Vector2 newAimDirection)
    {
        aimDirection = newAimDirection;
    }

    private void OnShoot(AttackSO attackSO)
    {
        RangedAttackSO RangedAttackSO = attackSO as RangedAttackSO;
        float projectilesAngleSpace = RangedAttackSO.multipleProjectilesAngle;
        int numberOfProjectilesPerShot = RangedAttackSO.numberofProjectilesPerShot;

        // 중간부터 펼쳐지는게 아니라 minangle부터 커지면서 쏘는 것으로 설계했어요! 
        float minAngle = -(numberOfProjectilesPerShot / 2f) * projectilesAngleSpace + 0.5f * RangedAttackSO.multipleProjectilesAngle; 


        for (int i = 0; i < numberOfProjectilesPerShot; i++)
        {
            float angle = minAngle + projectilesAngleSpace * i;
            // 그냥 올라가면 재미없으니 랜덤으로 변하는 randomSpread를 넣었어요!
            float randomSpread = Random.Range(-RangedAttackSO.spread, RangedAttackSO.spread);
            angle += randomSpread;
            CreateProjectile(RangedAttackSO, angle);
        }
    }

    private void CreateProjectile(RangedAttackSO rangedAttackSO, float angle)
    {
        // 화살 생성 -> 다음강에서 구조개선을 위해 잠시 흉물스러운 이름 참아주세요!
        GameObject obj = GameManager.Instance.Objectpool.SpawnFromPool(rangedAttackSO.bulletNameTag);

        // 발사체 기본 세팅
        obj.transform.position = projectileSpawnPosition.position;
        ProjectileController attackController = obj.GetComponent<ProjectileController>();
        attackController.InitializeAttack(RotateVector2(aimDirection, angle), rangedAttackSO);

        obj.SetActive(true);

        if (ShootingClip) SoundManager.PlayClip(ShootingClip);
    }

    private static Vector2 RotateVector2(Vector2 v, float degree)
    {
        // 벡터 회전하기 : 쿼터니언 * 벡터 순
        return Quaternion.Euler(0, 0, degree) * v;
    }
}