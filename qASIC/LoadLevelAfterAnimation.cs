using UnityEngine;
using UnityEngine.SceneManagement;

namespace qASIC
{
    public class LoadLevelAfterAnimation : StateMachineBehaviour
    {
        public string SceneName;
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) => SceneManager.LoadScene(SceneName);
    }
}