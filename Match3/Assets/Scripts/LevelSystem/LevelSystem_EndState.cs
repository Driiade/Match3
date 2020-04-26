using BC_Solution;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class LevelSystem
{
    public class EndState : State
    {
        private float timer;
        private Coroutine loadingCoroutine;

        public override LevelStateEnum CheckForNextState(StatedMono<LevelStateEnum> statedMono)
        {      
            return this.stateType;
        }

        public override void OnEnter(StatedMono<LevelStateEnum> statedMono)
        {
            LevelSystem levelSystem = statedMono as LevelSystem;

            int currentScore = ServiceProvider.GetService<ScoreSystem>().GetScore();
            PersistentDataSystem persistentDataSystem = ServiceProvider.GetService<PersistentDataSystem>();
            ScoreSavedData scoreSavedData = persistentDataSystem.GetSavedData<ScoreSavedData>();
            int bestScore = scoreSavedData.bestScore;

            if (bestScore < currentScore)
            {
                scoreSavedData.bestScore = currentScore;
                persistentDataSystem.SaveData(scoreSavedData);
            }

            ServiceProvider.GetService<ResultScreenView>().Show(currentScore, scoreSavedData.bestScore);

            IPausable[] pausables = GameObjectExtensions.FindObjectsOfTypeAll<IPausable>(true);

            for (int i = 0; i < pausables.Length; i++)
            {
                pausables[i].Pause(); //Awake all entities in the level
            }

            timer = levelSystem.clock.CurrentRenderTime + 5f;
        }

        public override void OnExit(StatedMono<LevelStateEnum> statedMono)
        {
            //
        }

        public override void OnUpdate(StatedMono<LevelStateEnum> statedMono)
        {
            LevelSystem levelSystem = statedMono as LevelSystem;
            if (loadingCoroutine == null && timer < levelSystem.clock.CurrentRenderTime)
            {
                AutoScriptFlowSystem autoScriptFlow = ServiceProvider.GetService<AutoScriptFlowSystem>();
                autoScriptFlow.autoIAwake = false;
                autoScriptFlow.autoIStart = false;
                loadingCoroutine = ServiceProvider.GetService<CoroutineProvider>().StartCoroutine(levelSystem.gameRootScenePackage.LoadScenePackageCoroutine());
            }
        }
    }
}
