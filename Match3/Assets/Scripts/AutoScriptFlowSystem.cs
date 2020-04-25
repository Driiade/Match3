using BC_Solution;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// System to Auto Awake/Start script after game itself Awake / Start 
/// </summary>
public class AutoScriptFlowSystem : MonoBehaviour
{

    public bool autoIAwake = false;
    public bool autoIStart = false;

    private void Awake()
    {
        GameObjectExtensions.OnGameObjectInstantiate += AutoFlow;
        SceneManager.sceneLoaded += AutoFlow;
    }

    void AutoFlow(GameObject go)
    {
        if(autoIAwake)
        {
            IAwakable[] awakables = go.GetComponentsInChildren<IAwakable>(true);
            for (int i = 0; i < awakables.Length; i++)
            {
                awakables[i].IAwake();
            }
        }


        if (autoIStart)
        {
            IStartable[] startables = go.GetComponentsInChildren<IStartable>(true);
            for (int i = 0; i < startables.Length; i++)
            {
                startables[i].IStart();
            }
        }
    }

    void AutoFlow(Scene s, LoadSceneMode loadSceneMode)
    {
       GameObject[] go = s.GetRootGameObjects();
        foreach (var item in go)
        {
            if (autoIAwake)
            {
                IAwakable[] awakables = item.GetComponentsInChildren<IAwakable>(true);
                for (int i = 0; i < awakables.Length; i++)
                {
                    awakables[i].IAwake();
                }
            }


            if (autoIStart)
            {
                IStartable[] startables = item.GetComponentsInChildren<IStartable>(true);
                for (int i = 0; i < startables.Length; i++)
                {
                    startables[i].IStart();
                }
            }
        }
    }
}
