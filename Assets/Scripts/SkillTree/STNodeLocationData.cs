using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class STNodeLocationData //:  ScriptableObject
{
    public SkillTree skillTree;
    public List<STNodeLocationDataEntry> dataList;

    public STNodeLocationData(SkillTree myTree = null)
    {
        skillTree = myTree;
        dataList = new List<STNodeLocationDataEntry>();
    }

    public void AddEntry(Skill skill, Vector2 position)
    {
        if (dataList == null)
        {
            dataList = new List<STNodeLocationDataEntry>();
        }
        foreach (STNodeLocationDataEntry lde in dataList)
        {
            if(lde.skill == skill)
            {
                lde.position = position;
                return;
            }
        }
        dataList.Add(new STNodeLocationDataEntry(skill, position));
    }

    public void AddEntry(STNodeLocationDataEntry entry)
    {
        AddEntry(entry.skill, entry.position);
    }

    public Vector2 Position(Skill skill)
    {
        if(dataList==null)
        {
            dataList = new List<STNodeLocationDataEntry>();
            return Vector2.zero;
        }
        foreach(STNodeLocationDataEntry lde in dataList)
        {
            if(lde.skill == skill)
            {
                return lde.position;
            }
        }
        return Vector2.zero;
    }
}
