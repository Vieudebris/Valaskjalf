using UnityEngine;
using System.Collections;

namespace StoneplantStudios.VikingWeapons.Demo
{
    public class AnimationEventListener : MonoBehaviour
    {
        [SerializeField]
        protected Transform leftFoot;

        [SerializeField]
        protected Transform rightFoot;

        [SerializeField]
        protected ParticleSystem stepEffect;

        [SerializeField]
        protected float destroyStepEffectAfterSeconds = 1f;

        [SerializeField]
        protected Vector3 stepEffectOffset;


        protected virtual void Awake()
        {

        }

        private void RightFootTouchGround(float intensity)
        {
            OnStep(rightFoot, intensity);
        }

        private void LeftFootTouchGround(float intensity)
        {
            OnStep(leftFoot, intensity);
        }

        protected virtual void OnStep(Transform bone, float stepIntensity)
        {
            if (stepEffect == null)
            {
                return;
            }

            var p = Instantiate<ParticleSystem>(stepEffect);
            p.transform.position = bone.transform.position + stepEffectOffset;
            p.transform.rotation = Quaternion.Euler(Vector3.up);

            p.Play();
            if (destroyStepEffectAfterSeconds > 0f)
            {
               Destroy(p.gameObject, destroyStepEffectAfterSeconds);
            }
        }
    }
}