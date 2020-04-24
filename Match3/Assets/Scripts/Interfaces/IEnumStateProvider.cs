using System;


/// <summary>
/// Simple interface to provide state by an Enum
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IEnumStateProvider<T> where T : Enum
{
   T CurrentStateType { get; }
}
