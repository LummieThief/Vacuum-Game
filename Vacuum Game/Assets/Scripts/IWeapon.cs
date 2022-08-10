using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public void AttackDown();
    public void AttackUp();
    public void AltAttackDown();
    public void AltAttackUp();
    public float GetSlowPlayerMult();
    public bool IsAttacking();
    public void Activate();
    public void Deactivate();
}
