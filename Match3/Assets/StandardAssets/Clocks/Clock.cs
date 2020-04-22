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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BC_Solution
{
    public class Clock : MonoBehaviour, IClockProvider
    {
        [SerializeField]
        string clockType;

        [SerializeField]
        string underClockType;

        public string ClockType
        {
            get { return clockType; }
        }

        public string UnderClockType
        {
            get { return underClockType; }
        }

        [Space(10)]
        [SerializeField]
        private float _localTimeScale = 1f;

        public float LocalTimeScale
        {
            get => _localTimeScale;
            set
            {
                _localTimeScale = value;
                UpdateTimeScale(parentTimeScale);
            }
        }


#if !UNITY_EDITOR
    [System.NonSerialized]
#endif

        private float fixedFrameTime = 0f;
        private float renderFrameTime = 0f;

        [Space(10)]
#if !UNITY_EDITOR
    [System.NonSerialized]
#endif
        public Clock clockParent;

#if !UNITY_EDITOR
    [System.NonSerialized]
#endif
        public List<Clock> clocks = new List<Clock>();


        private float deltaFixedTime = 0f;
        public float DeltaFixedTime
        {
            get { return deltaFixedTime; }
        }

        private float deltaRenderTime;
        public float DeltaRenderTime
        {
            get { return deltaRenderTime; }
        }

        private float timeScale;
        private float parentTimeScale;
        public float TimeScale
        {
            get => timeScale;
        }


        public float CurrentFixedTime
        {
            get => fixedFrameTime;
            set => fixedFrameTime = value;
        }

        public float CurrentRenderTime
        {
            get => renderFrameTime;
            set => renderFrameTime = value;
        }

        private Coroutine bulletTimeCoroutine;

        public Clock GetClock()
        {
            return this;
        }

        public void UpdateTimeScale(float parentTimeScale)
        {
            this.parentTimeScale = parentTimeScale;
            timeScale = _localTimeScale * parentTimeScale;

            for (int i = 0; i < this.clocks.Count; i++)
            {
                this.clocks[i].UpdateTimeScale(TimeScale);
            }
        }


        public float UpdateRenderTime(float deltaTime)
        {
            this.deltaRenderTime = deltaTime * timeScale;
            this.renderFrameTime += this.deltaRenderTime;

            for (int i = 0; i < clocks.Count; i++)
            {
                clocks[i].UpdateRenderTime(deltaTime);
            }

            return renderFrameTime;
        }

        public float UpdateFixedTime(float deltaTime)
        {
            this.deltaFixedTime = deltaTime * timeScale;
            this.fixedFrameTime += this.deltaFixedTime;

            for (int i = 0; i < clocks.Count; i++)
            {
                clocks[i].UpdateFixedTime(deltaTime);
            }

            return fixedFrameTime;
        }

        public void BulletTime(BulletTimeScriptableObject bulletTime)
        {
            BulletTime(bulletTime.inDuration, bulletTime.onDuration, bulletTime.outDuration, bulletTime.minTimeScale);
        }

        public void BulletTime(float inDuration, float onDuration, float outDuration, float minTimeScale, float finalTimeScale = 1f)
        {
            if (bulletTimeCoroutine != null)
            {
                StopCoroutine(bulletTimeCoroutine);
            }

            bulletTimeCoroutine = StartCoroutine(BulletTimeCoroutine(inDuration, onDuration, outDuration, minTimeScale, finalTimeScale));
        }

        IEnumerator BulletTimeCoroutine(float inDuration, float onDuration, float outDuration, float minTimeScale, float finalTimeScale = 1f)
        {
            float initialBulletTimeScale = LocalTimeScale;
            float timer = clockParent.fixedFrameTime + inDuration;
            WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

            while (timer > clockParent.fixedFrameTime)
            {
                LocalTimeScale = Mathf.Lerp(initialBulletTimeScale, minTimeScale, 1f - (timer - clockParent.fixedFrameTime) / inDuration);
                yield return waitForFixedUpdate;
            }

            LocalTimeScale = minTimeScale;

            timer = clockParent.fixedFrameTime + onDuration;
            while (timer > clockParent.fixedFrameTime)
            {
                yield return waitForFixedUpdate;
            }

            timer = clockParent.fixedFrameTime + outDuration;

            while (timer > clockParent.fixedFrameTime)
            {
                LocalTimeScale = Mathf.Lerp(minTimeScale, finalTimeScale, (timer - clockParent.fixedFrameTime) / outDuration);
                yield return waitForFixedUpdate;
            }

            LocalTimeScale = finalTimeScale;
            bulletTimeCoroutine = null;
        }

        public void SetClockTime(float time)
        {
            this.fixedFrameTime = time;
            this.renderFrameTime = time;
        }
    }
}