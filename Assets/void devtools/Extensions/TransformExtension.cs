using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Void.devtools
{
    public static class TransformExtension
    {
        public static void DestroyChildren(this Transform transform)
        {
            foreach (Transform child in transform)
                Object.Destroy(child.gameObject);
        }

        public static void DestroyChildren(this Transform transform, float time)
        {
            foreach (Transform child in transform)
                Object.Destroy(child.gameObject, time);
        }
    }
}