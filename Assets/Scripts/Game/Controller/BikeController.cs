using System;
using UnityEngine;

namespace Game.Controller {
    public class BikeController : MonoBehaviour {
        [SerializeField] private float maxSpeed;
        [SerializeField] private float moveSpeed;
        private Rigidbody _rb;
        private Rigidbody rb {
            get {
                if (!_rb) _rb = GetComponent<Rigidbody>();
                return _rb;
            }
        }

        private float _currentMoveSpeed;

        private void Update() {
            if (Input.GetAxisRaw("Horizontal") != 0) {
                if (_currentMoveSpeed == 0) _currentMoveSpeed += 0.1f;
                _currentMoveSpeed += _currentMoveSpeed * moveSpeed * Time.deltaTime * Mathf.Abs(Input.GetAxis("Horizontal"));
                _currentMoveSpeed =  Mathf.Clamp(_currentMoveSpeed, 0f, maxSpeed);
            } else {
                _currentMoveSpeed = 0f;
            }
        }

        private void FixedUpdate() {
            Move();
        }

        private void Move() {
            rb.MovePosition(transform.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f) * _currentMoveSpeed * Time.fixedDeltaTime);
        }
    }
}