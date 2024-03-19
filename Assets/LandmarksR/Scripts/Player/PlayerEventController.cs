using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LandmarksR.Scripts.Player
{

    public class PlayerEventController : MonoBehaviour
    {
        public delegate void KeyboardEventHandler();
        private readonly Dictionary<KeyCode, KeyboardEventHandler> _keyboardEvents = new();

        public delegate void VRInputEventHandler();
        private readonly Dictionary<OVRInput.Button, VRInputEventHandler> _vrButtonInputEvents = new();

        public class TimedHandle
        {
            public float elapsedTime;
            public readonly float totalTime;
            public VRInputEventHandler vrInputEventHandler;

            public TimedHandle(VRInputEventHandler vrInputEventHandler, float totalTime)
            {
                this.vrInputEventHandler = vrInputEventHandler;
                this.totalTime = totalTime;
            }
        }

        // private readonly Dictionary<OVRInput.Button, TimedHandle> _vrButtonTimedInputEvents = new();
        private readonly Dictionary<OVRInput.Button, Dictionary<float, TimedHandle>> _vrButtonTimedInputEvents = new();



        public delegate void InputEventHandler();
        private InputEventHandler _onConfirm;

        public delegate void TriggerEventHandler(Collider collider);
        private TriggerEventHandler _onTriggerEnter;


        public delegate void CollisionEventHandler(Collision collider);
        private CollisionEventHandler _onCollisionEnter;

        private void Start()
        {
            RegisterKeyHandler(KeyCode.Return, Confirm);
            RegisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, Confirm);
        }

        private void OnDisable()
        {
            UnregisterKeyHandler(KeyCode.Return, Confirm);
            UnregisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, Confirm);
        }


        private void Update()
        {
            HandleKeys();
            HandleVRButtonInputs();
            HandleTimedVRButtonInputs();
        }

        private void HandleKeys()
        {
            foreach (var keys in _keyboardEvents.Keys.Where(Input.GetKeyDown))
            {
                _keyboardEvents[keys]?.Invoke();
            }
        }

        private void HandleVRButtonInputs()
        {
            foreach (var input in _vrButtonInputEvents.Where(input => OVRInput.GetDown(input.Key)))
            {
                _vrButtonInputEvents[input.Key]?.Invoke();
            }
        }

        private void HandleTimedVRButtonInputs()
        {
            foreach (var selectedButtonDictionaryPair in _vrButtonTimedInputEvents)
            {
                foreach (var timeHandle in selectedButtonDictionaryPair.Value.Select(timeHandlePair => _vrButtonTimedInputEvents[selectedButtonDictionaryPair.Key][timeHandlePair.Key]))
                {
                    if (OVRInput.Get(selectedButtonDictionaryPair.Key))
                    {
                        timeHandle.elapsedTime += Time.deltaTime;
                        if (!(timeHandle.elapsedTime >= timeHandle.totalTime)) continue;
                        timeHandle.vrInputEventHandler?.Invoke();
                        timeHandle.elapsedTime = 0;
                    }
                    else
                    {
                        timeHandle.elapsedTime = 0;
                    }
                }
            }
        }
        private void HandleVRButtonTimedInputs()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            _onTriggerEnter?.Invoke(other);
        }

        private void OnCollisionEnter(Collision other)
        {
            _onCollisionEnter?.Invoke(other);
        }

        public void Confirm()
        {
            _onConfirm?.Invoke();
        }

        private void RegisterKeyHandler(KeyCode code, KeyboardEventHandler keyboardEventHandler)
        {
            if (!_keyboardEvents.TryAdd(code, keyboardEventHandler))
            {
                _keyboardEvents[code] += keyboardEventHandler;
            }
        }

        private void UnregisterKeyHandler(KeyCode code, KeyboardEventHandler keyboardEventHandler)
        {
            if (_keyboardEvents.ContainsKey(code))
            {
                _keyboardEvents[code] -= keyboardEventHandler;
            }
        }

        public void UnregisterAllKeyHandlers()
        {
            _keyboardEvents.Clear();
        }

        // public void RegisterTimedVRInputHandler(OVRInput.Button input, VRInputEventHandler vrInputEventHandler, float time)
        // {
        //     if (!_vrButtonTimedInputEvents.ContainsKey(input))
        //     {
        //         _vrButtonTimedInputEvents.Add(input, new TimedHandle(vrInputEventHandler, time));
        //     }
        //     else
        //     {
        //         var timeHandle = _vrButtonTimedInputEvents[input];
        //         if (Math.Abs(timeHandle.totalTime - time) < 0.01f)
        //         {
        //             timeHandle.vrInputEventHandler += vrInputEventHandler;
        //         }
        //     }
        // }
        public void RegisterTimedVRInputHandler(OVRInput.Button input, VRInputEventHandler vrInputEventHandler,
            float time)
        {
            if (_vrButtonTimedInputEvents.ContainsKey(input))
            {
                if (_vrButtonTimedInputEvents[input].ContainsKey(time))
                {
                    _vrButtonTimedInputEvents[input][time].vrInputEventHandler += vrInputEventHandler;
                }
                else
                {
                    _vrButtonTimedInputEvents[input].Add(time, new TimedHandle(vrInputEventHandler, time));
                }
            }
            else
            {
                _vrButtonTimedInputEvents.Add(input, new Dictionary<float, TimedHandle> {{time, new TimedHandle(vrInputEventHandler, time)}});
            }
        }

        public void UnregisterTimedVRInputHandler(OVRInput.Button input, VRInputEventHandler vrInputEventHandler, float time)
        {
            if (_vrButtonTimedInputEvents.ContainsKey(input))
            {
                if (_vrButtonTimedInputEvents[input].ContainsKey(time))
                {
                    _vrButtonTimedInputEvents[input][time].vrInputEventHandler -= vrInputEventHandler;
                }
            }
        }

        public void RegisterVRInputHandler(OVRInput.Button input, VRInputEventHandler vrInputEventHandler)
        {
            if (!_vrButtonInputEvents.TryAdd(input, vrInputEventHandler))
            {
                _vrButtonInputEvents[input] += vrInputEventHandler;
            }
        }

        public void UnregisterVRInputHandler(OVRInput.Button input, VRInputEventHandler vrInputEventHandler)
        {
            if (_vrButtonInputEvents.ContainsKey(input))
            {
                _vrButtonInputEvents[input] -= vrInputEventHandler;
            }
        }

        public void UnregisterAllVRInputHandlers()
        {
            _vrButtonInputEvents.Clear();
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
