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
        // ================= 스토리 씬 =================
        if (_dialogue != null)
        {
            if (!_dialogue.IsActive) return;

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

        // ================= 게임 씬 =================
        // 오른쪽 클릭 = 확대 진입/해제
        if (Input.GetMouseButtonDown(1) && _magnifier != null)
            _magnifier.ToggleSearchMode();

        // 좌클릭 = 개미 판정 (확대 여부 상관없이 항상)
        if (Input.GetMouseButtonDown(0))
            JudgeAnt();

        // 확대 모드 중엔 회전 막기
        if (_magnifier != null && _magnifier.IsSearchMode)
            return;

        // 큐브 회전
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