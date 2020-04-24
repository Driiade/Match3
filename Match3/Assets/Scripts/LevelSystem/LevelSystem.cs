using BC_Solution;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class LevelSystem : StatedMono<LevelStateEnum>
{
    [SerializeField]
    Vector2 gridSize;

    [SerializeField]
    AbstractClockMono clock;

    void Start()
    {
        Add(LevelStateEnum.START, new StartState());
        Add(LevelStateEnum.RUN, new RunState());

        StartBehaviour(LevelStateEnum.START);
    }

}
