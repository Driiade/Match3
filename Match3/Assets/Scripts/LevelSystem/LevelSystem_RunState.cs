using BC_Solution;

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


            IStartable[] startables = GameObjectExtensions.FindObjectsOfTypeAll<IStartable>();

            for (int i = 0; i < startables.Length; i++)
            {
                startables[i].IStart(); //Awake all entities in the level
            }

        }

        public override void OnExit(StatedMono<LevelStateEnum> statedMono)
        {
            //
        }

        public override void OnUpdate(StatedMono<LevelStateEnum> statedMono)
        {

        }
    }
}
