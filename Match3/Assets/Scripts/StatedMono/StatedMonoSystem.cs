using BC_Solution;


/// <summary>
/// The main purpose is to simplify and synchronize stated entities
/// </summary>
public class StatedMonoSystem : MonoSystem<StatedMono>
{
    public override void OnRemoveEntities()
    {
        //Will see later
    }


    private void Update()
    {
        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].CheckForNextState();
        }

        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].UpdateBehaviour();
        }
    }

}
