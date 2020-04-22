/*Copyright(c) <2017> <Benoit Constantin ( France ) >

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. 
*/
#if NODECANVAS
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace BC_Solution
{
    [Category("Decorators")]
    public class WaitClockDecorator : BTDecorator
    {
        public BBParameter<ClockProvider> clockProvider;
        public float waitTime = 1;

        private float timer;


        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            if (decoratedConnection == null)
            {
                return Status.Optional;
            }

            status = decoratedConnection.Execute(agent, blackboard);

            if (status == Status.Running)
            {
                timer += clockProvider.value.GetClock().DeltaFixedTime;
                if (timer >= waitTime)
                {
                    timer = 0;
                    decoratedConnection.Reset();
                    return Status.Failure;
                }
            }

            return status;
        }

        protected override void OnReset()
        {
            timer = 0;
        }

        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        protected override void OnNodeGUI()
        {
            GUILayout.Space(25);
            var pRect = new Rect(5, GUILayoutUtility.GetLastRect().y, rect.width - 10, 20);
            var t = 1 - (timer / waitTime);
            UnityEditor.EditorGUI.ProgressBar(pRect, t, timer > 0 ? string.Format("Timeouting ({0})", timer.ToString("0.0")) : $"Ready {waitTime}s");
        }

#endif
        ///----------------------------------------------------------------------------------------------


    }
}
#endif