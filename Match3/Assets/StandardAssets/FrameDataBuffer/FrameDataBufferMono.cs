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


using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mono Wrapper for a FrameDataBuffer
/// </summary>
[DefaultExecutionOrder(int.MinValue)]
public class FrameDataBufferMono : MonoBehaviour, IDataContainer
{
    private FrameDataBuffer frameDataBuffer = new FrameDataBuffer();

    public void OnEnable()
    {
        frameDataBuffer.Clear();
    }

    private void Update()
    {
        frameDataBuffer.SwitchFrame();
    }

    void OnDisable()
    {
        frameDataBuffer.Clear();
    }

    public void AddData<T>(T data)
    {
        frameDataBuffer.AddData(data);
    }

    public bool Contains<T>()
    {
       return frameDataBuffer.Contains<T>();
    }

    public bool Exists<T>(Func<T, bool> predicate)
    {
        return frameDataBuffer.Exists(predicate);
    }

    public List<T> GetAll<T>()
    {
        return frameDataBuffer.GetAll<T>();
    }

    public T GetFirst<T>()
    {
       return frameDataBuffer.GetFirst<T>();
    }

    public T GetLast<T>()
    {
        return frameDataBuffer.GetLast<T>();
    }

    public T GetLast<T>(Func<T, bool> predicate)
    {
        return frameDataBuffer.GetLast<T>(predicate);
    }

    public void Clear()
    {
        frameDataBuffer.Clear();
    }
}
