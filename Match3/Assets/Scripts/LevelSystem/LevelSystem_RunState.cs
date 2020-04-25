using BC_Solution;
using UnityEngine;

public partial class LevelSystem
{
    public class RunState : State
    {
        public override LevelStateEnum CheckForNextState(StatedMono<LevelStateEnum> statedMono)
        {
            return this.stateType;
        }

        public override void OnEnter(StatedMono<LevelStateEnum> statedMono)
        {
            ServiceProvider.GetService<AutoScriptFlowSystem>().autoIStart = true;

            ServiceProvider.GetService<Grid>().Generate(((LevelSystem)statedMono).gridSize);

            IStartable[] startables = GameObjectExtensions.FindObjectsOfTypeAll<IStartable>(true);

            for (int i = 0; i < startables.Length; i++)
            {
                startables[i].IStart(); //Awake all entities in the level
            }

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
