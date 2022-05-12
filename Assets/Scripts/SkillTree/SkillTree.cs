using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillTree", menuName = "ScriptableObjects/SkillTreeScriptableObject", order = 1)]

public class SkillTree : ScriptableObject
{
    public Dictionary<string, Skill> nameSkillDict;
    public List<Skill> skillList;
    public List<SkillConnection> connectionList;

    public int skillPoints;

    public SkillTree(int startingPoints=0)
    {
        nameSkillDict = new Dictionary<string, Skill>();
        skillList = new List<Skill>();
        connectionList = new List<SkillConnection>();
        skillPoints = startingPoints;
    }
    public void AddSkill(Skill skill)
    {
        if(nameSkillDict == null)
        {
            nameSkillDict = new Dictionary<string, Skill>();
        }
        if (nameSkillDict.ContainsKey(skill.GetName()))
        {
            Debug.LogWarning($"Attempted to add a skill with an existing name: {skill.GetName()}");
            return;
        }
        skillList.Add(skill);
        nameSkillDict.Add(skill.GetName(), skill);
        //skillChildrenDict.Add(skill, skill.childSkills);
    }

    public void AddChildSkill(Skill child, Skill parent)
    {
        if (nameSkillDict == null)
        {
            nameSkillDict = new Dictionary<string, Skill>();
        }
        if (!nameSkillDict.ContainsKey(parent.GetName()))
        {
            Debug.LogError($"Could not find parent skill with name: {parent.GetName()}");
            return;
        }
        if (!nameSkillDict.ContainsKey(child.GetName()))
        {
            nameSkillDict.Add(child.GetName(), child);
            skillList.Add(child);
        }
        //skillChildrenDict.Add(child, child.childSkills);
        parent.AddChild(child);
    }

    public void EditSkill(Skill skill, string name, int cost, bool learned)
    {
        if (nameSkillDict == null)
        {
            nameSkillDict = new Dictionary<string, Skill>();
        }
        if (nameSkillDict.ContainsKey(skill.GetName()))
        {
            Debug.LogError($"Could not find skill with name: {skill.GetName()}");
            return;
        }
        skill.Edit(name, cost, learned);
    }

    public void SyncDataStructures()
    {
        if (nameSkillDict == null)
        {
            nameSkillDict = new Dictionary<string, Skill>();
        }
        foreach (Skill skill in skillList)
        {
            if(!nameSkillDict.ContainsKey(skill.GetName()))
            {
                nameSkillDict.Add(skill.GetName(), skill);
            }
        }
    }
}
