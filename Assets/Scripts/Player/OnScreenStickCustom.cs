using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Serialization;
using Player.Diver;

namespace UnityEngine.InputSystem.OnScreen
{
    //TODO: Remove all OnScreenControl code from this class, as it now directly calls to the DiverSmoothMovement code
    public class OnScreenStickCustom : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        

        public DiverSmoothMovement _diver;
        

        [FormerlySerializedAs("movementRange")]
        [SerializeField]
        private float m_MovementRange = 50;
        
        [InputControl(layout = "Vector2")]
        [SerializeField]
        private string _controlPath;

        private Vector3 m_StartPos;
        private Vector2 m_PointerDownPos;
        
        protected override string controlPathInternal
        {
            get => _controlPath;
            set => _controlPath = value;
        }
        
        public float movementRange
        {
            get => m_MovementRange;
            set => m_MovementRange = value;
        }

        private void Start()
        {
            m_StartPos = ((RectTransform)transform).anchoredPosition;
        }
        
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
            if (_diver == null)
                _diver = FindObjectOfType<DiverSmoothMovement>();
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            if (_diver == null)
                _diver = FindObjectOfType<DiverSmoothMovement>();
            if (_diver == null)
                return;
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
            var delta = position - m_PointerDownPos;

            delta = Vector2.ClampMagnitude(delta, movementRange);
            ((RectTransform)transform).anchoredPosition = m_StartPos + (Vector3)delta;

            var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
            _diver.OnMove_OnScreen(newPos);
        }

        public void OnPointerUp(PointerEventData eventData)
        { 
            if (_diver == null)
                _diver = FindObjectOfType<DiverSmoothMovement>();
            if (_diver == null)
                return;
            
            ((RectTransform)transform).anchoredPosition = m_StartPos;
            _diver.OnMove_OnScreen(Vector2.zero);
        }
    }

    
}