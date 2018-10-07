using System;
using System.Linq;
using UniRx;
using UnityEngine;

public interface IReactiveBarData
{
    Subject<float> GetReactiveBarAtIndex(int index);
    Subject<float[]> ReactiveFrame{get;}
    int NumberOfBars { get; }
}

public class BasicSingleFrameModel : MonoBehaviour, IReactiveBarData
{
    private Subject<float>[] _barSubjects;
    private Subject<float[]> _frameSubject;
    private IBarData _barDataProvider;
    public float BarUpdateThreshold = 0.01f;

    void Awake()
    {
        _barDataProvider = GetComponent<IBarData>();
        Debug.Log(_barDataProvider);
        _barSubjects = new Subject<float>[_barDataProvider.NumberOfBars];
        for (int i = 0; i < _barDataProvider.NumberOfBars; i++)
        {
            _barSubjects[i] = new Subject<float>(0);
        }
        _frameSubject = new Subject<float[]>(new float[_barDataProvider.NumberOfBars]);
    }

    void Update()
    {
        for (int i = 0; i < _barDataProvider.NumberOfBars; i++)
        {
            _barSubjects[i].Value = Mathf.Max(BarUpdateThreshold, _barDataProvider.BarValues[i]);
        }
        _frameSubject.Value = _barDataProvider.BarValues.ToArray();
    }

    public int NumberOfBars
    {
        get { return _barDataProvider.NumberOfBars; }
    }

    public Subject<float> GetReactiveBarAtIndex(int index)
    {
        if(index < _barSubjects.Length) return _barSubjects[index];
        else throw new ArgumentOutOfRangeException(string.Format("Requested index {0}, but array of reactive bars has lenght of {1}", index, _barSubjects.Length));
    }

    public Subject<float[]> ReactiveFrame
    {
        get { return _frameSubject; }
    }
}
