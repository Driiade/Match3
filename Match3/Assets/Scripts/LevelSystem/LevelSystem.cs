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

    [SerializeField]
    ScriptableScenePackage gameRootScenePackage;

    void Start()
    {
        Add(LevelStateEnum.START, new StartState());
        Add(LevelStateEnum.RUN, new RunState());
        Add(LevelStateEnum.END, new EndState());

        StartBehaviour(LevelStateEnum.START);
    }

}
