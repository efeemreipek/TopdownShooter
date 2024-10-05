using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private GameControls gameControls;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction sprintAction;
    private InputAction fireAction;
    private InputAction reloadAction;
    private InputAction aimAction;

    protected override void Awake()
    {
        base.Awake();

        gameControls = new GameControls();
    }

    private void OnEnable()
    {
        moveAction = gameControls.Player.Move;
        lookAction = gameControls.Player.Look;
        sprintAction = gameControls.Player.Sprint;
        fireAction = gameControls.Player.Fire;
        reloadAction = gameControls.Player.Reload;
        aimAction = gameControls.Player.Aim;

        gameControls.Enable();
    }
    private void OnDisable()
    {
        gameControls.Disable();
    }

    public Vector2 GetMoveInputVector() => moveAction.ReadValue<Vector2>();
    public Vector2 GetLookInputVector() => lookAction.ReadValue<Vector2>();
    public bool GetIsSprintPressed() => sprintAction.IsPressed();
    public bool GetIsFirePressed() => fireAction.IsPressed();
    public bool GetIsFirePressedThisFrame() => fireAction.WasPressedThisFrame();
    public bool GetIsReloadPressedThisFrame() => reloadAction.WasPressedThisFrame();
    public bool GetIsAimPressed() => aimAction.IsPressed();
}
