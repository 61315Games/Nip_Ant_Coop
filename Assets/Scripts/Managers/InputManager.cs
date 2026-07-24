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
            if (_dialogue.choosing)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))   _dialogue.Move(-1);
                if (Input.GetKeyDown(KeyCode.DownArrow)) _dialogue.Move(1);
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) _dialogue.Confirm();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) _dialogue.OnClick();
            }
            return;
        }

        if (TutorialController.instance != null && TutorialController.instance.IsBlocking)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                TutorialController.instance.Advance();
            return; 
        }

        if (Input.GetMouseButtonDown(1) && _magnifier != null)
            _magnifier.ToggleSearchMode();

        if (Input.GetMouseButtonDown(0))
            JudgeAnt();

        if (_magnifier != null && _magnifier.IsSearchMode)
            return;

        if (Input.GetKeyDown(KeyCode.A) && _rotateController != null) _rotateController.Rotate(-90);
        if (Input.GetKeyDown(KeyCode.D) && _rotateController != null) _rotateController.Rotate(90);
    }

    void JudgeAnt()
    {
        Camera cam = (_magnifier != null) ? _magnifier.mainCam : Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var t = hit.collider.GetComponent<Termite>();
            if (t != null) t.Judge();
        }
    }
}