using UniRx;
using UnityEngine.InputSystem;

namespace Diver
{
    public static class UniRxInputSystemExtensions
    {
        public static IObservable<InputAction.CallbackContext> PerformedAsObservable(this InputAction action)
        {
            return Observable.FromEvent<InputAction.CallbackContext>(
            t => action.performed += t, 
            t => action.performed -= t);
        }
        
        public static IObservable<InputAction.CallbackContext> StartedAsObservable(this InputAction action)
        {
            return Observable.FromEvent<InputAction.CallbackContext>(
                t => action.started += t, 
                t => action.started -= t);
        }
        
        public static IObservable<InputAction.CallbackContext> CancelledAsObservable(this InputAction action)
        {
            return Observable.FromEvent<InputAction.CallbackContext>(
                t => action.canceled += t, 
                t => action.canceled -= t);
        }
    }
}