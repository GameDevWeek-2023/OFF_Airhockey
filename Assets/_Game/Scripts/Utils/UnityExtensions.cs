using UnityEngine;

namespace Airhockey.Utils {
    public static class UnityExtensions {
        public static bool IsInLayerMask(this int layer, LayerMask layerMask) {
            return layerMask == (layerMask | (1 << layer));
        }
    }
}