using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class MinimapController : MonoBehaviour
{
    [Header("Refs")]
    public Camera mainCam;
    public Camera minimapCam;
    public MagnifierController magnifier;
    public RectTransform minimapRect;
    public RectTransform marker;
    public GameObject minimapPanel;

    [Header("Options")]
    public float padding = 1.05f;
    [Range(1f, 3f)] public float zoom = 1.4f;

    private bool fitted;
    private float fitRadius;

    private void Start()
    {
        minimapCam.enabled = false;
        minimapPanel.SetActive(false);
        TryFit();
    }

    private void LateUpdate()
    {
        bool on = magnifier != null && magnifier.IsSearchMode;

        if (on && !fitted) TryFit();

        if (minimapPanel.activeSelf != on)
        {
            minimapPanel.SetActive(on);
            minimapCam.enabled = on && fitted;
        }

        if (on && fitted)
        {
            minimapCam.orthographicSize = fitRadius / zoom;
            UpdateMarker();
        }
    }

    void UpdateMarker()
    {
        if(!magnifier.AimValid) { marker.gameObject.SetActive(false); return; }

        Vector3 vp = minimapCam.WorldToViewportPoint(magnifier.AimPoint);
        bool inside = vp.z > 0f && vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f;
        marker.gameObject.SetActive(inside);
        if (!inside) return;

        Rect r = minimapRect.rect;
        marker.anchoredPosition = new Vector2((vp.x - 0.5f) * r.width,
            (vp.y - 0.5f) * r.height);
    }

    public bool TryFit()
    {
        int mask = minimapCam.cullingMask;
        Bounds b = default;
        bool has = false;

        foreach (var r in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        {
            if (!r.enabled) continue;
            if (((1 << r.gameObject.layer) & mask) == 0) continue;

            if (!has)
            {
                b = r.bounds;
                has = true;
            }
            else
                b.Encapsulate(r.bounds);
        }

        if (!has) return false;

        float radius = b.extents.magnitude * padding;
        minimapCam.orthographic = true;
        minimapCam.orthographicSize = radius;
        minimapCam.transform.rotation = mainCam.transform.rotation;
        minimapCam.transform.position = b.center - minimapCam.transform.forward * (radius * 2f);
        minimapCam.nearClipPlane = 0.01f;
        minimapCam.farClipPlane = radius * 5f;
        fitRadius = b.extents.magnitude * padding;
        minimapCam.orthographicSize = fitRadius / zoom;
        
        fitted = true;
        
        return true;
    }

    public void Refit()
    {
        fitted = false;
        TryFit();
    }
}