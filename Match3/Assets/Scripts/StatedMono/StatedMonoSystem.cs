using BC_Solution;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The main purpose is to simplify and synchronize stated entities
/// </summary>
public class StatedMonoSystem : MonoSystem<StatedMono>
{
    List<StatedMono> entitiesToAdd = new List<StatedMono>();


    public override void OnRemoveEntities()
    {
        //Will see later
    }

    protected override void Awake()
    {
        base.Awake();

        StatedMono.OnStartBehaviour += AddEntity;
    }


    private void Update()
    {
        for (int i = 0; i < entitiesToAdd.Count; i++)
        {
            entities.Add(entitiesToAdd[i]);
        }

        entitiesToAdd.Clear();

        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].CheckForNextState();
        }

        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].CheckForEnteringState();
        }

        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].UpdateBehaviour();
        }
    }

    public override void OnNewEntities(StatedMono[] entities)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            if (entities[i].isRunning)
                AddEntity(entities[i]);
        }
    }

    public override void AddEntity(StatedMono entity)
    {
        if (!entitiesToAdd.Contains(entity))
            entitiesToAdd.Add(entity);
    }

}
