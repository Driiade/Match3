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
using System.Collections;
using System.Collections.Generic;


namespace BC_Solution
{
    [System.Serializable]
    public class ObjectPool : MonoBehaviour
    {
        public enum Mode { INIT_ON_ENABLE, INIT_ON_AWAKE, INIT_ON_START, NONE}

        public Action<GameObject> OnGameObjectInstantiate;

        public Mode mode;

        public GameObject copyGameObject;
        public Transform parent;

        [Space(20)]
        public int bufferCount = 10;            //Min of gameObject instantiate
        public int maxCount = 10;               //Max of gameObject instantiate

        public List<GameObject> pooledGameObjects = new List<GameObject>();
        public List<GameObject> unPooledGameObjects = new List<GameObject>();
        public List<GameObject> allGameObjects = new List<GameObject>();

        public float timeBetweenEachInstantiate = 0.05f;

        Coroutine initCoroutine = null;

        private void Awake()
        {
            if (mode == Mode.INIT_ON_AWAKE)
            {
                if (initCoroutine != null)
                {
                    StopCoroutine(initCoroutine);
                    initCoroutine = null;
                }
                initCoroutine = StartCoroutine(Init());
            }
        }

        private void Start()
        {
            if(mode == Mode.INIT_ON_START)
            {
                if (initCoroutine != null)
                {
                    StopCoroutine(initCoroutine);
                    initCoroutine = null;
                }
                initCoroutine = StartCoroutine(Init());
            }
        }

        private void OnEnable()
        {
            if(mode == Mode.INIT_ON_ENABLE)
            {
                if (initCoroutine != null)
                {
                    StopCoroutine(initCoroutine);
                    initCoroutine = null;
                }
                initCoroutine = StartCoroutine(Init());
            }
        }

        GameObject InstantiateGameObject(GameObject prefab)
        {
            GameObject newObj = Instantiate(prefab) as GameObject;
            newObj.transform.SetParent(transform);
            if (newObj.transform.parent == null)
            {
                newObj.transform.SetParent(transform);
            }
            newObj.transform.localScale = Vector3.one;
            return newObj;
        }

        public IEnumerator Init()
        {
            DestroyAll();

            yield return new WaitForSeconds(UnityEngine.Random.Range(0,timeBetweenEachInstantiate));

            allGameObjects.Clear();
            pooledGameObjects.Clear();
            unPooledGameObjects.Clear();

            for (int i = 0; i < bufferCount; i++)
            {
                GameObject obj = InstantiateGameObject(copyGameObject);
                pooledGameObjects.Add(obj);
                allGameObjects.Add(obj);

                obj.SetActive(false);
                obj.transform.SetParent(parent);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;

                if (OnGameObjectInstantiate != null)
                    OnGameObjectInstantiate(obj);

                PooledElement pe = obj.GetComponent<PooledElement>();
                if (!pe)
                    pe = obj.AddComponent<PooledElement>();

                pe.Init(this);

                if(timeBetweenEachInstantiate > 0)
                    yield return new WaitForSeconds(timeBetweenEachInstantiate);
            }
        }

        public void DestroyAll()
        {
            PoolAll();

            for (int i = allGameObjects.Count-1; i >= 0; i--)
            {
                if (allGameObjects[i] != copyGameObject)
                {
                    Destroy(allGameObjects[i]);
                    allGameObjects.RemoveAt(i);
                }
            }

            pooledGameObjects.Clear();
        }


        /// <summary>
        /// Gets a new object for the name type provided.  If no object of that type in the pool then <c>null</c> will be returned.
        /// </summary>
        /// <returns>
        /// The object for request prefab name.
        /// </returns>
        /// <param name='objectName'>
        /// Object prefab name
        /// </param>
        public GameObject GetFromPool()
        {
            return GetFromPool(false);
        }


        /// <summary>
        /// Gets a new object for the name type provided.  If no object type exists or if onlypooled is true and there is no objects of that type in the pool
        /// then null will be returned.
        /// </summary>
        /// <returns>
        /// The object for type.
        /// </returns>
        /// <param name='objectType'>
        /// Object type.
        /// </param>
        /// <param name='onlyPooled'>
        /// If true, it will only return an object if there is one currently pooled.
        /// </param>
        public GameObject GetFromPool(bool onlyPooled)
        {
            return GetFromPool(onlyPooled, true);
        }



        /// <summary>
        /// Gets a new object for the name type provided.  If no object type exists or if onlypooled is true and there is no objects of that type in the pool
        /// then null will be returned.
        /// </summary>
        /// <returns>
        /// The object for type.
        /// </returns>
        /// <param name='objectType'>
        /// Object type.
        /// </param>
        /// <param name='onlyPooled'>
        /// If true, it will only return an object if there is one currently pooled.
        /// </param>
        public GameObject GetFromPool(bool onlyPooled, bool activate)
        {
            GameObject obj = null;

            if (pooledGameObjects.Count > 0)
            {
                obj = pooledGameObjects[0];
                pooledGameObjects.RemoveAt(0);

                obj.SetActive(activate);
            }
            else if (!onlyPooled && allGameObjects.Count < maxCount)
            {
                obj = InstantiateGameObject(copyGameObject);
                obj.SetActive(activate);

                obj.transform.SetParent(parent, false);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;

                if (OnGameObjectInstantiate != null)
                    OnGameObjectInstantiate(obj);

                PooledElement pe = obj.GetComponent<PooledElement>();
                if (pe)
                    pe.Init(this);

                allGameObjects.Add(obj);       
            }

            if (obj)
            {
                obj.transform.SetParent(parent, false);
                unPooledGameObjects.Add(obj);
            }

            return obj;
        }

        /// <summary>
        /// Pool the object
        /// </summary>
        /// <param name="obj"></param>
        public void Pool(GameObject obj)
        {
            if (!pooledGameObjects.Contains(obj))
            {
                pooledGameObjects.Add(obj);
                unPooledGameObjects.Remove(obj);
            }

            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.SetActive(false);
        }

        public void PoolAll()
        {
            for (int i = 0; i < allGameObjects.Count; i++)
            {
                Pool(allGameObjects[i]);
            }
        }

        /// <summary>
        /// Extract from pool list this element.
        /// </summary>
        /// <param name="obj"></param>
        public void ExtractFromPool(GameObject obj)
        {
            allGameObjects.Remove(obj);
            pooledGameObjects.Remove(obj);
            unPooledGameObjects.Remove(obj);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (bufferCount > maxCount)
                maxCount = bufferCount;
        }
#endif
    }
}