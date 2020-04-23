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

using CircularBuffer;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FrameDataBuffer {

    public struct FrameData<T>
    {
        public object data;
    }


    [SerializeField]
    int maxDataInBuffer = 20;

    public Action<object> OnDataAdded;

    private Dictionary<Type, CircularBuffer<FrameData<object>>> currentFrameData = new Dictionary<Type, CircularBuffer<FrameData<object>>>();
    private Dictionary<Type, CircularBuffer<FrameData<object>>> lastFrameData = new Dictionary<Type, CircularBuffer<FrameData<object>>>();


    public void SwitchFrame()
    {
        Dictionary<Type, CircularBuffer<FrameData<object>>> temp = currentFrameData;
        currentFrameData = lastFrameData;
        lastFrameData = temp;

        foreach (Type type in currentFrameData.Keys)
        {
            CircularBuffer<FrameData<object>> buffer = currentFrameData[type];
            buffer.Clear();
        }
    }


    public void AddData<T>(T data)
    {
        if (!currentFrameData.TryGetValue(typeof(T), out CircularBuffer<FrameData<object>> buffer))
        {
            buffer = new CircularBuffer<FrameData<object>>(maxDataInBuffer);
            currentFrameData.Add(typeof(T), buffer);
        }

        buffer.PushBack(new FrameData<object>
        {
            data = data
        });

        OnDataAdded?.Invoke(data);
    }

    public bool Contains<T>()
    {
        if (lastFrameData.TryGetValue(typeof(T), out CircularBuffer<FrameData<object>> buffer))
        {
            return buffer.Count != 0;
        }
        else
        {
            return false;
        }
    }

    public bool Exists<T>(Func<T,bool> predicate)
    {
        if (lastFrameData.TryGetValue(typeof(T), out CircularBuffer<FrameData<object>> buffer))
        {
            for (int i = 0; i < buffer.Count; i++)
            {
                if (predicate((T)buffer[i].data))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Return data valid for the frame
    /// Result in some garbage.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public List<T> GetAll<T>()
    {
        List<T> list = new List<T>();

        if (lastFrameData.TryGetValue(typeof(T), out CircularBuffer<FrameData<object>> buffer))
        {
            for (int i = 0; i < buffer.Count; i++)
            {
                list.Add((T)buffer[i].data);
            }
        }

        return list;
    }


    public T GetFirst<T>()
    {
        if (lastFrameData.TryGetValue(typeof(T), out CircularBuffer<FrameData<object>> buffer))
        {
            return (T)buffer.Front().data;
        }

        return default(T);
    }

    public T GetLast<T>()
    {
        if (lastFrameData.TryGetValue(typeof(T), out CircularBuffer<FrameData<object>> buffer))
        {
            return (T)buffer.Back().data;
        }

        return default(T);
    }

    public void Clear()
    {
        currentFrameData.Clear();
        lastFrameData.Clear();
    }
}