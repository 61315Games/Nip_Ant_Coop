using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Renderer))]
public class RandomBackground : MonoBehaviour
{
   [SerializeField] private Texture2D[] backgrounds;

   private void Awake()
   {
      if (backgrounds == null || backgrounds.Length == 0) return;

      int index = Random.Range(0, backgrounds.Length);
      GetComponent<Renderer>().material.mainTexture = backgrounds[index];
   }
}
