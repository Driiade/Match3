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
    public class ClockedRigidbody2D : MonoBehaviour
    {
        [SerializeField]
        new Rigidbody2D rigidbody2D;

        [SerializeField]
        ClockMonoReference clockProvider;

        private float lastTimeScale = 1f;
        private Vector2 lastVelocity;
        private float lastGravityScale;
        private float lastAngularVelocity;
        private void FixedUpdate()
        {
            float clockTimeScale = clockProvider.GetClock().TimeScale;
            if (clockTimeScale != lastTimeScale)
            {
                if (lastTimeScale != 0)
                {
                    lastVelocity = rigidbody2D.velocity;
                    lastGravityScale = rigidbody2D.gravityScale;
                    lastAngularVelocity = rigidbody2D.angularVelocity;

                    rigidbody2D.velocity *= clockTimeScale / lastTimeScale;
                    rigidbody2D.gravityScale *= clockTimeScale / lastTimeScale;
                    rigidbody2D.angularVelocity *= clockTimeScale / lastTimeScale;
                }
                else
                {
                    rigidbody2D.velocity = lastVelocity;
                    rigidbody2D.gravityScale = lastGravityScale;
                    rigidbody2D.angularVelocity = lastAngularVelocity;
                }

                lastTimeScale = clockTimeScale;
            }
        }
    }
}
