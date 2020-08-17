using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Hook
{
    public class HookMovementSmoother : MonoBehaviour
    {
        [MinMaxSlider(0.1f, 5, ShowFields = true)]
        private Vector2 smoothDistanceRange = new Vector2(0.25f, 3);


        [Serializable]
        private class GravityScaleSettings
        {
            public bool useGravityScale = false;

            [MinMaxSlider(1, 3, ShowFields = true)]
            private Vector2 minMaxGravityScale = new Vector2(1, 2);

            public float GetGravityScale(float time)
            {
                if (!useGravityScale) return minMaxGravityScale.x;
                return Mathf.Lerp(minMaxGravityScale.x, minMaxGravityScale.y, time);
            }
        }

        [Serializable]
        private class LinearDragSettings
        {
            public bool useLinearDrag = true;

            [SerializeField] [MinMaxSlider(0.1f, 5, ShowFields = true)]
            private Vector2 smoothDragRange = new Vector2(0.1f, 3);
            
            [MinMaxSlider(0, 10, ShowFields = true)] [SerializeField]
            private Vector2 minMaxLinearDrag = new Vector2(0, 5);

            [SerializeField] private AnimationCurve dragCurve = AnimationCurve.Linear(0, 0, 1, 1);


            public float GetLinearDrag(float time)
            {
                if (!useLinearDrag) return minMaxLinearDrag.x;
                return Mathf.Lerp(minMaxLinearDrag.x, minMaxLinearDrag.y, dragCurve.Evaluate(time));
            }
            
            public float GetLinearDrag(Vector2 actualPosition, Vector2 targetPosition)
            {
                if (!useLinearDrag) return minMaxLinearDrag.x;
                float xDiff =  Mathf.Abs(actualPosition.x - targetPosition.x);
                xDiff = Mathf.Clamp(xDiff, smoothDragRange.x, smoothDragRange.y);
                var time = Mathf.InverseLerp(smoothDragRange.x, smoothDragRange.y, xDiff);
                return Mathf.Lerp(minMaxLinearDrag.y, minMaxLinearDrag.x, dragCurve.Evaluate(time));
            }
        }


        [Toggle("useGravityScale")] [SerializeField]
        private GravityScaleSettings _gravityScaleSettings;

        [Toggle("useLinearDrag")] [SerializeField]
        private LinearDragSettings _linearDragSettings;

        private SpriteRenderer _indicator;
        private Rigidbody2D _hookBody;

        [Inject]
        void Inject(SpriteRenderer indicator, Rigidbody2D hookBody)
        {
            this._indicator = indicator;
            this._hookBody = hookBody;
        }


        private void Update()
        {
            var aPos = _hookBody.position;
            var tPos = (Vector2) _indicator.transform.position;

            if (_linearDragSettings.useLinearDrag)
                _hookBody.drag = _linearDragSettings.GetLinearDrag(aPos, tPos);
            
            
            //Not concerned with the hook being raised only lowered
            if (aPos.y < tPos.y) aPos.y = tPos.y;

            var distance = Vector2.Distance(aPos, tPos);
            distance = Mathf.Clamp(distance, smoothDistanceRange.x, smoothDistanceRange.y);
            var time = Mathf.InverseLerp(smoothDistanceRange.x, smoothDistanceRange.y, distance);
            
            if (_gravityScaleSettings.useGravityScale)
                _hookBody.drag = _gravityScaleSettings.GetGravityScale(time);
            
            
        }
    }
}