using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class AntMonologue : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [Header("Timing")]
    [SerializeField] private float showTime = 3f;
    [SerializeField] private float hideTime = 3f;
    [SerializeField] private float maxStartDelay = 2f;

    [Header("Occlusion")]
    [SerializeField] private LayerMask occluderMask;
    [SerializeField] private float rayBackDistance = 100f;
    [SerializeField] private float skin = 0.2f;
    [SerializeField] private float checkInterval = 0.1f; 

    private Coroutine loop;
    private bool wantVisible;
    private bool notOccluded = true;
    private float nextCheck;
    private Transform cam;

    private void Awake()
    {
        if(label == null) label = GetComponentInChildren<TMP_Text>(true);
        if(label != null) label.gameObject.SetActive(false);
    }
    
    void Start()
    {
        cam = Camera.main != null ? Camera.main.transform : null;
    }
    
    void LateUpdate()
    {
        if (label == null) return;

        if (Time.time >= nextCheck)
        {
            nextCheck = Time.time + checkInterval;
            notOccluded = !IsOccluded();
        }

        bool visible = wantVisible && notOccluded;
        if (label.gameObject.activeSelf != visible)
            label.gameObject.SetActive(visible);
    }

    public void Begin(string line)
    {
        if(label == null || string.IsNullOrEmpty(line)) return;

        label.text = line;
        if(loop != null) StopCoroutine(loop);
        loop = StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        yield return new WaitForSeconds(Random.Range(0f, maxStartDelay));

        while (true)
        {
            wantVisible = true;
            yield return new WaitForSeconds(showTime);
            wantVisible = false;
            yield return new WaitForSeconds(hideTime);
        }
    }
    
    private bool IsOccluded()
    {
        if (cam == null) return false;

        Vector3 dir    = cam.forward;
        Vector3 origin = transform.position - dir * rayBackDistance;
        float   dist   = rayBackDistance - skin;

        return Physics.Raycast(origin, dir, dist, occluderMask,
            QueryTriggerInteraction.Ignore);
    }
}
