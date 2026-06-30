using UnityEngine;

public class MagnifierController : MonoBehaviour
{
    [Header("Refs")]
    public Camera mainCam;
    public Camera magnifierCam;
    public RectTransform magnifierRoot;
    public LayerMask floorLayer;

    private bool isZoomed = false;
    
    void Start()
    {
        SetZoom(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            SetZoom(!isZoomed);

        if (isZoomed)
        {
            Vector2 mouse = Input.mousePosition;
            magnifierRoot.position = mouse;
            AimMagnifierCamera(mouse);
        }
    }

    void SetZoom(bool on)
    {
        isZoomed = on;
        magnifierRoot.gameObject.SetActive(on);
        magnifierCam.enabled = on;
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
