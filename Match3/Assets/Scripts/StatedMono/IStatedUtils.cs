using System;

public class IStatedUtils
{
    public static Action<IStated> OnStartBehaviour;
    public static Action<IStated> OnPauseBehaviour;
    public static Action<IStated> OnStopBehaviour;
}