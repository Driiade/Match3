using BC_Solution;
using UnityEngine;

public partial class LevelSystem
{
    public class RunState : State
    {

        private Timer timer;

        public override LevelStateEnum CheckForNextState(StatedMono<LevelStateEnum> statedMono)
        {
            if (timer.GetRemainingTime() <= 0)
                return LevelStateEnum.END;

            return this.stateType;
        }

        public override void OnEnter(StatedMono<LevelStateEnum> statedMono)
        {
            ServiceProvider.GetService<AutoScriptFlowSystem>().autoIStart = true;

            IStartable[] startables = GameObjectExtensions.FindObjectsOfTypeAll<IStartable>(true);

            for (int i = 0; i < startables.Length; i++)
            {
                startables[i].IStart(); //Awake all entities in the level
            }

            ServiceProvider.GetService<Grid>().Generate(((LevelSystem)statedMono).gridSize);
            timer = ServiceProvider.GetService<Timer>();
            timer.maxAllowedTime = 60f;

        }

        public override void OnExit(StatedMono<LevelStateEnum> statedMono)
        {
            ServiceProvider.GetService<AutoScriptFlowSystem>().autoIStart = false; //At the end of the game, don't start any object : the game is finished !
        }

        public override void OnUpdate(StatedMono<LevelStateEnum> statedMono)
        {

        }
    }
}
