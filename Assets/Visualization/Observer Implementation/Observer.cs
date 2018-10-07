using System;

public class Observer<T> : ILightObserver<T>
{
    private Action<T> _onUpdateCallback;

    public Observer(Action<T> onUpdateCallback)
    {
        _onUpdateCallback = onUpdateCallback;
    }

    public void OnUpdate(T newValue)
    {
        _onUpdateCallback(newValue);
    }
}