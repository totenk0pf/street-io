using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game {
    [Serializable]
    public struct RopeSegment {
        public Vector3 prevPos;
        public Vector3 currPos;

        public RopeSegment(Vector3 pos) {
            prevPos = pos;
            currPos = pos;
        }
    }
    
    [RequireComponent(typeof(LineRenderer))]
    public class RopeBehaviour : MonoBehaviour {
        [ReadOnly] public Vector3 start;
        [ReadOnly] public Vector3 end;
        
        [SerializeField] private float segmentLength;
        
        private LineRenderer _renderer;
        private List<RopeSegment> _segments = new ();
        
        private const float _debugSphereRadius = 0.2f;

        private void Awake() {
            _renderer = GetComponent<LineRenderer>();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(start, _debugSphereRadius);
            Gizmos.DrawWireSphere(end, _debugSphereRadius);
        }
#endif
    }
}