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


using UnityEngine;

namespace BC_Solution
{
    /// <summary>
    /// Get a clock by string reference
    /// </summary>
    public class ClockMonoReference : AbstractClockMono, IAwakable
    {

        [SerializeField]
        string clockType = "Gameplay";

        private ClockMono cachedClock;

        public override float CurrentFixedTime { get => cachedClock.CurrentFixedTime; set => cachedClock.CurrentFixedTime = value; }
        public override float CurrentRenderTime { get => cachedClock.CurrentRenderTime; set => cachedClock.CurrentRenderTime = value; }
        public override float LocalTimeScale { get => cachedClock.LocalTimeScale; set => cachedClock.LocalTimeScale = value; }
        public override float DeltaFixedTime { get => cachedClock.DeltaFixedTime; }
        public override float DeltaRenderTime { get => cachedClock.DeltaRenderTime;}

        private void RefreshCachedClock()
        {
            var clockSystem = ServiceProvider.GetService<ClockSystem>();

            if (!clockSystem)
            {
                Debug.LogError(this.gameObject + ": ClockSystem service not available.");
                return;
            }

            cachedClock = clockSystem.GetClock(clockType);
        }

        public ClockMono GetClock()
        {
            if (!cachedClock)
                RefreshCachedClock();
            return cachedClock;
        }

        public override void SetClockTime(float time)
        {
            cachedClock.CurrentFixedTime = time;
            cachedClock.CurrentRenderTime = time;
        }

        public override void BulletTime(BulletTimeScriptableObject bulletTime)
        {
            cachedClock.BulletTime(bulletTime);
        }

        public void IAwake()
        {
            RefreshCachedClock();
        }
    }
}