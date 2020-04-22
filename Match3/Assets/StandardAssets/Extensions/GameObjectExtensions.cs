// Old Skull Games
// Bernard Barthelemy
// Tuesday, August 29, 2017
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
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace BC_Solution
{
    /// <summary>
    /// Helpers for game objects 
    /// </summary>
    public static class GameObjectExtensions
    {
        public static bool IsInLayerMask(this GameObject gameObject, LayerMask layerMask)
        {
            return 0 != (layerMask.value & (1 << gameObject.layer));
        }

        public static T[] FindObjectsOfTypeAll<T>(bool includeInactive = false)
        {
            List<T> results = new List<T>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);
                if (s.isLoaded)
                {
                    GameObject[] roots = s.GetRootGameObjects();
                    for (int j = 0; j < roots.Length; j++)
                    {
                        results.AddRange(roots[j].GetComponentsInChildren<T>(includeInactive));
                    }
                }
            }

            return results.ToArray();
        }

        public static T[] FindObjectsOfTypeAll<T>(string sceneName, bool includeInactive = false)
        {
            List<T> results = new List<T>();

            Scene s = SceneManager.GetSceneByName(sceneName);

            if (s.isLoaded)
            {
                GameObject[] roots = s.GetRootGameObjects();
                for (int j = 0; j < roots.Length; j++)
                {
                    results.AddRange(roots[j].GetComponentsInChildren<T>(includeInactive));
                }
            }

            return results.ToArray();
        }

        public static Component EnsureExists(this GameObject go, Type componentType)
        {
            Component c = go.GetComponent(componentType);
            if (!c)
            {
                c = go.AddComponent(componentType);
            }

            return c;
        }


        /// <summary>
        /// Return all gamobjects (with inactive) in a specified tag
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static GameObject[] FindObjectsWithTagAll(string tag)
        {
            List<GameObject> taggedObjects = new List<GameObject>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);
                if (s.isLoaded)
                {
                    GameObject[] rootGameobjects = s.GetRootGameObjects();

                    for (int j = 0; j < rootGameobjects.Length; j++)
                    {
                        taggedObjects.AddRange(FindObjectsWithTagAll(rootGameobjects[j], tag));
                    }
                }
            }

            return taggedObjects.ToArray();
        }

        public static GameObject[] FindObjectsWithTagAll(GameObject root, string tag)
        {
            List<GameObject> taggedObjects = new List<GameObject>();

            if (root.gameObject.tag == tag)
                taggedObjects.Add(root.gameObject);

            Transform transform = root.transform;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform currentTransform = transform.GetChild(i);
                if (currentTransform.gameObject.tag == tag)
                    taggedObjects.Add(currentTransform.gameObject);

                taggedObjects.AddRange(FindObjectsWithTagAll(currentTransform.gameObject, tag));
            }

            return taggedObjects.ToArray();
        }


        /// <summary>
        /// Get all childs of a gameObject
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static GameObject[] GetChildren(GameObject root)
        {
            List<GameObject> childs = new List<GameObject>();

            Transform transform = root.transform;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform currentTransform = transform.GetChild(i);
                childs.Add(currentTransform.gameObject);
                childs.AddRange(GetChildren(currentTransform.gameObject));
            }

            return childs.ToArray();
        }

        public static Action<GameObject> OnGameObjectInstantiate;

        /// <summary>
        /// Like a normal instantiate, but call OnGameObjectInstantiate callback
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public static GameObject InstantiateWithCallback(GameObject prefab)
        {
            GameObject go = GameObject.Instantiate(prefab);
            OnGameObjectInstantiate?.Invoke(go);
            return go;
        }
    }
}
