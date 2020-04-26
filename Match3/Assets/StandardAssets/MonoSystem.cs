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


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BC_Solution
{
    public abstract class MonoSystem<T> : MonoBehaviour
    {

        protected List<T> entities = new List<T>();

        protected virtual void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            GameObjectExtensions.OnGameObjectInstantiate += OnGameObjectInstantiate;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                SearchNewEntities(SceneManager.GetSceneAt(i));
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            GameObjectExtensions.OnGameObjectInstantiate -= OnGameObjectInstantiate;
        }


        private void SearchNewEntities(Scene scene)
        {
            GameObject[] go = scene.GetRootGameObjects();
            for (int i = 0; i < go.Length; i++)
            {
                T[] scripts = go[i].GetComponentsInChildren<T>(true);
                OnNewEntities(scripts);
            }
        }

        private void RemoveEntities(Scene scene)
        {
            List<T> previousEntities = new List<T>(entities); //Can't check what is destroyed or not with Unity
            entities.Clear();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                GameObject[] go = SceneManager.GetSceneAt(i).GetRootGameObjects();
                for (int j = 0; j < go.Length; j++)
                {
                    T[] scripts = go[j].GetComponentsInChildren<T>(true);

                    for (int k = 0; k < scripts.Length; k++)
                    {
                        if(previousEntities.Contains(scripts[k]))
                            AddEntity(scripts[k]);
                    }
                }
            }

            OnRemoveEntities();
        }

        private void OnGameObjectInstantiate(GameObject go)
        {
            T[] scripts = go.GetComponentsInChildren<T>(true);
            OnNewEntities(scripts);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (this && scene != this.gameObject.scene)
                SearchNewEntities(scene);
        }


        private void OnSceneUnloaded(Scene scene)
        {
            RemoveEntities(scene);
        }

        public virtual void OnNewEntities(T[] entities)
        {
            for (int k = 0; k < entities.Length; k++)
            {
                AddEntity(entities[k]);
            }
        }

        public abstract void OnRemoveEntities();

        public virtual void AddEntity(T entity)
        {
            entities.Add(entity);
        }

        public virtual void RemoveEntity(T entity)
        {
            entities.Remove(entity);
        }
    }
}