using System.Collections.Generic;
using System.Linq;
using LandmarksR.Scripts.Experiment.Log;
using UnityEngine;

namespace LandmarksR.Scripts.Player
{
    /// <summary>
    /// Manages player events including keyboard, VR input, and collision/trigger events.
    /// </summary>
    public class PlayerEventController : MonoBehaviour
    {
        /// <summary>
        /// The key used for confirmation actions.
        /// </summary>
        [SerializeField] private KeyCode confirmKey = KeyCode.Return;

        /// <summary>
        /// Delegate for handling keyboard events.
        /// </summary>
        public delegate void KeyboardEventHandler();

        private readonly Dictionary<KeyCode, KeyboardEventHandler> _keyboardEvents = new();

        /// <summary>
        /// Delegate for handling VR input events.
        /// </summary>
        public delegate void VRInputEventHandler();

        private readonly Dictionary<OVRInput.Button, VRInputEventHandler> _vrButtonInputEvents = new();

        /// <summary>
        /// Delegate for handling timed update events.
        /// </summary>
        public delegate void TimedUpdateHandler(float elapsedTime);

        private class TimedHandle
        {
            public float ElapsedTime;
            public readonly float TotalTime;
            public VRInputEventHandler VRInputEventHandler;
            public TimedUpdateHandler UpdateHandler;

            public TimedHandle(VRInputEventHandler vrInputEventHandler, float totalTime,
                TimedUpdateHandler updateHandler = null)
            {
                VRInputEventHandler = vrInputEventHandler;
                TotalTime = totalTime;
                UpdateHandler = updateHandler;
            }
        }

        private readonly Dictionary<OVRInput.Button, Dictionary<float, TimedHandle>> _vrButtonTimedInputEvents = new();

        /// <summary>
        /// Delegate for handling general input events.
        /// </summary>
        public delegate void InputEventHandler();

        private InputEventHandler _onConfirm;

        /// <summary>
        /// Delegate for handling trigger events.
        /// </summary>
        public delegate void TriggerEventHandler(Collider collider);

        private TriggerEventHandler _onTriggerEnter;

        /// <summary>
        /// Delegate for handling collision events.
        /// </summary>
        public delegate void CollisionEventHandler(Collision collider);

        private CollisionEventHandler _onCollisionEnter;

        private ExperimentLogger _logger;

        /// <summary>
        /// Unity Start method. Initializes the player event controller.
        /// </summary>
        private void Start()
        {
            _logger = ExperimentLogger.Instance;
            RegisterKeyHandler(confirmKey, Confirm);
            RegisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, Confirm);
        }

        /// <summary>
        /// Unity OnDisable method. Cleans up the event handlers when the object is disabled.
        /// </summary>
        private void OnDisable()
        {
            UnregisterKeyHandler(confirmKey, Confirm);
            UnregisterVRInputHandler(OVRInput.Button.PrimaryIndexTrigger, Confirm);
        }

        /// <summary>
        /// Unity Update method. Handles input events.
        /// </summary>
        private void Update()
        {
            HandleKeys();
            HandleVRButtonInputs();
            HandleTimedVRButtonInputs();
        }

        /// <summary>
        /// Handles keyboard input events.
        /// </summary>
        private void HandleKeys()
        {
            foreach (var keys in _keyboardEvents.Keys.Where(Input.GetKeyDown))
            {
                _keyboardEvents[keys]?.Invoke();
            }
        }

        /// <summary>
        /// Handles VR button input events.
        /// </summary>
        private void HandleVRButtonInputs()
        {
            foreach (var input in _vrButtonInputEvents.Where(input => OVRInput.GetDown(input.Key)))
            {
                _vrButtonInputEvents[input.Key]?.Invoke();
            }
        }

        /// <summary>
        /// Handles timed VR button input events.
        /// </summary>
        private void HandleTimedVRButtonInputs()
        {
            foreach (var selectedButtonDictionaryPair in _vrButtonTimedInputEvents)
            {
                foreach (var timeHandle in selectedButtonDictionaryPair.Value.Select(timeHandlePair =>
                             _vrButtonTimedInputEvents[selectedButtonDictionaryPair.Key][timeHandlePair.Key]))
                {
                    if (OVRInput.Get(selectedButtonDictionaryPair.Key))
                    {
                        timeHandle.ElapsedTime += Time.deltaTime;
                        timeHandle.UpdateHandler?.Invoke(timeHandle.ElapsedTime);

                        if ((timeHandle.ElapsedTime >= timeHandle.TotalTime))
                        {
                            timeHandle.VRInputEventHandler?.Invoke();
                            timeHandle.ElapsedTime = 0;
                            timeHandle.UpdateHandler?.Invoke(timeHandle.ElapsedTime);
                            return;
                        }
                    }

                    if (OVRInput.GetUp(selectedButtonDictionaryPair.Key))
                    {
                        timeHandle.ElapsedTime = 0;
                        timeHandle.UpdateHandler?.Invoke(timeHandle.ElapsedTime);
                    }
                }
            }
        }

        /// <summary>
        /// Handles trigger enter events.
        /// </summary>
        /// <param name="other">The collider that triggered the event.</param>
        private void OnTriggerEnter(Collider other)
        {
            _onTriggerEnter?.Invoke(other);
        }

        /// <summary>
        /// Handles collision enter events.
        /// </summary>
        /// <param name="other">The collision that occurred.</param>
        private void OnCollisionEnter(Collision other)
        {
            _onCollisionEnter?.Invoke(other);
        }

        /// <summary>
        /// Confirms the current action.
        /// </summary>
        public void Confirm()
        {
            _onConfirm?.Invoke();
        }

        /// <summary>
        /// Registers a key handler for a specific key code.
        /// </summary>
        /// <param name="code">The key code.</param>
        /// <param name="keyboardEventHandler">The event handler.</param>
        public void RegisterKeyHandler(KeyCode code, KeyboardEventHandler keyboardEventHandler)
        {
            if (!_keyboardEvents.TryAdd(code, keyboardEventHandler))
            {
                _keyboardEvents[code] += keyboardEventHandler;
            }
        }

        /// <summary>
        /// Unregisters a key handler for a specific key code.
        /// </summary>
        /// <param name="code">The key code.</param>
        /// <param name="keyboardEventHandler">The event handler.</param>
        public void UnregisterKeyHandler(KeyCode code, KeyboardEventHandler keyboardEventHandler)
        {
            if (_keyboardEvents.ContainsKey(code))
            {
                _keyboardEvents[code] -= keyboardEventHandler;
            }
        }

        /// <summary>
        /// Unregisters all key handlers.
        /// </summary>
        public void UnregisterAllKeyHandlers()
        {
            _keyboardEvents.Clear();
        }

        /// <summary>
        /// Registers a timed VR input handler.
        /// </summary>
        /// <param name="input">The VR input button.</param>
        /// <param name="time">The time in seconds.</param>
        /// <param name="vrInputEventHandler">The event handler.</param>
        /// <param name="updateAction">The update handler.</param>
        public void RegisterTimedVRInputHandler(OVRInput.Button input, float time,
            VRInputEventHandler vrInputEventHandler,
            TimedUpdateHandler updateAction = null)
        {
            if (_vrButtonTimedInputEvents.ContainsKey(input))
            {
                if (_vrButtonTimedInputEvents[input].ContainsKey(time))
                {
                    _vrButtonTimedInputEvents[input][time].VRInputEventHandler += vrInputEventHandler;
                    _vrButtonTimedInputEvents[input][time].UpdateHandler += updateAction;
                    _logger.I("event", "Register a new callback for existing input and time: " + input + " " + time);
                }
                else
                {
                    _vrButtonTimedInputEvents[input]
                        .Add(time, new TimedHandle(vrInputEventHandler, time, updateAction));
                    _logger.I("event", "Register a new callback with a new timer for input: " + input + " " + time);
                }
            }
            else
            {
                _vrButtonTimedInputEvents.Add(input,
                    new Dictionary<float, TimedHandle>
                        { { time, new TimedHandle(vrInputEventHandler, time, updateAction) } });
                _logger.I("event", "Register a new callback with a new timer for a new input: " + input + " " + time);
            }
        }

        /// <summary>
        /// Unregisters a timed VR input handler.
        /// </summary>
        /// <param name="input">The VR input button.</param>
        /// <param name="time">The time in seconds.</param>
        /// <param name="vrInputEventHandler">The event handler.</param>
        /// <param name="updateAction">The update handler.</param>
        public void UnregisterTimedVRInputHandler(OVRInput.Button input, float time,
            VRInputEventHandler vrInputEventHandler, TimedUpdateHandler updateAction = null)
        {
            if (!_vrButtonTimedInputEvents.ContainsKey(input)) return;
            if (!_vrButtonTimedInputEvents[input].ContainsKey(time)) return;
            _vrButtonTimedInputEvents[input][time].VRInputEventHandler -= vrInputEventHandler;
            _vrButtonTimedInputEvents[input][time].UpdateHandler -= updateAction;
            _logger.I("event", "UnregisterTimedVRInputHandler: " + input + " " + time);
        }

        /// <summary>
        /// Registers a VR input handler.
        /// </summary>
        /// <param name="input">The VR input button.</param>
        /// <param name="vrInputEventHandler">The event handler.</param>
        public void RegisterVRInputHandler(OVRInput.Button input, VRInputEventHandler vrInputEventHandler)
        {
            if (!_vrButtonInputEvents.TryAdd(input, vrInputEventHandler))
            {
                _vrButtonInputEvents[input] += vrInputEventHandler;
            }
        }

        /// <summary>
        /// Unregisters a VR input handler.
        /// </summary>
        /// <param name="input">The VR input button.</param>
        /// <param name="vrInputEventHandler">The event handler.</param>
        public void UnregisterVRInputHandler(OVRInput.Button input, VRInputEventHandler vrInputEventHandler)
        {
            if (!_vrButtonInputEvents.ContainsKey(input)) return;
            _vrButtonInputEvents[input] -= vrInputEventHandler;
        }

        /// <summary>
        /// Unregisters all VR input handlers.
        /// </summary>
        public void UnregisterAllVRInputHandlers()
        {
            _vrButtonInputEvents.Clear();
        }

        /// <summary>
        /// Registers a confirm handler.
        /// </summary>
        /// <param name="inputEventHandler">The input event handler.</param>
        public void RegisterConfirmHandler(InputEventHandler inputEventHandler)
        {
            _onConfirm += inputEventHandler;
        }

        /// <summary>
        /// Unregisters a confirm handler.
        /// </summary>
        /// <param name="inputEventHandler">The input event handler.</param>
        public void UnregisterConfirmHandler(InputEventHandler inputEventHandler)
        {
            _onConfirm -= inputEventHandler;
        }

        /// <summary>
        /// Registers a collision enter handler.
        /// </summary>
        /// <param name="collisionEventHandler">The collision event handler.</param>
        public void RegisterCollisionEnterHandler(CollisionEventHandler collisionEventHandler)
        {
            _onCollisionEnter += collisionEventHandler;
        }

        /// <summary>
        /// Unregisters a collision enter handler.
        /// </summary>
        /// <param name="collisionEventHandler">The collision event handler.</param>
        public void UnregisterCollisionEnterHandler(CollisionEventHandler collisionEventHandler)
        {
            _onCollisionEnter -= collisionEventHandler;
        }

        /// <summary>
        /// Registers a trigger enter handler.
        /// </summary>
        /// <param name="triggerEventHandler">The trigger event handler.</param>
        public void RegisterTriggerEnterHandler(TriggerEventHandler triggerEventHandler)
        {
            _onTriggerEnter += triggerEventHandler;
        }

        /// <summary>
        /// Unregisters a trigger enter handler.
        /// </summary>
        /// <param name="triggerEventHandler">The trigger event handler.</param>
        public void UnregisterTriggerEnterHandler(TriggerEventHandler triggerEventHandler)
        {
            _onTriggerEnter -= triggerEventHandler;
        }
    }
}
