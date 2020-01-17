using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room List", menuName = "New Room List", order = 59)]
public class RoomList : ScriptableObject
{
    public Texture2D[] rooms;
}
