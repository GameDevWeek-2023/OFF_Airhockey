using System;
using UnityEngine;

namespace Airhockey.Utils {
    public static class UnityExtensions {
        public static bool IsInLayerMask(this int layer, LayerMask layerMask) {
            return layerMask == (layerMask | (1 << layer));
        }

        public static Vector3 DirFromAngle(this Transform transform, float angleInDegrees, bool angleIsGlobal) {
            if (!angleIsGlobal) {
                angleInDegrees += transform.eulerAngles.y;
            }

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        public static Vector2 PointOnCircle(float radius, float angleInDegrees, Vector2 origin) {
            float x = (radius * Mathf.Cos(angleInDegrees * Mathf.PI / 180F)) + origin.x;
            float y = (radius * Mathf.Sin(angleInDegrees * Mathf.PI / 180F)) + origin.y;

            return new Vector2(x, y);
        }
    }
}