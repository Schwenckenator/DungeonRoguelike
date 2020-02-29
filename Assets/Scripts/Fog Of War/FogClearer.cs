using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogClearer : MonoBehaviour
{
    private Dungeon dungeon;

    // Start is called before the first frame update
    private void Start()
    {
        dungeon = GameObject.FindGameObjectWithTag("Dungeon").GetComponent<Dungeon>();
        Room room = dungeon.GetRoomOfPosition(transform.position.RoundToInt());
        FogOfWar.Instance.SetFog(room.Bounds, FogState.visible);
    }


}
