using UnityEngine;

public class InputManager : MonoBehaviour
{
    private RotateController _rotateController;
    private MagnifierController _magnifier;
    private DialogueRunner _dialogue;
    
    private void Start()
    {
        _rotateController = FindFirstObjectByType<RotateController>();
        _magnifier = FindFirstObjectByType<MagnifierController>();
        _dialogue = FindFirstObjectByType<DialogueRunner>();
    }
    
    void Update()
    {
        if (_dialogue != null && _dialogue.IsActive)
        {
            HandleInput();
            return;
        }
        
        if(Input.GetMouseButtonDown(1))
            _magnifier.ToggleSearchMode();
        
        if (_magnifier != null && _magnifier.IsSearchMode)
            return;
        
        if (Input.GetKeyDown(KeyCode.A))
            _rotateController.Rotate(-90);
        if(Input.GetKeyDown(KeyCode.D))
            _rotateController.Rotate(90);
        
        // TODO : 상호작용 키 추가(ESC...), 개미 판정 시스템
    }

    private void HandleInput()
    {
        if (_dialogue.choosing)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) _dialogue.Move(-1);
            if(Input.GetKeyDown(KeyCode.DownArrow)) _dialogue.Move(1);
            if (Input.GetKeyDown(KeyCode.Return)) _dialogue.Confirm();
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                _dialogue.OnClick();
        }
    }
}
