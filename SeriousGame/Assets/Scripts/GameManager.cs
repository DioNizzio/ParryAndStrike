using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerManager _player;

    [SerializeField] private UIManager _ui;

    private Camera _mainCamera;

    public enum Turns{
        PLAYER,
        OPPONENT
    }

    private Turns _current_turn;

    private void Awake(){
        _mainCamera = Camera.main;
        _current_turn = Turns.PLAYER;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showSmallCircumferenceAndSaveFirstPoint(){
        Vector3 mouseFirstPosition = Mouse.current.position.ReadValue();
        mouseFirstPosition.z = _mainCamera.nearClipPlane;
        var worldFirstPosition = _mainCamera.ScreenToWorldPoint(mouseFirstPosition);

        //Call func from PlayerManager
        _player.SetFirstPoint(worldFirstPosition);

        //Call func from UIManager
        _ui.InstanceSmallCircumference(worldFirstPosition);
    }

    public void eraseSmallCircumferenceAndSaveSecondPoint(){
        Vector3 mouseSecondPosition = Mouse.current.position.ReadValue();
        mouseSecondPosition.z = _mainCamera.nearClipPlane;
        var worldSecondPosition = _mainCamera.ScreenToWorldPoint(mouseSecondPosition);
        
        //Call func from PlayerManager    
        _player.SetSecondPoint(worldSecondPosition);

        //Call func from UIManager
        DeleteSmallCircumference();

    }

    public void DeleteSmallCircumference(){
        _ui.DeleteSmallCircumference();
    }

    //func to reset game and be called on yes button from exit menu
    public void ResetGame(){
        
    }
}