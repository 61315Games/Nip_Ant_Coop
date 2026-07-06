using UnityEngine;

public class MagnifierController : MonoBehaviour
{
    [Header("Refs")]
    public Camera mainCam;
    public Camera magnifierCam;
    public RectTransform magnifierRoot;
    public LayerMask floorLayer;
    private Canvas canvas;
    public RectTransform searchLight;
    public Vector2 searchLightOffset = new Vector2(60f, -60f);

    [Header("Zoom")]
    public float normalSize = 1.5f;

    [Header("Search Mode")]
    private Vector2 normalCircleSize;
    public Vector2 searchCircleSize = new Vector2(500, 500);
    public float searchMagnification = 4f;
    public GameObject searchBackground;
    private bool isSearchMode = false;
    public bool IsSearchMode => isSearchMode;
    
    [Header("Options")]
    public bool hideSystemCursor = false;
    
    void Start()
    {
        magnifierRoot.gameObject.SetActive(true);
        magnifierCam.enabled = true;

        if (hideSystemCursor)
            Cursor.visible = false;

        canvas = magnifierRoot.GetComponentInParent<Canvas>();
        normalCircleSize = magnifierRoot.sizeDelta;
        normalSize = ComputeOneToOneSize();

        ApplyMode();
    }

    void Update()
    {
        Vector2 mouse = Input.mousePosition;
        magnifierRoot.position = mouse;
        if(searchLight) searchLight.position = mouse + searchLightOffset;
        AimMagnifierCamera(mouse);
    }

    void ApplyMode()
    {
        if(searchBackground)
            searchBackground.SetActive(isSearchMode);
        
        magnifierRoot.sizeDelta = isSearchMode ? searchCircleSize : normalCircleSize;
        magnifierCam.orthographicSize = isSearchMode ? ComputeSearchSize() : normalSize;
    }

    public void ToggleSearchMode()
    {
        isSearchMode = !isSearchMode;
        ApplyMode();
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

    float ComputeOneToOneSize()
    {
        float circlePx = magnifierRoot.rect.height * canvas.scaleFactor;
        return mainCam.orthographicSize * (circlePx / Screen.height);
    }

    float ComputeSearchSize()
    {
        float circlePx = searchCircleSize.y * canvas.scaleFactor;
        return mainCam.orthographicSize * (circlePx / Screen.height) / searchMagnification;
    }
}
