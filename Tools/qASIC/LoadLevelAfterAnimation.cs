using UnityEngine;
using UnityEngine.SceneManagement;

namespace qASIC
{
    public class LoadLevelAfterAnimation : StateMachineBehaviour
    {
        public string sceneName;
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) => SceneManager.LoadScene(sceneName);
    }
}