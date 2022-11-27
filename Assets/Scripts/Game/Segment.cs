using UnityEngine;

namespace Game {
    public class Segment : MonoBehaviour {
        public float moveSpeed;

        private void FixedUpdate() {
            transform.position -= Time.fixedDeltaTime * new Vector3(0f, 0f, moveSpeed);
        }
    }
}