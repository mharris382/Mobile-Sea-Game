using System;

namespace Core
{
    public class ObservedValue<T>
    {
        private T currentValue;

        public ObservedValue(T initialValue)
        {
            currentValue = initialValue;
        }

        public T Value
        {
            get => currentValue;

            set
            {
                if (value == null)
                {
                    return;
                }
                
                if (currentValue != null && !currentValue.Equals(value) || currentValue == null)
                {
                    currentValue = value;

                    OnValueChange?.Invoke();
                    OnValueChanged?.Invoke(currentValue);
                }
            }
        }

        /// <summary>
        /// Sets the value without notification.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetSilently(T value)
        {
            currentValue = value;
        }

        /// <summary>
        /// Subscribe to this event to get notified when the value changes.
        /// </summary>
#pragma warning disable 0067
        public event Action OnValueChange;

        public event Action<T> OnValueChanged;
#pragma warning restore 0067
    }
}