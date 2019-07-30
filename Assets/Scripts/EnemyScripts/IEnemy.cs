using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    void Walk();
    void Hit(int killstreak);
    void Stomped(int killStreak);
    void IsHopedOn(bool value);
}
