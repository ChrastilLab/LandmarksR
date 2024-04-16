using System.Collections.Generic;
using System.Linq;
using LandmarksR.Scripts.Experiment.Log;
using UnityEngine;

namespace LandmarksR.Scripts.Player
{
    public class PlayerEventController : MonoBehaviour
    {
        public delegate void KeyboardEventHandler();

        private readonly Dictionary<KeyCode, KeyboardEventHandler> _keyboardEvents = new();

        public delegate void VRInputEventHandler();

        private readonly Dictionary<OVRInput.Button, VRInputEventHandler> _vrButtonInputEvents = new();

        public delegate void TimedUpdateHandler(float elapsedTime);

        private class TimedHandle
        {
            public float elapsedTime;
            public readonly float totalTime;
            public VRInputEventHandler vrInputEventHandler;
            public TimedUpdateHandler updateHandler;

            public TimedHandle(VRInputEventHandler vrInputEventHandler, float totalTime,
                TimedUpdateHandler updateHandler = null)
            {
                this.vrInputEventHandler = vrInputEventHandler;
                this.totalTime = totalTime;
                this.updateHandler = updateHandler;
            }
        }

        private readonly Dictionary<OVRInput.Button, Dictionary<float, TimedHandle>> _vrButtonTimedInputEvents = new();


        public delegate void InputEventHandler();

        private InputEventHandler _onConfirm;

        public delegate void TriggerEventHandler(Collider collider);

        private TriggerEventHandler _onTriggerEnter;


        public delegate void CollisionEventHandler(Collision collider);

        private CollisionEventHandler _onCollisionEnter;

        private ExperimentLogger _logger;

        private void Start()
        {
            _logger = ExperimentLogger.Instance;
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
                foreach (var timeHandle in selectedButtonDictionaryPair.Value.Select(timeHandlePair =>
                             _vrButtonTimedInputEvents[selectedButtonDictionaryPair.Key][timeHandlePair.Key]))
                {
                    if (OVRInput.Get(selectedButtonDictionaryPair.Key))
                    {
                        timeHandle.elapsedTime += Time.deltaTime;
                        timeHandle.updateHandler?.Invoke(timeHandle.elapsedTime);

                        if ((timeHandle.elapsedTime >= timeHandle.totalTime))
                        {
                            timeHandle.vrInputEventHandler?.Invoke();
                            timeHandle.elapsedTime = 0;
                            timeHandle.updateHandler?.Invoke(timeHandle.elapsedTime);
                            return;
                        }
                    }

                    if (OVRInput.GetUp(selectedButtonDictionaryPair.Key))
                    {
                        timeHandle.elapsedTime = 0;
                        timeHandle.updateHandler?.Invoke(timeHandle.elapsedTime);
                    }
                }
            }
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

        public void RegisterTimedVRInputHandler(OVRInput.Button input, float time,
            VRInputEventHandler vrInputEventHandler,
            TimedUpdateHandler updateAction = null)
        {
            if (_vrButtonTimedInputEvents.ContainsKey(input))
            {
                if (_vrButtonTimedInputEvents[input].ContainsKey(time))
                {
                    _vrButtonTimedInputEvents[input][time].vrInputEventHandler += vrInputEventHandler;
                    _vrButtonTimedInputEvents[input][time].updateHandler += updateAction;
                    _logger.I("event", "Register A new callback for existing input and time: " + input + " " + time);
                }
                else
                {
                    _vrButtonTimedInputEvents[input]
                        .Add(time, new TimedHandle(vrInputEventHandler, time, updateAction));
                    _logger.I("event", "Register A new callback with a new timer for input: " + input + " " + time);
                }
            }
            else
            {
                _vrButtonTimedInputEvents.Add(input,
                    new Dictionary<float, TimedHandle>
                        { { time, new TimedHandle(vrInputEventHandler, time, updateAction) } });
                _logger.I("event", "Register A new callback with a new timer for a new input: " + input + " " + time);
            }
        }

        public void UnregisterTimedVRInputHandler(OVRInput.Button input, float time,
            VRInputEventHandler vrInputEventHandler, TimedUpdateHandler updateAction = null)
        {
            if (!_vrButtonTimedInputEvents.ContainsKey(input)) return;
            if (!_vrButtonTimedInputEvents[input].ContainsKey(time)) return;
            _vrButtonTimedInputEvents[input][time].vrInputEventHandler -= vrInputEventHandler;
            _vrButtonTimedInputEvents[input][time].updateHandler -= updateAction;
            _logger.I("event", "UnregisterTimedVRInputHandler: " + input + " " + time);
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
            if (!_vrButtonInputEvents.ContainsKey(input)) return;
            _vrButtonInputEvents[input] -= vrInputEventHandler;
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
