public interface ILightSubject<T>
{
    void RegisterObserver(ILightObserver<T> observer);
    void UnregisterObserver(ILightObserver<T> observer);
    void Notify(T newValue);
}