using System.Collections;
using Holdables;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;
using Zenject;

namespace Hook
{
    public class HookMonoInstaller : MonoInstaller
    {
        [Required]
        //public HookAttachable attachable;
        public SpriteRenderer hookIndicator;
        public override void InstallBindings()
        {
            
            Container.Bind<SpriteRenderer>().FromComponentInNewPrefab(hookIndicator).AsSingle();
            Container.Bind<Rigidbody2D>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesAndSelfTo<Holder>().AsSingle();


            HookHeldItemChangedSignal.Installer.Install(Container);

            Container.Bind<HookAttacher>().FromComponentInHierarchy().AsSingle();
            Container.BindSignal<HookHeldItemChangedSignal>().ToMethod<HookAttacher>(t => t.OnHookAttached).FromResolve();
            
            Container.BindSignal<HookHeldItemChangedSignal>().ToMethod<HookedObjectLerper>(t => t.OnObjectHooked).FromNew();
            if (GetComponent<HoldableObject>() == null)
            {
                gameObject.AddComponent<HoldableObject>();
            }

            Container.BindInterfacesTo<HookTagSetup>().FromNew().AsSingle().NonLazy();
            //     Container.Bind<HoldableObject>().FromNewComponentOnRoot().AsSingle();
            // }
            // else
            // {
            //     Container.Bind<HoldableObject>().FromComponentInHierarchy().AsSingle();
            // }
            //  if (attachable != null) Container.BindSignal<HookHeldItemChangedSignal>().ToMethod(attachable.Callback);
        }
        
        public class HookTagSetup : IInitializable
        {
            private Rigidbody2D _rb;

            public HookTagSetup(Rigidbody2D rb) => _rb = rb;

            public void Initialize() => _rb.gameObject.tag = "Hook";
        }
    }

    
    public class HookedObjectLerper
    {
        public float lerpSpeed = 10;
        private Rigidbody2D _rb;
        private Coroutine _runningCoroutine;

        public HookedObjectLerper(Rigidbody2D rb)
        {
            _rb = rb;
        }

        public void OnObjectHooked(HookHeldItemChangedSignal signal)
        {
            if (_runningCoroutine != null) CoroutineHandler.instance.StopCoroutine(_runningCoroutine);
            
            if (signal.HeldObject != null && signal.HeldObject.rigidbody2D != null)
                _runningCoroutine = CoroutineHandler.instance.StartCoroutine(MoveObjectTowardsAnchor(signal.HeldObject.rigidbody2D));
        }


        IEnumerator MoveObjectTowardsAnchor(Rigidbody2D heldRb)
        {
            bool IsCloseToAnchor()
            {
                return (_rb.position - heldRb.position).sqrMagnitude < 0.5f;
            }
            while (!IsCloseToAnchor())
            {
                // if(IsCloseToAnchor())yield return new WaitForSeconds(1);
                
                    var newPosition = Vector2.Lerp(_rb.position, heldRb.position, Time.deltaTime * lerpSpeed);
                    heldRb.MovePosition(newPosition);
                    yield return null;
                
            }
        }
    }
}