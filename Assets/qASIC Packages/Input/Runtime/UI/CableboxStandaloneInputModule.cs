using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;


namespace qASIC.Input.UI
{
    public class CableboxStandaloneInputModule : BaseInputModule
    {
        [Header("Pointer")]
        [SerializeField] bool useMouse;

        [Header("Navigation")]
        [MapItemType(MapItemType.Input2DAxis)] [SerializeField] private InputMapItemReference i_move;
        [MapItemType(MapItemType.InputBinding)] [SerializeField] private InputMapItemReference i_confirm;
        [MapItemType(MapItemType.InputBinding)] [SerializeField] private InputMapItemReference i_cancel;

        Vector2 _moveDelta;
        InputEventType _confirmEventType;
        InputEventType _cancelEventType;

        [Header("Move repeating")]
        [SerializeField] private float moveRepeatDelay = 0.5f;
        [SerializeField] private float moveRepeatRate = 0.1f;

        private NavigationModel _navigationState;
        private ButtonEventData _buttonData;
        private PointerEventData _pointerEventData;

        protected override void OnEnable()
        {
            base.OnEnable();
            Update.InputUpdateManager.OnUpdate += HandleInput;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Update.InputUpdateManager.OnUpdate -= HandleInput;
        }

        public override void ActivateModule()
        {
            base.ActivateModule();

            //Select firstSelectedGameObject if nothing is selected
            var toSelect = eventSystem.currentSelectedGameObject;
            if (toSelect == null)
                toSelect = eventSystem.firstSelectedGameObject;

            eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
        }

        public override void Process()
        {
            ProcessNavigation(ref _navigationState);
        }

        protected bool SendUpdateEventToSelectedObject()
        {
            if (eventSystem.currentSelectedGameObject == null)
                return false;

            var data = GetBaseEventData();
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
            return data.used;
        }

        void HandleInput()
        {
            _navigationState.move = _moveDelta = i_move.GetInputValue<Vector2>();

            if (eventSystem.currentSelectedGameObject == null) return;

            ExecuteEvents.ExecuteHierarchy(eventSystem.currentSelectedGameObject, GetButtonEventData(i_confirm, ref _confirmEventType), CableboxExecuteEvents.ConfirmHandler);
            ExecuteEvents.ExecuteHierarchy(eventSystem.currentSelectedGameObject, GetButtonEventData(i_cancel, ref _cancelEventType), CableboxExecuteEvents.CancelHandler);
        }

        BaseEventData GetButtonEventData(InputMapItemReference binding, ref InputEventType eventType)
        {
            if (_buttonData == null)
                _buttonData = new ButtonEventData(eventSystem);

            eventType = binding.GetInputEvent();

            _buttonData.Reset();
            _buttonData.EventType = eventType;
            return _buttonData;
        }

        private void ProcessNavigation(ref NavigationModel state)
        {
            var usedSelectionChange = false;
            if (eventSystem.currentSelectedGameObject != null)
            {
                var data = GetBaseEventData();
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
                usedSelectionChange = data.used;
            }

            if (!eventSystem.sendNavigationEvents) return;
            var move = state.move;
            if (usedSelectionChange || (Mathf.Approximately(move.x, 0f) && Mathf.Approximately(move.y, 0f)))
            {
                state.consecutiveMoveCount = 0;
                return;
            }

            var time = Time.unscaledTime;
            var moveDirection = GetMoveDirection(move);

            if (moveDirection != state.lastMoveDirection)
                state.consecutiveMoveCount = 0;

            if (moveDirection == MoveDirection.None)
            {
                state.consecutiveMoveCount = 0;
                return;
            }

            var allow = true;
            if (state.consecutiveMoveCount != 0)
            {
                allow = time > state.lastMoveTime + 
                    (state.consecutiveMoveCount > 1 ? moveRepeatRate : moveRepeatDelay);
            }

            if (!allow) return;

            if (state.eventData == null)
                state.eventData = new AxisEventData(eventSystem);

            state.eventData.Reset();
            state.eventData.moveVector = move;
            state.eventData.moveDir = moveDirection;
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, state.eventData, ExecuteEvents.moveHandler);

            state.consecutiveMoveCount += 1;
            state.lastMoveTime = time;
            state.lastMoveDirection = moveDirection;
        }

        private MoveDirection GetMoveDirection(Vector2 move)
        {
            if (move.sqrMagnitude < Mathf.Epsilon)
                return MoveDirection.None;

            switch (Mathf.Abs(move.x) > Mathf.Abs(move.y))
            {
                //Horizontal
                case true:
                    return move.x > 0 ? MoveDirection.Right : MoveDirection.Left;
                //Vertical
                default:
                    return move.y > 0 ? MoveDirection.Up : MoveDirection.Down;
            }
        }
    }

    internal struct NavigationModel
    {
        public Vector2 move;
        public int consecutiveMoveCount;
        public MoveDirection lastMoveDirection;
        public float lastMoveTime;
        public AxisEventData eventData;

        public void Reset()
        {
            move = Vector2.zero;
        }
    }

    internal struct PointerModel
    {
        public PointerModel(PointerEventData eventData) : this()
        {
            this.eventData = eventData;
            ChangedThisFrame = true;
        }

        private Vector2 _screenPosition;
        private Vector2 _scrollDelta;
        private Vector3 _worldPosition;
        private Quaternion _worldOrientation;
        private float _pressure;
        private float _azimuthAngle;
        private float _altitudeAngle;
        private float _twist;
        private Vector2 _radius;

        private RaycastResult _pressRaycast;
        private GameObject _pressObject;
        private GameObject _rawPressObject;
        private GameObject _lastPressObject;
        private GameObject _dragObject;
        private Vector2 _pressPosition;
        private float _clickTime;
        private int _clickCount;
        private bool _dragging;
        private bool _clickedOnSameGameObject;
        private bool _ignoreNextClick;

        public InputEventType leftButton;
        public InputEventType rightButton;
        public InputEventType middleButton;
        public PointerEventData eventData;

        public bool ChangedThisFrame { get; set; }

        public Vector2 ScreenPosition
        {
            get => _screenPosition;
            set
            {
                if (_screenPosition != value)
                {
                    _screenPosition = value;
                    ChangedThisFrame = true;
                }
            }
        }

        public Vector3 WorldPosition
        {
            get => _worldPosition;
            set
            {
                if (_worldPosition != value)
                {
                    _worldPosition = value;
                    ChangedThisFrame = true;
                }
            }
        }

        public Quaternion WorldOrientation
        {
            get => _worldOrientation;
            set
            {
                if (_worldOrientation != value)
                {
                    _worldOrientation = value;
                    ChangedThisFrame = true;
                }
            }
        }

        public Vector2 ScrollDelta
        {
            get => _scrollDelta;
            set
            {
                if (_scrollDelta != value)
                {
                    ChangedThisFrame = true;
                    _scrollDelta = value;
                }
            }
        }

        public float Pressure
        {
            get => _pressure;
            set
            {
                if (_pressure != value)
                {
                    ChangedThisFrame = true;
                    _pressure = value;
                }
            }
        }

        public float AzimuthAngle
        {
            get => _azimuthAngle;
            set
            {
                if (_azimuthAngle != value)
                {
                    ChangedThisFrame = true;
                    _azimuthAngle = value;
                }
            }
        }

        public float AltitudeAngle
        {
            get => _altitudeAngle;
            set
            {
                if (_altitudeAngle != value)
                {
                    ChangedThisFrame = true;
                    _altitudeAngle = value;
                }
            }
        }

        public float Twist
        {
            get => _twist;
            set
            {
                if (_twist != value)
                {
                    ChangedThisFrame = true;
                    _twist = value;
                }
            }
        }

        public Vector2 Radius
        {
            get => _radius;
            set
            {
                if (_radius != value)
                {
                    ChangedThisFrame = true;
                    _radius = value;
                }
            }
        }
    }

    public class ButtonEventData : BaseEventData
    {
        public InputEventType EventType { get; set; }
        public ButtonEventData(EventSystem eventSystem) : base (eventSystem)
        {
            EventType = InputEventType.None;
        }
    }
}
