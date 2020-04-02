using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An serialisable max-included range
/// </summary>
[System.Serializable]
public struct RangeInt{
    public int min;
    public int max;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max">Max inclusive</param>
    public RangeInt(int min, int max) {
        if(min > max) {
            Debug.LogError("Minimum is greater than Maximum! Wrong way around!");
            throw new System.Exception();
        }

        this.min = min;
        this.max = max;
    }

    public int GetRandom() {
        return Random.Range(min, max+1);
    }
    
}
[System.Serializable]
public struct RangeFloat {
    public float min;
    public float max;

    public RangeFloat(float min, float max) {

        if (min > max) {
            Debug.LogError("Minimum is greater than Maximum! Wrong way around!");
            throw new System.Exception();
        }

        this.min = min;
        this.max = max;
    }

    public float GetRandom() {
        return Random.Range(min, max);
    }
}
