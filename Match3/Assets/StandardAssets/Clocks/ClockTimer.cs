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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BC_Solution
{
    public class ClockTimer : MonoBehaviour, IResettable, IPhysicsSimulated
    {
        public class Timing
        {
            public enum Mode { ONE_TIME, INFINITE }
            public Mode mode;
            public float timing;
            public float timer;
        }

        [SerializeField]
        ClockReference clockReference;

        [SerializeField]
        UnityEvent OnClock;

        public bool requireUpdate => timings.Count > 0;

        public int priority => 0;


        public List<Timing> timings = new List<Timing>();

        public void StartOneShotTimer(float timing)
        {
            timings.Add(new Timing()
            {
                mode = Timing.Mode.ONE_TIME,
                timing = timing,
                timer = clockReference.GetClock().CurrentFixedTime + timing,
            });


            IPhysicsSimulatedEvents.OnEnable(this); //based on physics to be stable from simulation
        }

        public void StartTimer(float timing)
        {
            timings.Add(new Timing()
            {
                mode = Timing.Mode.INFINITE,
                timing = timing,
                timer = clockReference.GetClock().CurrentFixedTime + timing,
            });

            IPhysicsSimulatedEvents.OnEnable(this); //based on physics to be stable from simulation
        }


        public void StopAllTimers()
        {
            timings.Clear();
        }

        public void IReset()
        {
            StopAllTimers();
        }

        public void ReSynchronizePhysics()
        {
            //PURE AIR
        }

        public void IFixedUpdate()
        {
            float time = clockReference.GetClock().CurrentFixedTime;
            for (int i = timings.Count - 1; i >= 0; i--)
            {
                if (timings[i].timer < time)
                {
                    if (timings[i].mode == Timing.Mode.INFINITE)
                        timings[i].timer = time + timings[i].timing;
                    else
                        timings.RemoveAt(i);

                    OnClock?.Invoke();
                }
            }
        }
    }
}
