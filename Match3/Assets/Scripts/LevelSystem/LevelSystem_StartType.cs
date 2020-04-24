﻿using BC_Solution;

public partial class LevelSystem
{
    public class StartState : State
    {
        float timer = 0f;

        public override LevelStateEnum CheckForNextState(StatedMono<LevelStateEnum> statedMono)
        {
            if (((LevelSystem)statedMono).clock.CurrentRenderTime > timer)
            {
                return (LevelStateEnum.RUN);
            }

            return this.stateType;
        }

        public override void OnEnter(StatedMono<LevelStateEnum> statedMono)
        {
            IAwakable[] awakables = GameObjectExtensions.FindObjectsOfTypeAll<IAwakable>();

            for (int i = 0; i < awakables.Length; i++)
            {
                awakables[i].IAwake(); //Awake all entities in the level
            }

            timer = ((LevelSystem)statedMono).clock.CurrentRenderTime + 0.5f;
        }

        public override void OnExit(StatedMono<LevelStateEnum> statedMono)
        {
            //
        }

        public override void OnUpdate(StatedMono<LevelStateEnum> statedMono)
        {
            //
        }
    }
}