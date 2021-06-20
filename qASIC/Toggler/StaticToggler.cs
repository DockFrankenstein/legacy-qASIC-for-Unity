using System.Collections.Generic;

namespace qASIC.Toggling
{
    public class StaticToggler : Toggler
    {
        public string Tag;
        public bool AddToDontDestroy = true;

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
            if (AddToDontDestroy) AssignSingleton();
            AssignListiner();

            if (!states.ContainsKey(Tag))
            {
                base.Awake();
                return;
            }

            Toggle(states[Tag].state);
        }

        private void AssignListiner()
        {
            if (!states.ContainsKey(Tag)) states.Add(Tag, new TogglerState());
            if (states[Tag].OnChange == null) states[tag].OnChange = new TogglerState.TogglerStateChange((bool state) => { });
            states[Tag].OnChange += Toggle;
        }

        private void AssignSingleton()
        {
            if (states.ContainsKey(Tag))
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (!states.ContainsKey(Tag) || states[Tag].OnChange == null) return;
            states[Tag].OnChange -= Toggle;
        }
        #endregion

        #region Toggle
        public override void Toggle(bool state)
        {
            base.Toggle(state);
            ChangeStateSilent(Tag, state);
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