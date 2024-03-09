using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LandmarksR.Scripts.Experiment.Log;
using UnityEngine;
using UnityEngine.Events;

namespace LandmarksR.Scripts.Player
{
    public class PlayerEventController : MonoBehaviour
    {
        public static PlayerEventController Instance => _instance ??= BuildInputControl();
        private static PlayerEventController _instance;

        public delegate void KeyEventHandler();
        private readonly Dictionary<KeyCode, KeyEventHandler> _keyEvents = new();

        public delegate void InputEventHandler();
        private InputEventHandler _onConfirm;

        public delegate void TriggerEventHandler(Collider collider);
        private TriggerEventHandler _onTriggerEnter;


        public delegate void CollisionEventHandler(Collision collider);
        private CollisionEventHandler _onCollisionEnter;

        private static PlayerEventController BuildInputControl()
        {
            return new GameObject("PlayerEventController").AddComponent<PlayerEventController>();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }

        private void Start()
        {
            RegisterKeyHandler(KeyCode.Return, ReturnConfirmHandler);
        }

        private void OnDisable()
        {
            UnregisterKeyHandler(KeyCode.Return, ReturnConfirmHandler);
        }


        private void Update()
        {
            HandleKeys();
        }


        private void OnTriggerEnter(Collider other)
        {
            _onTriggerEnter?.Invoke(other);
        }

        private void OnCollisionEnter(Collision other)
        {
            _onCollisionEnter?.Invoke(other);
        }

        private void RegisterKeyHandler(KeyCode code, KeyEventHandler keyEventHandler)
        {
            if (!_keyEvents.TryAdd(code, keyEventHandler))
            {
                _keyEvents[code] += keyEventHandler;
            }
        }

        private void ReturnConfirmHandler()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                _onConfirm?.Invoke();
            }
        }

        private void HandleKeys()
        {
            foreach (var keys in _keyEvents.Keys.Where(Input.GetKeyDown))
            {
                _keyEvents[keys]?.Invoke();
            }
        }

        public void Confirm()
        {
            _onConfirm?.Invoke();
        }

        private void UnregisterKeyHandler(KeyCode code, KeyEventHandler keyEventHandler)
        {
            if (_keyEvents.ContainsKey(code))
            {
                _keyEvents[code] -= keyEventHandler;
            }
        }

        public void RegisterConfirmHandler(InputEventHandler inputEventHandler)
        {
            _onConfirm += inputEventHandler;
        }

        public void UnregisterConfirmHandler(InputEventHandler inputEventHandler)
        {
            _onConfirm -= inputEventHandler;
        }

        public void RegisterCollisionEnterHandler(CollisionEventHandler collisionEventHandler)
        {
            _onCollisionEnter += collisionEventHandler;
        }

        public void UnregisterCollisionEnterHandler(CollisionEventHandler collisionEventHandler)
        {
            _onCollisionEnter -= collisionEventHandler;
        }

        public void RegisterTriggerEnterHandler(TriggerEventHandler triggerEventHandler)
        {
            _onTriggerEnter += triggerEventHandler;
        }

        public void UnregisterTriggerEnterHandler(TriggerEventHandler triggerEventHandler)
        {
            _onTriggerEnter -= triggerEventHandler;
        }

    }
}
