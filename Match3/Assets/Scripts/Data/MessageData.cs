

/// <summary>
/// A simple message to communicate
/// </summary>
public struct MessageData
{
    public MessageData(string message)
    {
        this.message = message;

    }

    public string message;
}



/// <summary>
/// A simple message to communicate
/// </summary>
public struct MessageData<T>
{
    public MessageData(string message, T obj)
    {
        this.message = message;
        this.obj = obj;
    }

    public string message;
    public T obj;
}
