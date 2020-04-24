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
            ServiceProvider.GetService<Grid>().Generate(((LevelSystem)statedMono).gridSize);

            GameObjectExtensions.OnGameObjectInstantiate += StartObject;

            IStartable[] startables = GameObjectExtensions.FindObjectsOfTypeAll<IStartable>(true);

            for (int i = 0; i < startables.Length; i++)
            {
                startables[i].IStart(); //Awake all entities in the level
            }

        }

        public override void OnExit(StatedMono<LevelStateEnum> statedMono)
        {
            GameObjectExtensions.OnGameObjectInstantiate -= StartObject; //At the end of the game, don't start any object : the game is finished !
        }

        public override void OnUpdate(StatedMono<LevelStateEnum> statedMono)
        {

        }

        void StartObject(GameObject go)
        {
            IStartable[] startable = go.GetComponentsInChildren<IStartable>(true);
            for (int i = 0; i < startable.Length; i++)
            {
                startable[i].IStart();
            }
        }
    }
}
