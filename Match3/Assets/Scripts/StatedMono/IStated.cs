/// <summary>
/// Used with StatedMonoSystem
/// </summary>
public interface IStated
{
    bool isRunning { get; }
    void CheckForEnteringState();
    void CheckForNextState();
    void UpdateBehaviour();
}

