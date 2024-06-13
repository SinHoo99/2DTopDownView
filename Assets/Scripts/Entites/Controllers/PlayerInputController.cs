using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerInputController : TopDownController
{
    private Camera _camera;
    protected override void Awake()
    {
        // 부모의 Awake도 빼먹지 말고 실행하라는 의미
        base.Awake();
        _camera = Camera.main;
    }

    public void OnMove(InputValue value)
    {
        // Debug.Log("OnMove" + value.ToString());
        Vector2 moveInput = value.Get<Vector2>().normalized;
        CallMoveEvent(moveInput);
    }

    public void OnLook(InputValue value)
    {
        // Debug.Log("OnLook" + value.ToString());
        Vector2 newAim = value.Get<Vector2>();
        Vector2 worldPos = _camera.ScreenToWorldPoint(newAim);
        newAim = (worldPos - (Vector2)transform.position).normalized;

        if (newAim.magnitude >= .9f)
        // Vector 값을 실수로 변환
        {
            CallLookEvent(newAim);
        }
    }

    public void OnFire(InputValue value)
    {
        isAttacking = value.isPressed;
    }
}