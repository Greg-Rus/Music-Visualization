using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarBehavior : MonoBehaviour, ILightObserver<float>
{
    private Transform _myTransform;

    public float sizeMultiplyer = 1f;
	// Use this for initialization
	void Start ()
	{
	    _myTransform = GetComponent<Transform>();
	}

    public void OnUpdate(float newValue)
    {
        _myTransform.localScale = new Vector3(1, newValue * sizeMultiplyer, 1);
    }
}
