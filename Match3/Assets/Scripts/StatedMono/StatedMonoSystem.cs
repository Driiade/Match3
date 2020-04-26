using BC_Solution;
using System.Collections.Generic;

/// <summary>
/// The main purpose is to simplify and synchronize stated entities
/// </summary>
public class StatedMonoSystem : MonoSystem<IStated>
{
    List<IStated> entitiesToAdd = new List<IStated>();

    public override void OnRemoveEntities()
    {
        for (int i = 0; i < entities.Count; i++)
        {
            entitiesToAdd.Remove(entities[i]);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        IStatedUtils.OnStartBehaviour += AddEntity;
        IStatedUtils.OnPauseBehaviour += RemoveEntity;
        IStatedUtils.OnStopBehaviour += RemoveEntity;
    }

    private void OnDestroy()
    {
        IStatedUtils.OnStartBehaviour -= AddEntity;
        IStatedUtils.OnPauseBehaviour -= RemoveEntity;
        IStatedUtils.OnStopBehaviour -= RemoveEntity;
    }


    private void Update()
    {
        for (int i = 0; i < entitiesToAdd.Count; i++)
        {
            if(!entities.Contains(entitiesToAdd[i]))
                entities.Add(entitiesToAdd[i]);
        }

        entitiesToAdd.Clear();

        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].CheckForNextState();
        }

        for (int i = entities.Count-1; i >=0 ; i--)
        {
            if (!entities[i].isRunning)
            {
                entities.RemoveAt(i);
            }
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

    public override void OnNewEntities(IStated[] entities)
    {
        for (int i = 0; i < entities.Length; i++)
        {
            if (entities[i].isRunning)
                AddEntity(entities[i]);
        }
    }

    public override void AddEntity(IStated entity)
    {
        if (!entitiesToAdd.Contains(entity))
            entitiesToAdd.Add(entity);
    }

    public override void RemoveEntity(IStated entity)
    {
        entitiesToAdd.Remove(entity);
    }

}
