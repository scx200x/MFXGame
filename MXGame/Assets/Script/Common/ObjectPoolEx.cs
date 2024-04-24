using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ObjectPoolEx<T> where T : new()
{
    private readonly Stack<T> Stack = new Stack<T>();
    private readonly UnityAction<T> ActionOnGet;
    private readonly UnityAction<T> ActionOnRelease;

    public int CountAll { get; private set; }
    public int CountActive { get { return CountAll - CountInactive; } }
    public int CountInactive { get { return Stack.Count; } }

    public ObjectPoolEx(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
    {
        ActionOnGet = actionOnGet;
        ActionOnRelease = actionOnRelease;
    }

    public T Get()
    {
        T element;
        if (Stack.Count == 0)
        {
            element = new T();
            CountAll++;
        }
        else
        {
            element = Stack.Pop();
        }
        if (ActionOnGet != null) ActionOnGet(element);
        return element;
    }

    public void Release(T element)
    {
        if (Stack.Count > 0 && ReferenceEquals(Stack.Peek(), element))
            LogHelper.LogError("Internal error. Trying to destroy object that is already released to pool.");
        
        if (ActionOnRelease != null) ActionOnRelease(element);
        
        Stack.Push(element);
    }
}
