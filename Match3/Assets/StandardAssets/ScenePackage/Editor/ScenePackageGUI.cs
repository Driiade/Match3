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

using UnityEditor;
using UnityEngine;
using System;

namespace BC_Solution.Editor
{
    public static class ScenePackageGUI
    {

        static int currentIndex = -1;

        [InitializeOnLoadMethod]
        static void AddScenePackageGUIOnSceneView()
        {

#if UNITY_2019
            SceneView.duringSceneGui += OnSceneGUI;
#else
        SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
        }

        static void OnSceneGUI(SceneView sceneview)
        {
            ScriptableGUIStyle.LoadAllGUIStyle();

            ScriptableScenePackage[] scenePackages = FindAssetExtension.GetAllInstances<ScriptableScenePackage>();

            for (int i = scenePackages.Length - 1; i >= 0; i--)
            {
                if (!scenePackages[i].showOnGUI)
                {
                    ArrayUtility.RemoveAt(ref scenePackages, i);
                }
            }

            Array.Sort(scenePackages, (x, y) => { return x.name.CompareTo(y.name); });

            string[] scenePackageNames = new string[scenePackages.Length];
            for (int i = 0; i < scenePackages.Length; i++)
            {
                scenePackageNames[i] = scenePackages[i].folderName + scenePackages[i].name;
            }

            Rect screenRect = sceneview.camera.pixelRect;
            Rect scenePackageRect = new Rect(screenRect.x + screenRect.width - 150, screenRect.y + 150, 150, 50);

#if UNITY_2019
            GUILayout.Window(0, scenePackageRect, (id) =>
            {
#else
        GUILayout.BeginArea(scenePackageRect);
     {
#endif
                GUILayoutExtension.CenterLayout(() =>
            {
                EditorGUILayout.BeginVertical("box");
                {
                    GUILayout.Label("Scene Package", "ScenePackageTitle");
                    GUILayout.Space(5);

                    EditorGUI.BeginChangeCheck();
                    {
                        currentIndex = EditorGUILayout.Popup(currentIndex, scenePackageNames);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        scenePackages[currentIndex].EditorLoadScenePackage();
                    }
                }
                EditorGUILayout.EndVertical();

            });
#if UNITY_2019
            }, "");
#else
        }
        GUILayout.EndArea();
#endif

        }
    }
}