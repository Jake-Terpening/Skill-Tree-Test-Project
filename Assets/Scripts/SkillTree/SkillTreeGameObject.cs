using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeGameObject : MonoBehaviour
{
    public SkillTree skillTree;

    public SkillButtonObject skillButtonPrefab;

    public GameObject horizontalConnectionPrefab;

    public List<SkillButtonObject> skillButtons;

    public Dictionary<Skill, SkillButtonObject> skillToButtonDict;

    private void Awake()
    {
        if (skillTree == null)
        {
            skillTree = new SkillTree();
        }
        if (skillToButtonDict == null)
        {
            skillToButtonDict = new Dictionary<Skill, SkillButtonObject>();
        }
    }
    public void AddSkillButton(SkillButtonObject sbo, bool fromTree = false)
    {
        if (!skillButtons.Contains(sbo))
        {
            skillButtons.Add(sbo);
        }
        if (!skillToButtonDict.ContainsKey(sbo.skill))
        {
            skillToButtonDict.Add(sbo.skill, sbo);
        }
        sbo.skillTreeGO = this;
        if (!fromTree)
        {
            skillTree.AddSkill(sbo.skill);
        }
    }

    public List<SkillButtonObject> GetChildButtons(SkillButtonObject skillButton)
    {
        List<SkillButtonObject> childButtons = new List<SkillButtonObject>();
        List<Skill> childSkills = skillTree.GetConnection(skillButton.skill).prerequisiteSkills;
        foreach (Skill cSkill in childSkills)
        {
            childButtons.Add(skillToButtonDict[cSkill]);
        }
        return childButtons;
    }

    public void GenerateTreeFromData(int highestRow = 10, float rowDist = 80f, float buttonWidth = 40f, float buttonHeight = 40f, float totalWidth = 400f, float xCenter =0f, float yBottom = 20f )
    {
        List<SkillButtonObject> bottomRow = new List<SkillButtonObject>();
        foreach (Skill skill in skillTree.skillList)
        {
            SkillButtonObject sbo = Instantiate<SkillButtonObject>(skillButtonPrefab, this.transform);
            sbo.SetSkill(skill);
            AddSkillButton(sbo,true);
            RectTransform sboRT = sbo.gameObject.GetComponent<RectTransform>();
            sboRT.sizeDelta = new Vector2(buttonWidth, buttonHeight);
            float xpos = xCenter;
            float ypos = yBottom;

            //outdated
            //Skill prereq = skill.GetPrereqSkill();
            //int numPrereqs = 0;
            //while (prereq != null)
            //{
            //    numPrereqs++;
            //    prereq = prereq.GetPrereqSkill();
            //}
            //ypos = yBottom + rowDist * numPrereqs;
            //if (numPrereqs == 0)
            //{
            //    bottomRow.Add(sbo);
            //}
            sboRT.anchoredPosition = new Vector2(xpos, ypos);
        }
        SetXPositions(bottomRow, totalWidth, xCenter);
    }


    public void SetXPositions(List<SkillButtonObject> row, float totalWidth, float xCenter)
    {
        for(int i = 0; i<row.Count; ++i)
        {
            float subSectionWidth = totalWidth / row.Count;
            SkillButtonObject current = row[i];
            RectTransform rt = current.gameObject.GetComponent<RectTransform>();
            float xpos = (xCenter - totalWidth / 2) + (subSectionWidth / 2 + subSectionWidth * i);
            rt.anchoredPosition = new Vector2(xpos, rt.anchoredPosition.y);
            List<SkillButtonObject> childButtons = GetChildButtons(current);
            if(childButtons.Count>0)
            {
                float horizontalLength = subSectionWidth * (childButtons.Count - 1) / (childButtons.Count);
                CreateConnectingLines(current, horizontalLength);
                SetXPositions(childButtons, subSectionWidth, xpos);
            }
        }
    }

    public void CreateConnectingLines(SkillButtonObject parentButton, float width, float rowDist = 80f)
    {
        parentButton.ActivateConnectingLineUp(true);
        List<SkillButtonObject> childButtons = GetChildButtons(parentButton);
        foreach (SkillButtonObject childButton in childButtons)
        {
            childButton.ActivateConnectingLineDown(true);
        }
        GameObject horizontalConnectingLine = Instantiate(horizontalConnectionPrefab, parentButton.transform);
        RectTransform hcrt = horizontalConnectingLine.GetComponent<RectTransform>();
        hcrt.sizeDelta = new Vector2(width, hcrt.sizeDelta.y);
        hcrt.anchoredPosition = new Vector2(hcrt.anchoredPosition.x, rowDist / 2);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GenerateTreeFromData();
        }
    }
}
