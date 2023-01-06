using UnityEngine;

namespace Team14
{
    [CreateAssetMenu(fileName = "new SingleUseTask", menuName = "ScriptableTasks/SingleUseTask", order = 0)]
    public class ScriptableTask : ScriptableTaskBase
    {
        public override void Update()
        {
            Finish();
            RaiseUI();
        }

        public override void ResetCounter()
        {
            return;
        }

        public override string GetProgressText()
        {
            return null;
        }
    }
}