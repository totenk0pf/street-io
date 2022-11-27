using UnityEngine;

namespace Core.System {
    public class LayerUtil {
        public static bool CompareMask(LayerMask mask, LayerMask compareMask) {
            return mask == (compareMask | (1 << mask));
        }

        public static bool CompareLayer(int layer, LayerMask compareMask) {
            return (compareMask & (1 << layer)) != 0;
        }
    }
}