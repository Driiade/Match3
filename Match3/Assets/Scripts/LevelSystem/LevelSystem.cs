using BC_Solution;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem : StatedMono<LevelStateEnum>
{
    public class StartState : State
    {
        float timer = 0f;
        public override void OnEnter(StatedMono<LevelStateEnum> statedMono)
        {
            IAwakable[] awakables = GameObjectExtensions.FindObjectsOfTypeAll<IAwakable>();

            for (int i = 0; i < awakables.Length; i++)
            {
                awakables[i].IAwake(); //Awake all entities in the level
            }


            timer = ((LevelSystem)statedMono).clock.CurrentRenderTime +0.5f;
        }

        public override void OnExit(StatedMono<LevelStateEnum> statedMono)
        {
          //
        }

        public override void OnUpdate(StatedMono<LevelStateEnum> statedMono)
        {
           if(((LevelSystem)statedMono).clock.CurrentRenderTime > timer)
            {          
                ((LevelSystem)statedMono).SwitchTo(LevelStateEnum.RUN);
            }
        }
    }

    public class RunState : State
    {
        public override void OnEnter(StatedMono<LevelStateEnum> statedMono)
        {
            ServiceProvider.GetService<Grid>().Generate(((LevelSystem)statedMono).gridSize);
        }

        public override void OnExit(StatedMono<LevelStateEnum> statedMono)
        {
            //
        }

        public override void OnUpdate(StatedMono<LevelStateEnum> statedMono)
        {

        }
    }

    [SerializeField]
    Vector2 gridSize;

    [SerializeField]
    AbstractClockMono clock;

    void Start()
    {
        Add(LevelStateEnum.START, new StartState());
        Add(LevelStateEnum.RUN, new RunState());

        SwitchTo(LevelStateEnum.START);

    }

}
