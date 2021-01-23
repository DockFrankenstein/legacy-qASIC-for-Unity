using UnityEngine;

namespace qASIC
{
    public class LoadLevelAfterAnimation : StateMachineBehaviour
    {
        public string sceneName;

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            LevelManager.LoadScene(sceneName);
        }
    }
}