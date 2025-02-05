using System;
using UnityEngine;

public enum BuffType { LockStaminaBuff, IncreaseFullness }

[Serializable]
public class BuffEffect
{
    public BuffType type;
    public float value;
    public bool isPermanent;
    public float effectDuration;

    public void CreateAndApplyEffect()
    {
        Type fxType = Type.GetType(type.ToString());
        if (fxType != null)
        {
            BuffEffect buff = (BuffEffect)Activator.CreateInstance(fxType);
            buff.Init(this).ApplyEffect();
        }
    }

    public BuffEffect Init(BuffEffect buff)
    {
        type = buff.type;
        value = buff.value;
        isPermanent = buff.isPermanent;
        effectDuration = buff.effectDuration;
        
        return this;
    }

    public virtual void ApplyEffect() { }

}


public class LockStaminaBuff : BuffEffect
{
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        Debug.Log("LockStamina " + effectDuration.ToString());
    }
}

public class IncreaseFullness : BuffEffect
{
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        Debug.Log("LockStamina " + effectDuration.ToString());
    }
}