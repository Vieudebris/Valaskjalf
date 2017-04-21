using System;
using UnityEngine;
using System.Collections;

namespace StoneplantStudios.VikingWeapons.Demo
{
    public class SmoothFollow : MonoBehaviour
    {
        public Vector3 offset = new Vector3(0f, 2f, 0f);
        public float speed = 1f;
        public Transform target;

        public Vector3 rotationOffset = new Vector3(20f, 0f, 0f);

        protected virtual void Awake()
        {
            if(target == null)
            {
                Debug.LogError("Target is empty", this);
            }
        }

        protected void LateUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.TransformPoint(offset), Time.deltaTime * speed);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.transform.rotation * Quaternion.Euler(rotationOffset), Time.deltaTime * speed);
        }
    }
}