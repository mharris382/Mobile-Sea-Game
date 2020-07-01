using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// Handles movement of the hook when picking up and dropping items
    /// </summary>
    public class HookPickupHandler : MonoBehaviour
    {
        [Range(0, 1)] [SerializeField] private float circleCastRadius = 0.125f;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private float hookLoweringSmoothing = 0.1f;
        [SerializeField] private float stuckDistance = 0.5f;
        [SerializeField] private float maxStuckTime = 3;
        [SerializeField] private float maxSpeed = 5;
        public Transform diver;

        public Hook hook;
        private bool _spawned = false;
        private Holder.TargetJointHolder joint;

        private void Awake()
        {
            hook.OnObjectHooked += holdable =>
            {
                Retract();
                Debug.Log("Hook picked up object");
            };
        }

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (_spawned || hook.IsHoldingObject)
                {
                    Retract();
                }
                else
                {
                    Spawn();
                }
            }
        }

        private void Spawn()
        {
            joint?.Dispose();
            StartCoroutine(doSpawn());
        }

        private IEnumerator doSpawn()
        {
            float timeStuck = 0;
            bool goalReached = false;
            Vector3 velocity = Vector3.zero;
            Vector3 diverPoint = diver.position;
            Vector3 surfacePoint = new Vector3(diverPoint.x, -1);

            Vector3 targetPoint = GetDropPoint(surfacePoint, diver.position);

            MoveHookToSurface(surfacePoint);
            CreateJoint(surfacePoint);


            Action pickupCallback = () => { goalReached = true; };
            hook.OnHookPickedUp += pickupCallback;

            while (!goalReached && !joint.isDisposed)
            {
                Vector3 currentTarget = targetPoint;
                if (hook.isBeingHeld) break;
                try
                {
                    if (!(joint.Length > stuckDistance))
                    {
                        timeStuck = 0;
                    }
                    else if (timeStuck > maxStuckTime)
                    {
                        Invoke("Retract", 0.125f);
                        joint?.Dispose();
                        BreakHook();
                        yield break;
                    }
                    else
                    {
                        currentTarget = joint.TargetPoint;
                        timeStuck += Time.deltaTime;
                    }

                    joint.TargetPoint = Vector3.SmoothDamp(joint.TargetPoint, currentTarget, ref velocity,
                        Time.deltaTime, maxSpeed);

                    if (currentTarget == targetPoint &&
                        Vector2.Distance(hook.rigidbody2D.position, targetPoint) < 0.125f)
                    {
                        goalReached = true;
                    }
                }
                catch (Exception e)
                {
                    joint?.Dispose();
                    Console.WriteLine(e);
                    throw;
                }

                yield return new WaitForEndOfFrame();
            }

            hook.OnHookPickedUp -= pickupCallback;
            joint?.Dispose();
        }

        private  Holder.TargetJointHolder CreateJoint(Vector3 targetPoint)
        {
            this.joint = new Holder.TargetJointHolder(hook.rigidbody2D, targetPoint, float.MaxValue);
            joint.Attach();
            return joint;
        }

        private Vector3 GetDropPoint(Vector3 surfacePoint, Vector3 diverPoint)
        {
            RaycastHit2D[] hits = new RaycastHit2D[10];
            ContactFilter2D filter = new ContactFilter2D()
            {
                useTriggers = false,
                useDepth = false,
                useNormalAngle = false,
                useOutsideDepth = false,
                useOutsideNormalAngle = false,
                useLayerMask = true,
                layerMask = collisionMask
            };
            var hitCnt = Physics2D.CircleCast(surfacePoint, circleCastRadius, Vector2.down, filter, hits);


            //if (hitCnt > 0) Debug.Log($"Hit a surface {hits[0].collider.name}");
            Vector3 targetPoint = (hitCnt > 0 && hits[0].point.y < diverPoint.y)
                ? (Vector3) hits[0].point + Vector3.up * (circleCastRadius + 0.25f)
                : diverPoint;
            return targetPoint;
        }

        private void MoveHookToSurface(Vector3 surfacePoint)
        {
            hook.rigidbody2D.position = surfacePoint;
            hook.rigidbody2D.velocity = Vector2.zero;
        }

        private void BreakHook()
        {
            if (hook.IsHoldingObject)
            {
                hook.ReleaseObject();
            }
        }

        private void Retract()
        {
            joint?.Dispose();
            StartCoroutine(doRetract());
        }

        private IEnumerator doRetract()
        {
            bool goalReached = false;
            float timeStuck = 0;
            Vector3 velocity = Vector3.zero;
            Vector3 surfacePoint = new Vector3(hook.rigidbody2D.position.x, -1);
            
            joint = CreateJoint(hook.rigidbody2D.position);
            
            
            while (!goalReached)
            {
                try
                {
                    joint.TargetPoint = Vector3.SmoothDamp(joint.TargetPoint, surfacePoint, ref velocity, Time.deltaTime);

                    if (joint.Length > stuckDistance)
                    {
                        hook.rigidbody2D.MovePosition(joint.TargetPoint);
                        hook.ReleaseObject();
                    }

                    if (Vector2.Distance(hook.rigidbody2D.position, surfacePoint) < 0.125f)
                    {
                        goalReached = true;
                    }
                }
                catch (Exception e)
                {
                    joint?.Dispose();
                    throw;
                }

                yield return new WaitForEndOfFrame();
            }

            joint.Dispose();
        }
    }
}