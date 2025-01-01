using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour{
    public enum State{
        BIGCIRCUMFERENCE,
        SMALLCIRCUMFERENCE
    }

    private State _current_state;

    [SerializeField] private PlayerManager _player;

    [SerializeField] private UIManager _ui;

    private Camera _mainCamera;

    private void Awake(){
        _mainCamera = Camera.main;
        _current_state = State.BIGCIRCUMFERENCE;
    }

    public void OnClick(InputAction.CallbackContext context){
        if(!context.started) return; //add OR can't attack

        var rayHit = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if(!rayHit.collider) return;

        if(string.Equals(rayHit.collider.gameObject.name, "BigCircumference") && _current_state == State.BIGCIRCUMFERENCE)
        {
            _current_state = State.SMALLCIRCUMFERENCE;
            
            Vector3 mouseFirstPosition = Mouse.current.position.ReadValue();
            mouseFirstPosition.z = _mainCamera.nearClipPlane;
            var worldFirstPosition = _mainCamera.ScreenToWorldPoint(mouseFirstPosition);

            //Call func from PlayerManager
            _player.SetFirstPoint(worldFirstPosition);

            //Call func from UIManager
            _ui.InstanceSmallCircumference(worldFirstPosition);
        }
        else if(string.Equals(rayHit.collider.gameObject.name, "BigCircumference") && _current_state == State.SMALLCIRCUMFERENCE)
        {
            _current_state = State.BIGCIRCUMFERENCE;

            //Call func from UIManager
            _ui.DeleteSmallCircumference();
        }
        else if(string.Equals(rayHit.collider.gameObject.name, "SmallCircumference") && _current_state == State.SMALLCIRCUMFERENCE)
        {
            Vector3 mouseSecondPosition = Mouse.current.position.ReadValue();
            mouseSecondPosition.z = _mainCamera.nearClipPlane;
            var worldSecondPosition = _mainCamera.ScreenToWorldPoint(mouseSecondPosition);
         
            //Call func from PlayerManager    
            _player.SetSecondPoint(worldSecondPosition);

            _current_state = State.BIGCIRCUMFERENCE;

            //Call func from UIManager
            _ui.DeleteSmallCircumference();
        }
        
    }
}
