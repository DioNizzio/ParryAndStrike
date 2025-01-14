using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour{
    public enum State{
        BIGCIRCUMFERENCE,
        SMALLCIRCUMFERENCE
    }
    public enum Moves{
        IDLE,
        SLASH,
        THRUST,
        THRUST_DEFENSE,
        PARRY,
        DODGE
    }
    public enum Dodge
    {
        IDLE,
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
    public enum BodyPart
    {
        NONE,
        HEAD,
        TORSO,
        LEFT_ARM,
        RIGHT_ARM,
        LEFT_LEG,
        RIGHT_LEG
    }

    public State current_state;
    public Moves current_move;
    public Dodge current_dodge_direction;
    public BodyPart current_body_part;

    [SerializeField] private GameManager _game;

    [SerializeField] private GameObject pauseMenu;

    private Camera _mainCamera;

    PlayerInput playerInput;

    private bool _isPaused;

    private void Awake(){
        _mainCamera = Camera.main;
        playerInput = new PlayerInput();
        current_state = State.BIGCIRCUMFERENCE;
        current_move = Moves.IDLE;
        current_dodge_direction = Dodge.IDLE;
        current_body_part = BodyPart.NONE;
        _isPaused = false;
    }

    void Update(){
        if(playerInput.Gameplay.Pause.WasPressedThisFrame()){
            if (!_isPaused){
                PauseGame();
            } else {
                UnpauseGame();
            }
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        //var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        var rayHit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector3.forward, 50.0f, LayerMask.GetMask("Circle"));
        if (!rayHit.collider) return;

        Debug.Log(rayHit.collider.gameObject.name);
        //make move
        if (string.Equals(rayHit.collider.gameObject.name, "BigCircumference") && current_state == State.BIGCIRCUMFERENCE && (current_move == Moves.SLASH || current_move == Moves.PARRY))
        {
            current_state = State.SMALLCIRCUMFERENCE;

            //send specific move
            _game.showSmallCircumferenceAndSaveFirstPoint();
            Debug.Log("In");
        }
        else if (string.Equals(rayHit.collider.gameObject.name, "SmallCircumference(Clone)") && current_state == State.SMALLCIRCUMFERENCE && current_move == Moves.SLASH)
        {
            current_state = State.BIGCIRCUMFERENCE;
            Debug.Log("2In");

            //destroy small circumference
            _game.eraseSmallCircumferenceAndSaveSecondPoint();

            //send specific move
            _game.SendDirectionToEnemy();
            current_move = Moves.IDLE;
        }
        else if (string.Equals(rayHit.collider.gameObject.name, "SmallCircumference(Clone)") && current_state == State.SMALLCIRCUMFERENCE && current_move == Moves.PARRY)
        {
            current_state = State.BIGCIRCUMFERENCE;

            //destroy small circumference
            _game.eraseSmallCircumferenceAndSaveSecondPoint();
            
            //send specific move
            _game.PlayerFinishedDefending();
            current_move = Moves.IDLE;
        }
        else if (string.Equals(rayHit.collider.gameObject.name, "BigCircumference") && current_state == State.SMALLCIRCUMFERENCE && (current_move == Moves.SLASH || current_move == Moves.PARRY))
        {
            current_state = State.BIGCIRCUMFERENCE;

            //Call func from UIManager
            _game.DeleteSmallCircumference();

            Debug.Log("Out");
        }
        else if (string.Equals(rayHit.collider.gameObject.name, "BigCircumference") && current_move == Moves.THRUST)
        {
            _game.ChoosePlayerThrustPoint();
        }
    }

    public void PauseGame(){
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        _isPaused = true;
    }

    public void UnpauseGame(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        _isPaused = false;
    }

    public void PressSlash(){
        current_move = Moves.SLASH;
    }

    public void PressThrust(){
        current_move = Moves.THRUST;
    }

    public void PressThrustDefending()
    {
        current_move = Moves.THRUST_DEFENSE;
    }

    public void PressParry(){
        current_move = Moves.PARRY;
    }

    public void PressDodge(){
        current_move = Moves.DODGE;
    }

    public void PressUpArrow()
    {
        current_dodge_direction = Dodge.UP;
        _game.GetDodgeDirection(current_dodge_direction);
    }
    public void PressDownArrow()
    {
        current_dodge_direction = Dodge.DOWN;
        _game.GetDodgeDirection(current_dodge_direction);
    }
    public void PressLeftArrow()
    {
        current_dodge_direction = Dodge.LEFT;
        _game.GetDodgeDirection(current_dodge_direction);
    }
    public void PressRightArrow()
    {
        current_dodge_direction = Dodge.RIGHT;
        _game.GetDodgeDirection(current_dodge_direction);
    }

    public void PressHead()
    {
        current_body_part = BodyPart.HEAD;
        _game.GetBodyPart(current_body_part);
    }
    public void PressTorso()
    {
        current_body_part = BodyPart.TORSO;
        _game.GetBodyPart(current_body_part);
    }
    public void PressLeftArm()
    {
        current_body_part = BodyPart.LEFT_ARM;
        _game.GetBodyPart(current_body_part);
    }
    public void PressRightArm()
    {
        current_body_part = BodyPart.RIGHT_ARM;
        _game.GetBodyPart(current_body_part);
    }
    public void PressLeftLeg()
    {
        current_body_part = BodyPart.LEFT_LEG;
        _game.GetBodyPart(current_body_part);
    }
    public void PressRightLeg()
    {
        current_body_part = BodyPart.RIGHT_LEG;
        _game.GetBodyPart(current_body_part);
    }
}
