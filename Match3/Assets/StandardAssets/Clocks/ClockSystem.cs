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

namespace BC_Solution
{
    public class ClockSystem : MonoSystem<ClockMono>, IPhysicsSimulated
    {
        public int priority => 0;

        List<ClockMono> rootClocks = new List<ClockMono>();

        Dictionary<string, ClockMono> typedClocks = new Dictionary<string, ClockMono>();

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            foreach (ClockMono clock in rootClocks)
            {
                clock.UpdateRenderTime(deltaTime);
            }
        }

        public bool requireUpdate
        {
            get => true;
        }

        public void ReSynchronizePhysics()
        {

        }

        public void IFixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            foreach (ClockMono clock in rootClocks)
            {
                clock.UpdateFixedTime(deltaTime);
            }
        }


        public override void OnNewEntities(ClockMono[] entities)
        {
            base.OnNewEntities(entities);
            RebuildClockHierarchy();
        }

        public override void OnRemoveEntities()
        {
            RebuildClockHierarchy();
        }

        public ClockMono GetClock(string clockType)
        {
#if UNITY_EDITOR || DEBUG
            if (!typedClocks.ContainsKey(clockType))
            {
                Debug.LogError($"Clock Type '{clockType}' unknown in ClockSystem.", this);
                return null;
            }
#endif
            return typedClocks[clockType];
        }

        private void RebuildClockHierarchy()
        {
            rootClocks.Clear();
            typedClocks.Clear();

            foreach (ClockMono clock in entities)
            {
                clock.clocks.Clear();

                if (string.IsNullOrEmpty(clock.UnderClockType))
                {
                    this.rootClocks.Add(clock);
                }

                if (!string.IsNullOrEmpty(clock.ClockType))
                    typedClocks.Add(clock.ClockType, clock);
            }

            foreach (ClockMono clock in entities)
            {
                if (!string.IsNullOrEmpty(clock.UnderClockType))
                {
                    PlaceClock(clock, typedClocks);
                }
            }

            foreach (ClockMono clock in rootClocks)
            {
                clock.UpdateTimeScale(1f);
            }
        }

        private void PlaceClock(ClockMono c, Dictionary<string, ClockMono> typedClocks)
        {
            if (typedClocks.TryGetValue(c.UnderClockType, out ClockMono root))
            {
                c.clockParent = root;
                root.clocks.Add(c);
            }
        }
    }
}