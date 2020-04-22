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
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;

namespace BC_Solution.Editor
{
    public static class EditorScenePackageExtension
    {
        public static void EditorLoadScenePackage(this ScriptableScenePackage scriptableScenePackage, bool askSaveToUser = true, bool removeScene = true)
        {
            if (askSaveToUser)
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            EditorApplication.isPaused = false;
            EditorApplication.isPlaying = false;

            for (int i = 0; i < scriptableScenePackage.startScenePackages.Length; i++)
            {
                scriptableScenePackage.startScenePackages[i].EditorLoadScenePackage(false, false);
            }

            SceneAsset[] sceneAssets = FindAssetExtension.GetAllInstances<SceneAsset>();

            for (int i = 0; i < scriptableScenePackage.scenes.Length; i++)
            {
                Scene s = EditorSceneManager.GetSceneByName(scriptableScenePackage.scenes[i]);

                if (!s.isLoaded)
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(Array.Find(sceneAssets, (x) => { return x.name == scriptableScenePackage.scenes[i]; })), OpenSceneMode.Additive);

                EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByName(scriptableScenePackage.scenes[i]));
            }

            for (int i = 0; i < scriptableScenePackage.cheatScenes.Length; i++)
            {
                Scene s = EditorSceneManager.GetSceneByName(scriptableScenePackage.cheatScenes[i]);
                if (!s.isLoaded)
                    EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(Array.Find(sceneAssets, (x) => { return x.name == scriptableScenePackage.cheatScenes[i]; })), OpenSceneMode.Additive);
            }

            for (int i = 0; i < scriptableScenePackage.endScenePackages.Length; i++)
            {
                scriptableScenePackage.endScenePackages[i].EditorLoadScenePackage(false, false);
            }

            if (removeScene)
            {
                for (int i = EditorSceneManager.sceneCount - 1; i >= 0; i--)
                {
                    Scene s = EditorSceneManager.GetSceneAt(i);

                    if (ScriptableScenePackage.SceneHasToBeRemoved(scriptableScenePackage, s.name))
                    {
                        EditorSceneManager.CloseScene(s, true);
                    }
                }
            }


            if (!String.IsNullOrEmpty(scriptableScenePackage.activeScene) && EditorSceneManager.GetSceneByName(scriptableScenePackage.activeScene).IsValid())
                EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByName(scriptableScenePackage.activeScene));
        }
    }
}