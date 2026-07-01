using UnityEngine;

public class InputManager : MonoBehaviour
{
    private RotateController _rotateController;
    private MagnifierController _magnifier;
    
    private void Start()
    {
        _rotateController = FindFirstObjectByType<RotateController>();
        _magnifier = FindFirstObjectByType<MagnifierController>();
    }
    
    void Update()
    {
        if(Input.GetMouseButtonDown(1))
            _magnifier.ToggleSearchMode();
        
        if (_magnifier != null && _magnifier.IsSearchMode)
            return;
        
        if (Input.GetKeyDown(KeyCode.A))
            _rotateController.Rotate(-90);
        if(Input.GetKeyDown(KeyCode.D))
            _rotateController.Rotate(90);
        
        // TODO : 상호작용 키 추가(Spacebar, ESC...), 개미 판정 시스템
    }
}
