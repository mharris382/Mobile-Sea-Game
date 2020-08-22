using System;
using Core;
using Signals;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace Hook
{
    public class UI_HookIndicator : MonoBehaviour
    {
        [Inject]
        private SpriteRenderer hookIndicator;

        [SerializeField] private Color offScreenColor = Color.white;
        
        private SpriteRenderer _offScreenIndicator;
        private ReactiveProperty<bool> _isIndicatorOnScreen;
        private IDisposable _disposable;
        
        
        private void Start()
        {
            _isIndicatorOnScreen = new ReactiveProperty<bool>();
            _offScreenIndicator = Instantiate(hookIndicator, hookIndicator.transform.position, Quaternion.identity);
            _offScreenIndicator.color = offScreenColor;

            MessageBroker.Default.Receive<RopeLengthChangedSignal>().Select(t => t.ropeLength).Subscribe(length => RopeLength = length);
            
           var d1 = _isIndicatorOnScreen.Subscribe(onScreen => _offScreenIndicator.enabled = !onScreen);
           var d2 = _isIndicatorOnScreen.Select(onScreen => !onScreen ? Observable.EveryUpdate().AsUnitObservable() : Observable.Never<Unit>()).Switch().Subscribe(_ =>
            {
                Debug.Log("Updating Off Screen Indicator");
                var viewportPos = Cam.WorldToViewportPoint(_pos);
                viewportPos.x = Mathf.Clamp01(viewportPos.x);
                viewportPos.y = Mathf.Clamp01(viewportPos.y);
                _offScreenIndicator.transform.position = (Vector2) Cam.ViewportToWorldPoint(viewportPos);
            });
           
            _disposable = new CompositeDisposable(d1, d2);
        }

        private void OnDestroy() => _disposable?.Dispose();

        private Camera _cam;
        private Vector2 _pos;

        private Camera Cam => _cam ? _cam : (_cam = Camera.main);
        
        public float RopeLength
        {
            set => _pos.y = 0 - value;
        }

        public Vector2 BoatPosition
        {
            set => _pos.x = value.x;
        }

        
        private void Update()
        {
            hookIndicator.transform.position = _pos;
            ClampPosition();
            //Debug.Log(IsOnScreen() ? "Hook Point is on screen!" : "Hook Point is off screen");
            _isIndicatorOnScreen.Value = IsOnScreen();
        }

        private bool IsOnScreen()
        {
            var pos = Cam.WorldToViewportPoint(_pos);
            if (pos.x < 0 || pos.x > 1)
            {
                return false;
            }

            if (pos.y < 0 || pos.y > 1)
            {
                return false;
            }

            return true;
        }

        private void ClampPosition()
        {
            void ClampPositionToLevel(ref Vector3 position)
            {
                var rect = GameManager.Instance.CurrentLevel.GetLevelRect();

                position.x = Mathf.Clamp(position.x, rect.xMin, rect.xMax);
                position.y = Mathf.Clamp(position.y, rect.yMin, rect.yMax);
            }

            var transform1 = hookIndicator.transform;
            Vector3 currPosition = transform1.position;
            ClampPositionToLevel(ref currPosition);
            transform1.position = currPosition;
        }
    }
}