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
using UnityEngine.SceneManagement;
using System;

namespace BC_Solution
{
    /// <summary>
    /// A package of scene that have to be loaded all together to work as expected.
    /// </summary>
   [CreateAssetMenu(fileName ="ScriptableScenePackage", menuName ="ScriptableObjects/ScriptableScenePackage")]
    public class ScriptableScenePackage : ScriptableObject
    {
        public static Action<ScriptableScenePackage> OnEndLoadingScenePackage;
        public ScriptableScenePackage[] startScenePackages = new ScriptableScenePackage[0];

        [Space(20)]
        public string[] scenes = new string[0];

    #if CHEAT || UNITY_EDITOR
    public string[] cheatScenes = new string[0];
    #endif


        [Space(20)]
        public ScriptableScenePackage[] endScenePackages = new ScriptableScenePackage[0];

        public string[] optionalScenes = new string[0];

        public string activeScene;


#if UNITY_EDITOR
        public bool showOnGUI = true;
        public string folderName;
        public bool participateToOptimization = true;
#endif


        public IEnumerator LoadScenePackageCoroutine(IEnumerator beforeRemovingSceneEnumerator = null, IEnumerator beforeActivatingSceneEnumerator = null, string[] notUnloadedScene = null, bool removeUnecessaryScene = true)
        {
            yield return null; //Wait beginning of the frame
            List<AsyncOperation> asyncAddSceneOperations = new List<AsyncOperation>();
            List<AsyncOperation> asyncDelSceneOperations = new List<AsyncOperation>();

            if (beforeRemovingSceneEnumerator != null)
            {
                yield return beforeRemovingSceneEnumerator;
            }

            if (removeUnecessaryScene)
                RemoveUnecessaryScenes(asyncDelSceneOperations, notUnloadedScene);


            for (int i = 0; i < asyncDelSceneOperations.Count; i++)
            {
                yield return asyncDelSceneOperations[i];
            }

            yield return AddNecessaryScenes(this, asyncAddSceneOperations);

            for (int i = 0; i < asyncAddSceneOperations.Count; i++)
            {
                yield return new WaitUntil(() => { return asyncAddSceneOperations[i].progress >= 0.9f; });
            }

            if (beforeActivatingSceneEnumerator != null)
            {
                yield return beforeActivatingSceneEnumerator;
            }

            float timer = Time.realtimeSinceStartup + 0.04f;
            for (int i = 0; i < asyncAddSceneOperations.Count; i++)
            {
                asyncAddSceneOperations[i].allowSceneActivation = true;
                yield return asyncAddSceneOperations[i];

                if (timer < Time.realtimeSinceStartup)
                {
                    timer = Time.realtimeSinceStartup + 0.04f;
                    yield return null; //Target 25fps
                }
            }

            yield return null; //Wait one frame to be sure Unity load the scene

            if (!String.IsNullOrEmpty(activeScene) && SceneManager.GetSceneByName(activeScene).IsValid())
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(activeScene));

            yield return Resources.UnloadUnusedAssets();
            GC.Collect();

            OnEndLoadingScenePackage?.Invoke(this);
        }


        void RemoveUnecessaryScenes(List<AsyncOperation> asyncDelSceneOperations, string[] notUnloadedScenes)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);

                if (!Array.Exists(optionalScenes, (x => { return x == s.name; })))
                {
                    if (notUnloadedScenes == null || !(Array.Exists(notUnloadedScenes, (x => { return x == s.name; }))))
                    {
                        if (SceneHasToBeRemoved(this, s.name))
                        {
                            GameObject[] roots = s.GetRootGameObjects();
                            for (int j = 0; j < roots.Length; j++)
                            {
                                roots[j].SetActive(false);
                            }

                            asyncDelSceneOperations.Add(SceneManager.UnloadSceneAsync(s.buildIndex));
                        }
                    }
                }
            }
        }



        IEnumerator AddNecessaryScenes(List<AsyncOperation> asyncAddSceneOperations, string[] scenes)
        {
            float timer = Time.realtimeSinceStartup + 0.04f;
            for (int i = 0; i < scenes.Length; i++)
            {
                Scene s = SceneManager.GetSceneByName(scenes[i]);
                if (!s.IsValid())
                {
                    AsyncOperation a = SceneManager.LoadSceneAsync(scenes[i], LoadSceneMode.Additive);
                    a.allowSceneActivation = false;
                    asyncAddSceneOperations.Add(a);

                    if (timer < Time.realtimeSinceStartup)
                    {
                        timer = Time.realtimeSinceStartup + 0.04f;
                        yield return null; //Target 25fps
                    }
                }
            }
        }

        IEnumerator AddNecessaryScenes(ScriptableScenePackage scenePackage, List<AsyncOperation> asyncAddSceneOperations)
        {
#if CHEAT
        yield return AddNecessaryScenes(asyncAddSceneOperations, scenePackage.cheatScenes);
#endif

            for (int i = 0; i < scenePackage.startScenePackages.Length; i++)
            {
                yield return AddNecessaryScenes(scenePackage.startScenePackages[i], asyncAddSceneOperations);
            }

            yield return AddNecessaryScenes(asyncAddSceneOperations, scenePackage.scenes);


            for (int i = 0; i < scenePackage.endScenePackages.Length; i++)
            {
                yield return AddNecessaryScenes(scenePackage.endScenePackages[i], asyncAddSceneOperations);
            }
        }

        /// <summary>
        /// Concat all scenes to be loaded
        /// </summary>
        /// <param name="scenePackages"></param>
        /// <returns></returns>
        public static List<string> ConcatScenes(params ScriptableScenePackage[] scenePackages)
        {
            List<string> concat = new List<string>();

            for (int i = 0; i < scenePackages.Length; i++)
            {
#if CHEAT
            concat.AddRange(scenePackages[i].cheatScenes);
#endif

                concat.AddRange(scenePackages[i].scenes);
            }

            return concat;
        }

        public static bool SceneHasToBeRemoved(ScriptableScenePackage scenePackage, string sceneName)
        {
            bool remove = true;

#if CHEAT
        remove = remove && !(Array.Exists(scenePackage.cheatScenes, ((x) => { return x == sceneName; })));
#endif

            for (int i = 0; i < scenePackage.startScenePackages.Length; i++)
            {
                remove = remove && SceneHasToBeRemoved(scenePackage.startScenePackages[i], sceneName);
            }

            remove = remove && !(Array.Exists(scenePackage.scenes, ((x) => { return x == sceneName; })));

            for (int i = 0; i < scenePackage.endScenePackages.Length; i++)
            {
                remove = remove && SceneHasToBeRemoved(scenePackage.endScenePackages[i], sceneName);
            }

            return remove;
        }

        public bool IsLoaded()
        {
            for (int i = 0; i < startScenePackages.Length; i++)
            {
                if (!startScenePackages[i].IsLoaded())
                    return false;
            }

            for (int i = 0; i < scenes.Length; i++)
            {
                if (!SceneManager.GetSceneByName(scenes[i]).isLoaded)
                    return false;
            }

#if CHEAT
        for (int i = 0; i < cheatScenes.Length; i++)
        {
            if (!SceneManager.GetSceneByName(cheatScenes[i]).isLoaded)
                return false;
        }
#endif

            for (int i = 0; i < endScenePackages.Length; i++)
            {
                if (!endScenePackages[i].IsLoaded())
                    return false;
            }

            return true;
        }
    }
}