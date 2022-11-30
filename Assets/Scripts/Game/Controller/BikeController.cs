using System;
using Core.Events;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Game.Controller {
    public class BikeController : MonoBehaviour {
        [SerializeField] private float maxSpeed;
        [SerializeField] private float acceleration;
        [SerializeField] private float deceleration;
        [SerializeField] private Transform camPos;
        
        private Rigidbody _rb;
        private Rigidbody rb {
            get {
                if (!_rb) _rb = GetComponent<Rigidbody>();
                return _rb;
            }
        }

        private Camera _cam;
        private float _currentMoveSpeed;

        private void Awake() {
            this.AddListener(EventType.ObstacleHit, param => Destroy());
            _cam = Camera.main;
        }
        
        private void Update() {
            if (Input.GetAxisRaw("Horizontal") != 0) {
                // if (_currentMoveSpeed == 0) _currentMoveSpeed += 0.1f;
                // _currentMoveSpeed += _currentMoveSpeed * moveSpeed * Time.deltaTime * Mathf.Abs(Input.GetAxis("Horizontal"));
                // _currentMoveSpeed =  Mathf.Clamp(_currentMoveSpeed, 0f, maxSpeed);
                _currentMoveSpeed = Mathf.Lerp(_currentMoveSpeed, maxSpeed, acceleration * Time.deltaTime);
            } else {
                _currentMoveSpeed = Mathf.Lerp(_currentMoveSpeed, 0f, deceleration * Time.deltaTime);
            }
        }

        private void FixedUpdate() {
            Move();
        }

        private void LateUpdate() {
            _cam.transform.position = camPos.transform.position;
        }

        private void Move() {
            rb.MovePosition(transform.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f) * _currentMoveSpeed * Time.fixedDeltaTime);
        }

        private void Destroy() {
            Destroy(gameObject);
        }
    }
}