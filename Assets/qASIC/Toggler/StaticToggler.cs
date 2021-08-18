using System.Collections.Generic;

namespace qASIC.Toggling
{
    public class StaticToggler : Toggler
    {
        public string togglerTag;
        public bool addToDontDestroy = true;

        #region Static
        public class TogglerState
        {
            public bool state;
            public delegate void TogglerStateChange(bool state);
            public TogglerStateChange OnChange;

            public TogglerState(bool state, TogglerStateChange onChange)
            {
                this.state = state;
                OnChange = onChange;
            }

            public TogglerState(bool state)
            {
                this.state = state;
            }

            public TogglerState()
            {
                state = false;
                OnChange = new TogglerStateChange((bool state) => { });
            }
        }

        public static Dictionary<string, TogglerState> states = new Dictionary<string, TogglerState>();

        public override void Awake()
        {
            if (addToDontDestroy) AssignSingleton();
            AssignListiner();

            if (!states.ContainsKey(togglerTag))
            {
                base.Awake();
                return;
            }

            Toggle(states[togglerTag].state);
        }

        private void AssignListiner()
        {
            if (!states.ContainsKey(togglerTag)) states.Add(togglerTag, new TogglerState());
            if (states[togglerTag].OnChange == null) states[base.tag].OnChange = new TogglerState.TogglerStateChange((bool state) => { });
            states[togglerTag].OnChange += Toggle;
        }

        private void AssignSingleton()
        {
            if (states.ContainsKey(togglerTag))
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (!states.ContainsKey(togglerTag) || states[togglerTag].OnChange == null) return;
            states[togglerTag].OnChange -= Toggle;
        }
        #endregion

        #region Toggle
        public override void Toggle(bool state)
        {
            base.Toggle(state);
            ChangeStateSilent(togglerTag, state);
        }

        public static void ChangeState(string tag, bool state)
        {
            ChangeStateSilent(tag, state);
            if (states[tag].OnChange != null) states[tag].OnChange.Invoke(state);
        }

        public static void ChangeStateSilent(string tag, bool state)
        {
            if (!states.ContainsKey(tag)) states.Add(tag, new TogglerState());
            states[tag].state = state;
        }
        #endregion
    }
}