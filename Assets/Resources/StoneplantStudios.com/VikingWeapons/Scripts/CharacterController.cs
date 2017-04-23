using System;
using UnityEngine;
using System.Collections;

namespace StoneplantStudios.VikingWeapons.Demo
{
    [RequireComponent(typeof(Animator))]
    public class CharacterController : MonoBehaviour
    {
        public enum EquipLocation
        {
            Left,
            Right
        }

        public enum EquipType
        {
            OneHanded = 0,
            TwoHanded = 1,
            TwoHandedBig = 2,
        }


        public float movementSpeed = 6f;
        public float rotationSpeed = 50f;

        public Transform leftEquip;
        public Transform rightEquip;


        protected Animator animator;

        private int _speedHash;
        private int _jumpHash;
        private int _deadHash;
        private int _hitHash;
        private int _leftHitHash;
        private int _weaponTypeHash;

//        private float _aimMovementSpeed;
//        private float _movementSpeed;


        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();

            _speedHash = Animator.StringToHash("MovementSpeed");
            _jumpHash = Animator.StringToHash("Jump");
            _deadHash = Animator.StringToHash("Dead");
            _hitHash = Animator.StringToHash("Hit");
            _leftHitHash = Animator.StringToHash("LeftHit");
            _weaponTypeHash = Animator.StringToHash("WeaponType");
        }

        protected void Update()
        {
            HandleMovementInput();
            HandleJumpInput();
            HandleHitInput();

//            _movementSpeed = Mathf.Lerp(_movementSpeed, _aimMovementSpeed, Time.deltaTime * 10f);
        }

        protected virtual void HandleMovementInput()
        {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");
            Vector2 movement = new Vector2(horizontal, vertical);
            movement *= movementSpeed;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                // Run
                movement *= 2f;
            }

            if (Mathf.Approximately(horizontal, 0f) == false)
            {
                transform.Rotate(Vector3.up * horizontal, rotationSpeed * Time.deltaTime);
            }

//            _aimMovementSpeed = movement.magnitude;
            animator.SetFloat(_speedHash, movement.magnitude);
        }

        protected virtual void HandleJumpInput()
        {
            animator.SetBool(_jumpHash, Input.GetKeyDown(KeyCode.Space));
        }

        protected virtual void HandleHitInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(_DoHit(_hitHash));
            }
            else if (Input.GetMouseButtonDown(1))
            {
                StartCoroutine(_DoHit(_leftHitHash));
            }
        }

        private IEnumerator _DoHit(int hitHash)
        {
            animator.SetBool(hitHash, true);
            yield return new WaitForSeconds(0.1f);
            animator.SetBool(hitHash, false);
        }

        public virtual void Equip(Transform t, EquipLocation equipLocation, EquipType equipType)
        {
            switch (equipLocation)
            {
                case EquipLocation.Left:

                    foreach (Transform child in leftEquip)
                    {
                        Destroy(child.gameObject);
                    }

                    t.transform.SetParent(leftEquip);
                    break;
                case EquipLocation.Right:

                    foreach (Transform child in rightEquip)
                    {
                        Destroy(child.gameObject);
                    }

                    t.transform.SetParent(rightEquip);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            animator.SetFloat(_weaponTypeHash, (float)equipType);

            t.transform.localScale = Vector3.one;
            t.transform.localPosition = Vector3.zero;
            t.transform.localRotation = Quaternion.identity;
        }
    }
}