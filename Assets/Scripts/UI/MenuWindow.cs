using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class MenuWindow : MonoBehaviour
    {
        [Tooltip("The window that will be opened when this one is closed")]
        public MenuWindow onBackWindow;
        public RectTransform menuRoot;
        public GameObject defaultSelection;


        private GameObject _lastSelection;
        private bool _isOpen;

        private static Queue<MenuWindow> _windowQueue = new Queue<MenuWindow>();

        private void Awake()
        {
            _isOpen = false;
            menuRoot.localPosition = Vector3.zero;
            CloseWindow();
        }

        public void OnCloseAllWindowsButton(InputAction.CallbackContext context)
        {
            ChangeToWindow(null);
        }

        public void OnOpenButtonPressed(InputAction.CallbackContext context)
        {
            if (_isOpen) return;
            ChangeToWindow(this);
        }

        public void OnBackPressed(InputAction.CallbackContext context)
        {
            if (_isOpen == false) return;
            ChangeToWindow(onBackWindow);
        }


        private void CloseWindow()
        {
            _isOpen = false;
            _lastSelection = EventSystem.current.currentSelectedGameObject ?? defaultSelection;
            menuRoot.gameObject.SetActive(false);
        }

        private void OpenWindow()
        {
            _isOpen = true;
            EventSystem.current.SetSelectedGameObject(_lastSelection ?? defaultSelection);
            menuRoot.gameObject.SetActive(false);
        }



        public static void ChangeToWindow(MenuWindow window)
        {
            while(_windowQueue.Count > 0)
            {
                var nextWindow = _windowQueue.Dequeue();
                //if window is already in queue for some reason
                if (window != null && nextWindow == window)
                {
                    EnqeueAndOpen(window);
                    return;
                }
                nextWindow.CloseWindow();
            }
            if(window != null) EnqeueAndOpen(window);
        }

        private static void EnqeueAndOpen(MenuWindow window)
        {
            _windowQueue.Enqueue(window);
            window.OpenWindow();
        }
    }

}