using UnityEngine;

public class MagnifierController : MonoBehaviour
{
    [Header("Refs")]
    public Camera mainCam;
    public Camera magnifierCam;
    public RectTransform magnifierRoot;
    public LayerMask floorLayer;

    [Header("Options")]
    public bool hideSystemCursor = false;
    
    void Start()
    {
        magnifierRoot.gameObject.SetActive(true);
        magnifierCam.enabled = true;

        if (hideSystemCursor)
            Cursor.visible = false;
    }

    void Update()
    {
        Vector2 mouse = Input.mousePosition;
        magnifierRoot.position = mouse;
        AimMagnifierCamera(mouse);
        
        // TODO : 개미 판정 시스템
        // if (Input.GetMouseButtonDown(1))
        //     CheckAnt(mouse);
    }

    void AimMagnifierCamera(Vector2 mouse)
    {
        Vector3 wp;
        Ray ray = mainCam.ScreenPointToRay(mouse);
        if (Physics.Raycast(ray, out RaycastHit hit, 2000f, floorLayer))
            wp = hit.point;
        else
            wp = mainCam.ScreenToWorldPoint(
                new Vector3(mouse.x, mouse.y, mainCam.nearClipPlane + 20f));

        magnifierCam.transform.rotation = mainCam.transform.rotation;
        float d = Vector3.Distance(mainCam.transform.position, wp);
        magnifierCam.transform.position = wp - magnifierCam.transform.forward * d;
    }
}
