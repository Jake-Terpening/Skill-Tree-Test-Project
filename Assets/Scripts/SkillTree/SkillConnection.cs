using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillConnection
{
    public Skill parentSkill;

    //These are the prerequisites for the parent skill (usually will just be 1)
    public List<Skill> prerequisiteSkills;

    public bool IsEmpty()
    {
        return (prerequisiteSkills.Count == 0);
    }

    public void Add(Skill child)
    {
        prerequisiteSkills.Add(child);
    }
    public void Remove(Skill child)
    {
        prerequisiteSkills.Remove(child);
    }
    public SkillConnection(Skill parent, List<Skill> children = null)
    {
        parentSkill = parent;
        if (children == null)
        {
            prerequisiteSkills = new List<Skill>();
        }
        else
        {
            prerequisiteSkills = children;
        }
    }
}
