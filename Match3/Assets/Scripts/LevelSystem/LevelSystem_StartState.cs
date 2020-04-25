using BC_Solution;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class LevelSystem
{
    public class StartState : State
    {
        float timer = 0f;
        Coroutine waitCoroutine;

        public override LevelStateEnum CheckForNextState(StatedMono<LevelStateEnum> statedMono)
        {
            if (timer != -1 && ((LevelSystem)statedMono).clock.CurrentRenderTime > timer)
            {
                return (LevelStateEnum.RUN);
            }

            return this.stateType;
        }

        public override void OnEnter(StatedMono<LevelStateEnum> statedMono)
        {
            waitCoroutine = statedMono.StartCoroutine(this.WaitForSceneLoaded());
            timer = -1;
        }

        public override void OnExit(StatedMono<LevelStateEnum> statedMono)
        {
            //
        }

        public override void OnUpdate(StatedMono<LevelStateEnum> statedMono)
        {
            if(waitCoroutine == null && timer == -1)
            {
                ServiceProvider.GetService<AutoScriptFlowSystem>().autoIAwake = true;

                IAwakable[] awakables = GameObjectExtensions.FindObjectsOfTypeAll<IAwakable>(true);

                for (int i = 0; i < awakables.Length; i++)
                {
                    awakables[i].IAwake(); //Awake all entities in the level
                }

                timer = ((LevelSystem)statedMono).clock.CurrentRenderTime + 0.5f;
            }
        }

        IEnumerator WaitForSceneLoaded()
        {
            yield return new WaitUntil(()=>SceneExtensions.ScenesAreAllLoaded());
            waitCoroutine = null;
        }
      
    }
}
