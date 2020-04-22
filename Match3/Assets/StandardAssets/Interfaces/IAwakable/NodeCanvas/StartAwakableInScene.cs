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
using OSG;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

namespace BC_Solution
{
    [Category("Game")]
    public class StartAwakableInScene : ActionTask
    {
        protected override string info => $"Start Awakable in: {sceneName}";

        public BBParameter<string> sceneName;
        public float allowedTime = 0.00833333333f;

        private Coroutine delayedAwake;
        protected override void OnExecute()
        {
            delayedAwake = StartCoroutine(DelayedAwake());
        }

        protected override void OnStop()
        {
            if (delayedAwake != null)
            {
                StopCoroutine(delayedAwake);
                delayedAwake = null;
            }
        }

        IEnumerator DelayedAwake()
        {
            Physics2DSimulation simulation = ServiceProvider.GetService<Physics2DSimulation>();

            float timer = Time.realtimeSinceStartup + allowedTime;
            IAwakable[] awakables = GameObjectExtensions.FindObjectsOfTypeAll<IAwakable>(sceneName.value, true);
            foreach (IAwakable awakable in awakables)
            {
                InterfaceFlowUtils.Awake(awakable, simulation);

                if (timer < Time.realtimeSinceStartup)
                {
                    yield return null;
                    timer = Time.realtimeSinceStartup + allowedTime;
                }
            }

            EndAction();

            delayedAwake = null;
        }
    }
}
#endif