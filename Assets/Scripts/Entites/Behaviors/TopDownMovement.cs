using UnityEngine;

// TopDownMovement는 캐릭터와 몬스터의 이동에 사용될 예정입니다.
public class TopDownMovement : MonoBehaviour
{
    private TopDownController movementController;
    private CharacterStatHandler characterStatHandler;
    private Rigidbody2D movementRigidbody;

    private Vector2 _movementDirection = Vector2.zero;

    private Vector2  knockback = Vector2.zero;
    private float knockbackDuration = 0.0f;

    private void Awake()
    {
        // 같은 게임오브젝트의 TopDownController, Rigidbody를 가져올 것 
        movementController = GetComponent<TopDownController>();
        movementRigidbody = GetComponent<Rigidbody2D>();
        characterStatHandler = GetComponent<CharacterStatHandler>();
    }

    private void Start()
    {
        // OnMoveEvent에 Move를 호출하라고 등록함
        movementController.OnMoveEvent += Move;
    }

    private void FixedUpdate()
    {
        // 물리 업데이트에서 움직임 적용
        ApplyMovement(_movementDirection);

        if(knockbackDuration > 0.0f)
        {
            knockbackDuration -= Time.fixedDeltaTime;
        }
    }

    private void Move(Vector2 direction)
    {
        // 이동방향만 정해두고 실제로 움직이지는 않음.
        // 움직이는 것은 물리 업데이트에서 진행(rigidbody가 물리니까)
        _movementDirection = direction;
    }

    public void ApplyKnockback(Transform other, float power, float duration)
    {
        knockbackDuration = duration;
        knockback = -(other.position - transform.position).normalized * power;
    }

    private void ApplyMovement(Vector2 direction)
    {
        // 5라는 기본 스피드 숫자가 아니라 SO값을 참조하도록 변경했음
        direction = direction * characterStatHandler.CurrentStat.speed;

        if(knockbackDuration > 0f)
        {
            direction *= knockback;
        }

        movementRigidbody.velocity = direction;
    }
}