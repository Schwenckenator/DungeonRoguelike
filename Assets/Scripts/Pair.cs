using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pair<T>
{

    public T value1;
    public T value2;


    public Pair(T value1, T value2) {
        this.value1 = value1;
        this.value2 = value2;
    }

    public bool Contains(T value) {
        return value.Equals(value1) || value.Equals(value2);
    }

    public override bool Equals(object obj) {

        if (!(obj is Pair<T>)) return false;

        Pair<T> vObj = (Pair<T>)obj;
        //This should return true if the two values are equal, not considering the order

        //if (this.value1 != vObj.value1 && this.value1 != vObj.value2) return false;
        //if (this.value2 != vObj.value1 && this.value2 != vObj.value2) return false;

        if (value1.Equals(vObj.value1)) {
            return value2.Equals(vObj.value2);
        } else if (value1.Equals(vObj.value2)) {
            return value2.Equals(vObj.value1);
        } else {
            return false;
        }
    }

    public override int GetHashCode() {
        var hashCode = 1200061873;
        hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(value1);
        hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(value2);
        return hashCode;
    }

    public override string ToString() {
        return $"{value1.ToString()}, {value2.ToString()}";
    }
}
