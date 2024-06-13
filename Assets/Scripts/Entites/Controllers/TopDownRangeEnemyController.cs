using UnityEngine;

public class TopDownRangeEnemyController : TopDownEnemyController
{
    [SerializeField] private float followRange = 15f;
    [SerializeField] private float shootRange = 10f;
    private int layerMaskLevel;
    private int layerMaskTarget;

    protected override void Start()
    {
        base.Start();
        layerMaskLevel = LayerMask.NameToLayer("Level");
        layerMaskTarget = stats.CurrentStat.attackSO.target;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        float distanceToTarget = DistanceToTarget();
        Vector2 directionToTarget = DirectionToTarget();

        UpdateEnemyState(distanceToTarget, directionToTarget);
    }

    private void UpdateEnemyState(float distance, Vector2 direction)
    {
        isAttacking = false; // 기본적으로 공격 상태를 false로 설정합니다.

        if (distance <= followRange)
        {
            CheckIfNear(distance, direction);
        }
    }

    private void CheckIfNear(float distance, Vector2 direction)
    {
        if (distance <= shootRange)
        {
            TryShootAtTarget(direction);
        }
        else
        {
            CallMoveEvent(direction); // 사정거리 밖이지만 추적 범위 내에 있을 경우, 타겟 쪽으로 이동합니다.
        }
    }

    private void TryShootAtTarget(Vector2 direction)
    {
        // 몬스터 위치에서 direction 방향으로 레이를 발사합니다.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, shootRange, GetLayerMaskForRaycast());

        // 벽에 맞은게 아니라 실제 플레이어에 맞았는지 확인합니다.
        if (IsTargetHit(hit))
        {
            PerformAttackAction(direction);
        }
        else
        {
            CallMoveEvent(direction);
        }
    }

    private int GetLayerMaskForRaycast()
    {
        // "Level" 레이어와 타겟 레이어 모두를 포함하는 LayerMask를 반환합니다.
        return (1 << layerMaskLevel) | layerMaskTarget;
    }

    private bool IsTargetHit(RaycastHit2D hit)
    {
        // RaycastHit2D 결과를 바탕으로 실제 타겟을 명중했는지 확인합니다.
        return hit.collider != null && layerMaskTarget == (layerMaskTarget | (1 << hit.collider.gameObject.layer));
    }

    private void PerformAttackAction(Vector2 direction)
    {
        // 타겟을 정확히 명중했을 경우의 행동을 정의합니다.
        CallLookEvent(direction);
        CallMoveEvent(Vector2.zero); // 공격 중에는 이동을 멈춥니다.
        isAttacking = true;
    }
}
