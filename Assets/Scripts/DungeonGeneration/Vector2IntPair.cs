using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vector2IntPair
{
    public Vector2Int value1;
    public Vector2Int value2;

    public Vector2IntPair(Vector2Int value1, Vector2Int value2) {
        this.value1 = value1;
        this.value2 = value2;
    }

    public override bool Equals(object obj) {

        if (!(obj is Vector2IntPair)) return false;

        Vector2IntPair vObj = (Vector2IntPair)obj;
        //This should return true if the two values are equal, not considering the order
        
        //if (this.value1 != vObj.value1 && this.value1 != vObj.value2) return false;
        //if (this.value2 != vObj.value1 && this.value2 != vObj.value2) return false;

        if(this.value1 == vObj.value1) {
            if(this.value2 == vObj.value2) {
                return true;
            } else {
                return false;
            }
        }else if(this.value1 == vObj.value2) {
            if(this.value2 == vObj.value1) {
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    public override int GetHashCode() {
        var hashCode = 1200061873;
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector2Int>.Default.GetHashCode(value1);
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector2Int>.Default.GetHashCode(value2);
        return hashCode;
    }

    public override string ToString() {
        return $"{value1.ToString()}, {value2.ToString()}";
    }
}
