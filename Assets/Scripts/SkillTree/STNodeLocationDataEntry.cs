using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class STNodeLocationDataEntry //: ScriptableObject
{
    public Skill skill;
    public Vector2 position;

    public STNodeLocationDataEntry(Skill mySkill, Vector2 myPosition)
    {
        skill = mySkill;
        position = myPosition;
    }
}
