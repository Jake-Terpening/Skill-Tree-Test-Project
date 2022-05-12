using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtonObject : MonoBehaviour
{
    public Skill skill;
    private Text skillNameText;
    public SkillTreeGameObject skillTreeGO;
    private SkillTree skillTree;
    public Image connectingLineDown;
    public Image connectingLineUp;


    private void Awake()
    {
        //skillTreeGO = gameObject.GetComponentInParent<SkillTreeGameObject>();
        if(null != skillTreeGO)
        {
            skillTree = skillTreeGO.skillTree;
        }
        skillNameText = this.gameObject.GetComponentInChildren<Text>();
        if(null != skill)
        {
            skillNameText.text = skill.GetName();
        }
        else
        {
            skill = new Skill(skillNameText.text);
        }
    }


    private void Start()
    {
        if(skillTreeGO == null)
        {
            skillTreeGO = gameObject.GetComponentInParent<SkillTreeGameObject>();
        }
        skillTreeGO.AddSkillButton(this);
    }

    public void SetSkill(Skill newSkill)
    {
        skill = newSkill;
        skillNameText.text = skill.GetName();
    }

    public List<SkillButtonObject> GetChildButtons()
    {
        List<SkillButtonObject> children = new List<SkillButtonObject>();
        foreach(Skill childSkill in skill.childSkills)
        {
            if(skillTree == null)
            {
                skillTree = skillTreeGO.skillTree;
            }
            if(!children.Contains(skillTreeGO.skillToButtonDict[childSkill]) && (skillTreeGO.skillToButtonDict.ContainsKey(childSkill)))
            {
                children.Add(skillTreeGO.skillToButtonDict[childSkill]);
            }
        }
        return children;
    }

    public void ActivateConnectingLineUp(bool turnOn)
    {
        connectingLineUp.enabled = turnOn;
    }
    public void ActivateConnectingLineDown(bool turnOn)
    {
        connectingLineDown.enabled = turnOn;
    }
    /*public bool OnClick()
    {
        if (!skill.isLearned()) 
        {
            return skill.Learn();
        }
        else
        {
            skill.Forget();
        }
        return false;
    }*/
}
