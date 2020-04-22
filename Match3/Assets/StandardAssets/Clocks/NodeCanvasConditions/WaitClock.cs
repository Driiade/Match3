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
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;


namespace BC_Solution
{
    [Category("Clock")]
    [Description("Wait clock time (dont work on decorator, use WaitClockDecorator instead)")]
    public class WaitClock : ConditionTask<ClockReference>
    {
        protected override string info => $"Wait Clock: {waitTime}s";

        public BBParameter<float> waitTime = 0;

        private float timer;
        private bool isInit = false;

        protected override void OnEnable()
        {
            isInit = false;
        }

        protected override bool OnCheck()
        {
            if (!isInit)
            {
                timer = agent.GetClock().CurrentFixedTime + waitTime.value;
                isInit = true;
            }

            return timer <= agent.GetClock().CurrentFixedTime;
        }
    }
}
#endif