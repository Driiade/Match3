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
using System.Reflection;
using System.Collections.Generic;
using System;

namespace BC_Solution.Editor
{
    [CustomEditor(typeof(PersistentDataSystem))]
    public class PersistentDataSystemEditor : UnityEditor.Editor
    {
        bool foldout = false;

        public override void OnInspectorGUI()
        {
           serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            PersistentDataSystem persistentData = ((PersistentDataSystem)target);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("dataVersion"), true);

            GUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("autoSave"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("saveMode"), true);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("awakeLoadMode"), true);


            if (persistentData.awakeLoadMode == PersistentDataSystem.AwakeLoadMode.SPECIFIC_CLASS)
            {
                //FIND ALL SAVEDDATA BY REFLECTION
                List<string> options = new List<string>();

                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (type.IsSubclassOf(typeof(SavedData)))
                        {
                            options.Add(type.FullName);
                        }
                    }
                }

                foldout = EditorGUILayout.Foldout(foldout, "Class To Load");

                if (foldout)
                {
                    GUILayout.Space(5);
                    GUILayout.BeginVertical();

                    if (GUILayout.Button("Add Element"))
                    {
                        if (persistentData.classToLoad == null)
                        {
                            persistentData.classToLoad = new List<string>();
                        }
                        persistentData.classToLoad.Add("");
                    }

                    if (persistentData.classToLoad != null)
                    {
                        EditorGUI.indentLevel++;
                        for (var i = 0; i < persistentData.classToLoad.Count; i++)
                        {
                            GUILayout.BeginHorizontal();

                            int selectedType = 0;

                            selectedType = options.FindIndex((x) => { return x.Equals(persistentData.classToLoad[i]); });

                            if (selectedType == -1)
                                selectedType = 0;

                            selectedType = EditorGUILayout.Popup("Type", selectedType, options.ToArray());

                            persistentData.classToLoad[i] = options[selectedType];

                            if (GUILayout.Button("X", GUILayout.Width(18)))
                            {
                                persistentData.classToLoad.RemoveAt(i);
                            }

                            GUILayout.EndHorizontal();
                        }
                    }

                    EditorGUI.indentLevel--;
                    GUILayout.EndVertical();
                }

                EditorGUILayout.Space();
            }

            persistentData.Init();

            GUILayout.Space(10);

            if (persistentData.savedDataDictionnary != null)
            {
                foreach (List<SavedData> sdList in persistentData.savedDataDictionnary.Values)
                {

                    if (sdList != null && sdList.Count > 0)
                    {
                        GUILayout.BeginVertical(EditorStyles.textArea);
                        GUILayout.Space(2);
                        GUIStyle option = new GUIStyle();
                        option.alignment = TextAnchor.MiddleCenter;
                        option.fontSize = 15;
                        option.fontStyle = FontStyle.Bold;
                        GUILayout.Label(sdList[0].GetType().Name, option);
                        GUILayout.Space(5);

                        foreach (SavedData sd in sdList)
                        {
                            AutoClassInspectorExtension.ShowAutoEditorGUI(sd);
                            GUILayout.Space(2);
                        }

                        GUILayout.EndVertical();
                        GUILayout.Space(2);
                    }
                }

                SaveDataEditor();
            }

            GUILayout.Space(10);
            LoadDataEditor();

            GUILayout.Space(10);
            if (GUILayout.Button("Unload saved data"))
                persistentData.UnloadAllSavedData();

            GUILayout.Space(10);

            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Persisdent Data System");
            }
        }


        void SaveDataEditor()
        {
            PersistentDataSystem persistentData = ((PersistentDataSystem)target);

            GUILayout.BeginVertical(EditorStyles.textArea);
            GUILayout.Space(2);
            GUIStyle option = new GUIStyle();
            option.alignment = TextAnchor.MiddleCenter;
            option.fontSize = 15;
            option.fontStyle = FontStyle.Bold;
            GUILayout.Label("SAVE", option);
            GUILayout.Space(5);

            if (persistentData.savedDataDictionnary.Count > 0 && GUILayout.Button("Save as player Data"))
            {
                persistentData.SaveAllData();
                Debug.Log("Data Saved in the Directory : " + persistentData.automaticPlayerSavedDataDirectoryPath);
            }

            if (persistentData.savedDataDictionnary.Count > 0 && GUILayout.Button("Save as default Data"))
            {
                persistentData.SaveAllData(PersistentDataSystem.PathMode.DEFAULT);
                AssetDatabase.Refresh(ImportAssetOptions.Default);

                if (persistentData.saveMode == PersistentDataSystem.SaveMode.SINGLE_FILE)
                    Debug.Log("Data Saved in the Directory : " + persistentData.singleDefaultFileDirectoryPath);
                else
                    Debug.Log("Data Saved in the Directory : " + persistentData.multipleDefaultFilesDirectoryPath);
            }

            GUILayout.EndVertical();
            GUILayout.Space(2);
        }


        void LoadDataEditor()
        {
            PersistentDataSystem persistentData = ((PersistentDataSystem)target);

            GUILayout.BeginVertical(EditorStyles.textArea);
            GUILayout.Space(2);
            GUIStyle option = new GUIStyle();
            option.alignment = TextAnchor.MiddleCenter;
            option.fontSize = 15;
            option.fontStyle = FontStyle.Bold;
            GUILayout.Label("LOAD", option);
            GUILayout.Space(20);

            option.fontSize = 10;
            GUILayout.Label("Player", option);
            GUILayout.Space(5);

            if (persistentData.awakeLoadMode == PersistentDataSystem.AwakeLoadMode.SPECIFIC_CLASS && GUILayout.Button("Load specific player class data"))
                persistentData.LoadClass(persistentData.classToLoad);

            GUILayout.Space(2);
            if (GUILayout.Button("Load all player saved data"))
                persistentData.LoadAllSavedData();

            GUILayout.Space(2);
            if (GUILayout.Button("Erase all player saved data"))
                persistentData.EraseAllSavedData();

            GUILayout.Space(10);
            GUILayout.Label("Default", option);
            GUILayout.Space(5);

            if (persistentData.awakeLoadMode == PersistentDataSystem.AwakeLoadMode.SPECIFIC_CLASS && GUILayout.Button("Load specific default class data"))
                persistentData.LoadClass(persistentData.classToLoad, PersistentDataSystem.PathMode.DEFAULT);

            GUILayout.Space(2);
            if (GUILayout.Button("Load all default saved data"))
                persistentData.LoadAllSavedData(PersistentDataSystem.PathMode.DEFAULT);

            GUILayout.Space(2);
            if (GUILayout.Button("Erase all default saved data"))
            {
                persistentData.EraseAllSavedData(PersistentDataSystem.PathMode.DEFAULT);
                AssetDatabase.Refresh(ImportAssetOptions.Default);
            }

            GUILayout.EndVertical();
            GUILayout.Space(2);
        }
    }
}