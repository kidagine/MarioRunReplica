using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    void Walk();
    void Stomped(int killStreak);
}
