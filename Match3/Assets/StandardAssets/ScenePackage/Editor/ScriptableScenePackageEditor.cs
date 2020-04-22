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
using UnityEditor;
using System.Linq;

namespace BC_Solution.Editor
{
    [CustomEditor(typeof(ScriptableScenePackage))]
    public class ScriptableScenePackageEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            ScriptableScenePackage scenePackage = (ScriptableScenePackage)target;
            Undo.RecordObject(scenePackage, "ScenePackage");
            EditorGUI.BeginChangeCheck();
            {
                SceneAsset[] scenes = FindAssetExtension.GetAllInstances<SceneAsset>();

                string[] sceneNames = new string[scenes.Length];
                for (int i = 0; i < scenes.Length; i++)
                {
                    sceneNames[i] = scenes[i].name;
                }

                sceneNames = sceneNames.OrderBy(x => x).ToArray();

                int currentIndex = 0;

                for (int i = 0; i < scenes.Length; i++)
                {
                    if (sceneNames[i] == scenePackage.activeScene)
                    {
                        currentIndex = i;
                        break;
                    }
                }

                scenePackage.activeScene = sceneNames[EditorGUILayout.Popup("Active Scene", currentIndex, sceneNames)];
                GUILayout.Space(10);

                EditorGUILayout.BeginVertical("box");
                {
                    scenePackage.startScenePackages = (ScriptableScenePackage[])AutoClassInspectorExtension.GenericField("Start Scene package", scenePackage.startScenePackages, typeof(ScriptableScenePackage[]));
                }
                EditorGUILayout.EndVertical();

                GUILayout.Space(10);

                DisplayArray("Scenes", ref scenePackage.scenes, sceneNames);
                GUILayout.Space(5);
                DisplayArray("Cheat Scenes", ref scenePackage.cheatScenes, sceneNames);

                GUILayout.Space(10);

                EditorGUILayout.BeginVertical("box");
                {
                    scenePackage.endScenePackages = (ScriptableScenePackage[])AutoClassInspectorExtension.GenericField("End Scene package", scenePackage.endScenePackages, typeof(ScriptableScenePackage[]));
                }
                EditorGUILayout.EndVertical();

                DisplayArray("Optional Scenes", ref scenePackage.optionalScenes, sceneNames);

                GUILayout.Space(20);
                scenePackage.folderName = EditorGUILayout.TextField("Folder Name", scenePackage.folderName);
                scenePackage.showOnGUI = EditorGUILayout.Toggle("Show on GUI", scenePackage.showOnGUI);
                scenePackage.participateToOptimization = EditorGUILayout.Toggle("Participate to optimization", scenePackage.participateToOptimization);
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(scenePackage);
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Load it"))
            {
                EditorScenePackageExtension.EditorLoadScenePackage(scenePackage);
            }
        }


        public void DisplayArray(string arrayName, ref string[] array, string[] displayName)
        {
            EditorGUILayout.BeginVertical("box");
            {
                GUILayout.Label(arrayName);

                for (int i = 0; i < array.Length; i++)
                {
                    int currentIndex = -1;
                    for (int j = 0; j < displayName.Length; j++)
                    {
                        if (array[i] == displayName[j])
                        {
                            currentIndex = j;
                            break;
                        }
                    }

                    if (currentIndex == -1)
                    {
                        ArrayUtility.RemoveAt(ref array, i);
                        break;
                    }

                    EditorGUILayout.BeginHorizontal();
                    {
                        array[i] = displayName[EditorGUILayout.Popup(currentIndex, displayName)];

                        if (i > 0)
                        {
                            if (GUILayout.Button("up", GUILayout.Width(40)))
                            {
                                string temp = array[i];
                                array[i] = array[i - 1];
                                array[i - 1] = temp;
                                break;
                            }
                        }
                        else
                            GUILayout.Space(40);

                        if (i < array.Length - 1)
                        {
                            if (GUILayout.Button("down", GUILayout.Width(40)))
                            {
                                string temp = array[i];
                                array[i] = array[i + 1];
                                array[i + 1] = temp;
                                break;
                            }
                        }
                        else
                            GUILayout.Space(40);

                        if (GUILayout.Button("-", GUILayout.Width(40)))
                        {
                            ArrayUtility.RemoveAt(ref array, i);
                            break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("+"))
                {
                    ArrayUtility.Add(ref array, displayName[0]);
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}