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
using System;

namespace BC_Solution.Editor
{
    public static class GUILayoutExtension
    {
        /// <summary>
        /// Center a LayoutCall
        /// </summary>
        /// <param name="layoutCall"></param>
        public static void CenterLayout(Action layoutCall, params GUILayoutOption[] guiLayoutOptions)
        {
            CenterLayout(layoutCall, GUIStyle.none, guiLayoutOptions);
        }


        /// <summary>
        /// Center a LayoutCall
        /// </summary>
        /// <param name="layoutCall"></param>
        public static void CenterLayout(Action layoutCall,GUIStyle guiStyle, params GUILayoutOption[] guiLayoutOptions)
        {
            GUILayout.BeginHorizontal(guiStyle, guiLayoutOptions);
            {
                GUILayout.FlexibleSpace();
                layoutCall();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }


        private static ICloneable copiedObject;
        public static void CopyPasteButton<T>(ref T c, GUIStyle guiStyle, params GUILayoutOption[] guiLayoutOptions) where T : ICloneable
        {
            GUILayout.BeginHorizontal(guiStyle, guiLayoutOptions);
            {
                if(GUILayout.Button("C"))
                    copiedObject = c;

                if(copiedObject != null && copiedObject.GetType() == typeof(T))
                {
                    if (GUILayout.Button("P"))
                        c = (T)copiedObject.Clone();
                }
            }
            GUILayout.EndHorizontal();
        }


        public static void CopyPasteButton<T>(ref T c,params GUILayoutOption[] guiLayoutOptions) where T : ICloneable
        {
            CopyPasteButton(ref c, GUIStyle.none, guiLayoutOptions);
        }
    }
}
