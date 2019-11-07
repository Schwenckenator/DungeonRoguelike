using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2IntPairTEST : MonoBehaviour
{
    public List<Vector2IntPair> testList;

    // Start is called before the first frame update
    void Start()
    {
        Test();
    }

    void Test() {
        for(int i = 0; i < testList.Count; i++) {
            for(int j=0; j< testList.Count; j++) {
                Debug.Log($"Checking equality of {testList[i].ToString()} and {testList[j].ToString()}.\n" +
                    $"It is {testList[i].Equals(testList[j])}.");

                
            }
        }
    }
}
