using System.Collections;
using UnityEngine;

public class RotateController : MonoBehaviour
{
    public float rotationDuration = 0.3f;

    private Transform pivot;
    private bool isRotating = false;

    void Awake()
    {
        var rends = GetComponentsInChildren<Renderer>();
        if (rends.Length > 0)
        {
            Bounds b = rends[0].bounds;
            foreach (var r in rends) b.Encapsulate(r.bounds);
            GameObject p = new GameObject("AutoPivot");
            p.transform.position = b.center;
            pivot = p.transform;
        }
    }

    public void Rotate(float angle)
    {
        if (isRotating) return;
        StartCoroutine(RotateWorld(angle));
    }

    IEnumerator RotateWorld(float angle)
    {
        isRotating = true;
        float elapsed = 0f, rotated = 0f;

        Vector3 center = pivot.position;
        Vector3 axis = transform.forward; 

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / rotationDuration);
            float target = Mathf.Lerp(0f, angle, t);
            transform.RotateAround(center, axis, target - rotated);
            rotated = target;
            yield return null;
        }

        transform.RotateAround(center, axis, angle - rotated);
        isRotating = false;
    }
}