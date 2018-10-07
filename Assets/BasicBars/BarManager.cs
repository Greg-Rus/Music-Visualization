using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicSingleFrameModel))]
public class BarManager : MonoBehaviour
{
    public BarBehavior BarPrefab;
    public float barSizeMultiplyer;

    private BasicSingleFrameModel _myFrameModel;
    // Use this for initialization
    void Start ()
    {
        _myFrameModel = GetComponent<BasicSingleFrameModel>();
        MakeBars();
    }

    private void MakeBars()
    {
        int startingPosition = _myFrameModel.NumberOfBars / 2 * -1;
        for (int i = 0; i < _myFrameModel.NumberOfBars; i++)
        {
            var bar = Instantiate(BarPrefab, new Vector3(startingPosition + i, 0, 0), Quaternion.identity);
            bar.sizeMultiplyer = barSizeMultiplyer;
            _myFrameModel.GetReactiveBarAtIndex(i).RegisterObserver(bar);
        }
    }

}
