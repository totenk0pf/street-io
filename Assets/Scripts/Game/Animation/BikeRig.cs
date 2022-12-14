using System;
using Core.Logging;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Animation {
    [Flags]
    public enum RigState {
        Moving,
        Wheelie,
        Stopped
    }
    
    public class BikeRig : MonoBehaviour {
        [ShowInInspector] [ReadOnly] private RigState state;
        
        [TitleGroup("Transforms")] 
        [SerializeField] private Transform root;
        [SerializeField] private Transform headAxis;
        [SerializeField] private Transform swerveAxis;
        [SerializeField] private Transform[] wheels;

        [TabGroup("Settings", "Head")] [SerializeField]
        private float headRotateSpeed;

        [TabGroup("Settings", "Head")] [SerializeField]
        private float maxHeadAngle;

        [TabGroup("Settings", "Body")] [TitleGroup("Settings/Body/Speed")] [SerializeField]
        private float bodyLiftSpeed;

        [TabGroup("Settings", "Body")] [TitleGroup("Settings/Body/Speed")] [SerializeField]
        private float bodyRotateSpeed;

        [TabGroup("Settings", "Body")] [TitleGroup("Settings/Body/Speed")] [SerializeField]
        private float bodyLeanSpeed;

        [TabGroup("Settings", "Body")] [TitleGroup("Settings/Body/Angle")] [SerializeField]
        private float maxBodyAngle;

        [TabGroup("Settings", "Body")] [TitleGroup("Settings/Body/Angle")] [SerializeField]
        private float maxBodyPitch;

        [TabGroup("Settings", "Body")] [TitleGroup("Settings/Body/Angle")] [SerializeField]
        private float maxLeanAngle;

        [TabGroup("Settings", "Body")] [TitleGroup("Settings/Body/Delay")] [SerializeField]
        private float rotateDelay;

        [TabGroup("Settings", "Body")] [TitleGroup("Settings/Body/Delay")] [SerializeField]
        private float leanDelay;

        [TabGroup("Settings", "Wheels")] [SerializeField]
        private float wheelSpeed;

        [TitleGroup("Equilibrium settings")] [SerializeField]
        private float returnSpeed;

        private float _baseHeadAngle;
        private float _headAngle;
        private Vector3 _clampVector;
        private Quaternion _baseHeadRotation;

        private float _bodyLean;
        private float _bodyAngle;
        private float _bodyPitch;
        private float _baseBodyAngle;
        private Quaternion _baseBodyRotation;
        private Quaternion _baseRootRotation;

        private float _currentRotateDelay;
        private float _currentLeanDelay;


        private void Awake() {
            state             = RigState.Moving;
            _baseHeadAngle    = headAxis.localEulerAngles.x;
            _baseHeadRotation = headAxis.localRotation;
            _baseBodyRotation = swerveAxis.localRotation;
            _baseRootRotation = root.localRotation;
            _baseBodyAngle    = swerveAxis.localEulerAngles.y;
            _headAngle        = _baseHeadAngle;
        }

        private void Update() {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");
            var absHori = Mathf.Abs(horizontal);
            var absVert = Mathf.Abs(vertical);
            if (absHori != 0f) {
                if (absHori > 0.9f) {
                    _currentRotateDelay += Time.deltaTime;
                    _currentLeanDelay   += Time.deltaTime;
                }
                RotateHead();
                RotateBody();
            } else if (absVert != 0f) {
                LiftBody();
                ReturnToEquilibrium(true);
            } else {
                ReturnToEquilibrium();
            }
            RotateWheels();
            NCLogger.Log($"{state.HasFlag(RigState.Wheelie)}");
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

        private void ReturnToEquilibrium(bool yawOnly = false) {
            if (yawOnly) {
                swerveAxis.localRotation = Quaternion.Slerp(swerveAxis.localRotation, 
                                                           _baseBodyRotation, 
                                                           returnSpeed * Time.deltaTime);
                _bodyAngle = Mathf.SmoothStep(_bodyAngle, 0f, bodyRotateSpeed * Time.deltaTime);
                _bodyLean  = Mathf.SmoothStep(_bodyLean, 0f, bodyLeanSpeed * Time.deltaTime);

                headAxis.localRotation = Quaternion.Slerp(headAxis.localRotation, 
                                                          _baseHeadRotation, 
                                                          headRotateSpeed * Time.deltaTime);
                _headAngle = Mathf.Lerp(_headAngle, 0f, returnSpeed * Time.deltaTime);
            } else {
                swerveAxis.localRotation = Quaternion.Slerp(swerveAxis.localRotation, 
                                                            _baseBodyRotation, 
                                                            returnSpeed * Time.deltaTime);
                _bodyAngle = Mathf.SmoothStep(_bodyAngle, 0f, bodyRotateSpeed * Time.deltaTime);
                _bodyLean  = Mathf.SmoothStep(_bodyLean, 0f, bodyLeanSpeed * Time.deltaTime);

                headAxis.localRotation = Quaternion.Slerp(headAxis.localRotation, 
                                                          _baseHeadRotation, 
                                                          headRotateSpeed * Time.deltaTime);
                _headAngle = Mathf.Lerp(_headAngle, 0f, returnSpeed * Time.deltaTime);
                
                root.localRotation = Quaternion.Slerp(root.localRotation, 
                                                      _baseRootRotation, 
                                                      returnSpeed * Time.deltaTime);
                _bodyPitch         = Mathf.SmoothStep(_bodyPitch, 0f, returnSpeed * Time.deltaTime);
            }
            _clampVector = Vector3.zero;
        }

        private void LiftBody() {
            // if (Mathf.Abs(_headAngle - _baseHeadAngle) > 2f ||
            // Mathf.Abs(_bodyAngle - _baseBodyAngle) > 2f) return;
            if (_bodyPitch > 1f) {
                state |= RigState.Wheelie;
            } else {
                state &= ~RigState.Wheelie;
            }
            var euler = root.localEulerAngles;
            _bodyPitch            -= Input.GetAxis("Vertical") * bodyLiftSpeed * Time.deltaTime;
            _bodyPitch            =  Mathf.Clamp(_bodyPitch, -maxBodyPitch, 0);
            root.localEulerAngles =  new Vector3(_bodyPitch, euler.y, euler.z);
        }

        private void RotateWheels() {
            if (wheels.Length < 1) return;
            foreach (var i in wheels) {
                i.localEulerAngles += new Vector3(wheelSpeed * Time.deltaTime, 0f, 0f);
            }
        }
    }
}