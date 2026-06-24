using UnityEngine;

public class InputManager : MonoBehaviour
{
    private RotateController _rotateController;
    private void Start()
    {
        _rotateController = FindFirstObjectByType<RotateController>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            _rotateController.Rotate(-90);
        if(Input.GetKeyDown(KeyCode.D))
            _rotateController.Rotate(90);
        
        // TODO : 상호작용 키 추가(Spacebar, ESC...)
    }
}
