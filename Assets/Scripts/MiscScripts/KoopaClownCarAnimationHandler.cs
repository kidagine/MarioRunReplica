using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoopaClownCarAnimationHandler : MonoBehaviour
{

    [SerializeField] Bowser bowser;


    public void TriggerInstantiateAttackPrefab(GameObject attackPrefab)
    {
        bowser.InstantiateAttackPrefab(attackPrefab);
    }

}
