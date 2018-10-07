using System.Collections.Generic;

public class Subject<T> : ILightSubject<T>
{
    private List<ILightObserver<T>> _observers;

    private T _value;

    public Subject(T value = default(T))
    {
        _observers = new List<ILightObserver<T>>();
        Value = value;
    }

    public T Value
    {
        get { return _value; }
        set
        {
            _value = value;
            Notify(_value);
        }
    }

    public void RegisterObserver(ILightObserver<T> observer)
    {
        _observers.Add(observer);
    }

    public void UnregisterObserver(ILightObserver<T> observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(T newValue)
    {
        foreach (var observer in _observers)
        {
            observer.OnUpdate(newValue);
        }
    }
}