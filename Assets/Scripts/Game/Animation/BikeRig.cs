using System.Collections;
using Core.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Animation {
    public class BikeRig : MonoBehaviour {
        [TitleGroup("Transforms")]
        [SerializeField] private Transform root;
        [SerializeField] private Transform headAxis;
        [SerializeField] private Transform swerveAxis;
        [SerializeField] private Transform[] wheels;

        [TitleGroup("Head settings")] 
        [SerializeField] private float headRotateSpeed;
        [SerializeField] private float maxHeadAngle;

        [TitleGroup("Body settings")]
        [SerializeField] private float bodyLiftSpeed;
        [SerializeField] private float bodyRotateSpeed;
        [SerializeField] private float bodyLeanSpeed;
        [SerializeField] private float maxBodyAngle;
        [SerializeField] private float maxLeanAngle;
        [SerializeField] private float rotateDelay;
        [SerializeField] private float leanDelay;
        
        private float _baseHeadAngle;
        private float _headAngle;
        private Vector3 _clampVector;

        private float _bodyLean;
        private float _bodyAngle;

        private float _currentRotateDelay;
        private float _currentLeanDelay;

        private void Awake() {
            _baseHeadAngle = headAxis.localEulerAngles.x;
            _headAngle     = _baseHeadAngle;
        }

        private void Update() {
            NCLogger.Log($"{Input.GetAxis("Horizontal")}");
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.9f) {
                _currentRotateDelay += Time.deltaTime;
                _currentLeanDelay += Time.deltaTime;
            } else {
                _currentRotateDelay = 0;
                _currentLeanDelay   = 0;
            }
            RotateHead();
            RotateBody();
            LiftBody();
        }

        private void RotateHead() {
            headAxis.Rotate(headAxis.up,
                            Input.GetAxisRaw("Horizontal") * headRotateSpeed * Time.deltaTime,
                            Space.World);
            var euler = headAxis.localEulerAngles;
            _headAngle = euler.x;
            _headAngle = Mathf.Clamp(_headAngle,
                                         _baseHeadAngle + -maxHeadAngle,
                                         _baseHeadAngle + maxHeadAngle);
            if (Mathf.Abs(_baseHeadAngle + maxHeadAngle - _headAngle) < 0.1f) {
                if (_clampVector == Vector3.zero) _clampVector = euler;
            } else {
                _clampVector = Vector3.zero;
            }

            headAxis.localEulerAngles =
                _clampVector != Vector3.zero
                    ? new Vector3(_clampVector.x, _clampVector.y, _clampVector.z)
                    : new Vector3(_headAngle, euler.y, euler.z);
        }

        private void RotateBody() {
            if (_currentRotateDelay >= rotateDelay) {
                _bodyAngle += Input.GetAxisRaw("Horizontal") * bodyRotateSpeed * Time.deltaTime;
                _bodyAngle =  Mathf.Clamp(_bodyAngle, -maxBodyAngle, maxBodyAngle);
            }

            if (_currentLeanDelay >= leanDelay) {
                _bodyLean -= Input.GetAxisRaw("Horizontal") * bodyLeanSpeed * Time.deltaTime;
                _bodyLean =  Mathf.Clamp(_bodyLean, -maxLeanAngle, maxLeanAngle);
            }
            
            swerveAxis.localEulerAngles = new Vector3(0f, _bodyAngle, _bodyLean);
        }

        private void LiftBody() {
            
        }
    }
}