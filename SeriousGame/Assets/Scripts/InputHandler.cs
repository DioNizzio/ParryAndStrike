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
        PARRY,
        DODGE
    }

    private State _current_state;
    private Moves _current_move;

    [SerializeField] private GameManager _game;

    [SerializeField] private GameObject pauseMenu;

    private Camera _mainCamera;

    PlayerInput playerInput;

    private bool _isPaused;

    private void Awake(){
        _mainCamera = Camera.main;
        playerInput = new PlayerInput();
        _current_state = State.BIGCIRCUMFERENCE;
        _current_move = Moves.IDLE;
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

    public void OnClick(InputAction.CallbackContext context){
        if(!context.started) return;

        //var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        var rayHit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector3.forward);
        if (!rayHit.collider) return;

        Debug.Log(rayHit.collider.gameObject.name);
        //make move
        if(string.Equals(rayHit.collider.gameObject.name, "BigCircumference") && _current_state == State.BIGCIRCUMFERENCE && (_current_move == Moves.SLASH || _current_move == Moves.THRUST || _current_move == Moves.PARRY))
        {
            _current_state = State.SMALLCIRCUMFERENCE;

            //send specific move
            _game.showSmallCircumferenceAndSaveFirstPoint();
            Debug.Log("In");
        }
        else if (string.Equals(rayHit.collider.gameObject.name, "SmallCircumference(Clone)") && _current_state == State.SMALLCIRCUMFERENCE && (_current_move == Moves.SLASH || _current_move == Moves.THRUST || _current_move == Moves.PARRY))
        {
            _current_state = State.BIGCIRCUMFERENCE;
            _current_move = Moves.IDLE;
            Debug.Log("2In");

            //send specific move
            _game.eraseSmallCircumferenceAndSaveSecondPoint();
        }
        else if(string.Equals(rayHit.collider.gameObject.name, "BigCircumference") && _current_state == State.SMALLCIRCUMFERENCE && (_current_move == Moves.SLASH || _current_move == Moves.THRUST || _current_move == Moves.PARRY))
        {
            _current_state = State.BIGCIRCUMFERENCE;

            //Call func from UIManager
            _game.DeleteSmallCircumference();
            
            Debug.Log("Out");
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
        _current_move = Moves.SLASH;
    }

    public void PressThrust(){
        _current_move = Moves.THRUST;
    }

    public void PressParry(){
        _current_move = Moves.PARRY;
    }

    public void PressDodge(){
        _current_move = Moves.DODGE;
    }

    //ver como fazer o player andar um pouco na direçao suposta por causa do dodge
}
