using System.Collections;
using UnityEngine;

public class RotateController : MonoBehaviour
{
    public float rotationDuration = 0.3f;

    private bool isRotating = false;

    public void Rotate(float angle)
    {
        if (isRotating) return;
        StartCoroutine(RotateWorld(angle));
    }

    IEnumerator RotateWorld(float angle)
    {
        isRotating = true;

        float elapsed = 0f;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, angle, 0);

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRot, endRot, elapsed / rotationDuration);
            yield return null;
        }

        transform.rotation = endRot;
        isRotating = false;
    }
}
