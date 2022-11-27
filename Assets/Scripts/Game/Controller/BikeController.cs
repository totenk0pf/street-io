using System;
using UnityEngine;

namespace Game.Controller {
    public class BikeController : MonoBehaviour {
        [SerializeField] private float moveSpeed;
        private Rigidbody _rb;
        private Rigidbody rb {
            get {
                if (!_rb) _rb = GetComponent<Rigidbody>();
                return _rb;
            }
        }
        
        private void FixedUpdate() {
            Move();
        }

        private void Move() {
            rb.MovePosition(transform.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f) * moveSpeed * Time.fixedDeltaTime);
        }
    }
}