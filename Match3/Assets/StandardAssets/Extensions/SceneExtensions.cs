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


using UnityEngine;
using UnityEngine.SceneManagement;


namespace BC_Solution
{
    public static class SceneExtensions {
		public static void MoveRootObjectsTo(this Scene scene, Vector3 wantedPosition)
		{
			if (!scene.isLoaded)
				UnityEngine.Debug.LogWarning("You are trying to move RootObjects of a scene that is not loaded ("+scene.name+")");
			foreach (var rootGameObject in scene.GetRootGameObjects())
			{
				rootGameObject.transform.position = wantedPosition;	
			}
		}
		
		public static void SetRootObjectsActivation(this Scene scene, bool p_active)
		{
			if (!scene.isLoaded)
				UnityEngine.Debug.LogWarning("You are trying to "+(p_active?"activate":"deactivate")+" RootObjects of a scene that is not loaded ("+scene.name+")");
			foreach (var rootGameObject in scene.GetRootGameObjects())
			{
				rootGameObject.SetActive(p_active);	
			}
		}

        public static bool ScenesAreAllLoaded()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (!SceneManager.GetSceneAt(i).isLoaded)
                    return false;
            }

            return true;
        }
    }
}
