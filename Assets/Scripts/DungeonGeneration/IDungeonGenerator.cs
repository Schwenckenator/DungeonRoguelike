﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDungeonGenerator
{
    void AttemptToGenerateDungeon(Dungeon newDungeon);
    IEnumerator GenerateDungeon(Dungeon newDungeon);
}
