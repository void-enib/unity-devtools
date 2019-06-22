using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Void.devtools
{
    public static class Gizmos2D
    {
        public static void DrawCircle(Vector3 centerPosition, float radius)
        {
            float theta = 0;
            Vector3 newPos;
            Vector3 lastPos = new Vector3(radius, 0);
            for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
            {
                newPos = radius * new Vector3(Mathf.Cos(theta), Mathf.Sin(theta));
                Gizmos.DrawLine(centerPosition + lastPos, centerPosition + newPos);
                lastPos = newPos;
            }
            Gizmos.DrawLine(centerPosition + lastPos, centerPosition + new Vector3(radius, 0));
        }
    }
}