using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


public class SkillTreeGraphTool : EditorWindow
{
    private SkillTree skillTree;

    private List<SkillTreeNode> skillNodes;

    private Dictionary<Skill, SkillTreeNode> skillNodeLookup;

    /*[SerializeField]
    public static STNodeLocationDict savedLocations;*/

    private List<Connection> connections;

    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    private Vector2 offset;
    private Vector2 drag;

    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;

    [MenuItem("Window/Skill Tree Editor")]
    private static SkillTreeGraphTool OpenWindow()
    {
        SkillTreeGraphTool window = GetWindow<SkillTreeGraphTool>();
        window.titleContent = new GUIContent("Skill Tree Editor");        
        return window;
    }

    [UnityEditor.Callbacks.OnOpenAsset(1)]
    public static bool OnOpenSkillTree(int instanceID, int line)
    {
        Debug.Log("OpenSkillTree");
        SkillTree mySkillTree = EditorUtility.InstanceIDToObject(instanceID) as SkillTree;
        if(mySkillTree!=null)
        {
            SkillTreeGraphTool editorWindow = SkillTreeGraphTool.OpenWindow();
            editorWindow.skillTree = mySkillTree;
            editorWindow.LoadData();
            return true;
        }
        return false;
    }

    void OnDestroy()
    {
        SaveData();
    }

    private void LoadData()
    {
        if(skillNodeLookup == null)
        {
            skillNodeLookup = new Dictionary<Skill, SkillTreeNode>();
        }
        if (skillTree.skillList == null)
        {
            return;
        }
        if(skillTree.nodeLocData == null)
        {
            SaveData(); //this will initialize an empty entry to avoid null reference exceptions
        }
        foreach (Skill mySkill in skillTree.skillList)
        {
            Vector2 pos = skillTree.nodeLocData.Position(mySkill);
            AddNode(pos, mySkill);
            /*if (!savedLocations.Tree(skillTree).dataList.dataList.ContainsKey(mySkill)) //puts the skills at position 0,0 if the tree is modified outside of the graph tool
            {
                SkillTreeNode node = AddNode(Vector2.zero, mySkill);
            }
            else
            {
                AddNode(savedLocations[skillTree].dataList[mySkill], mySkill);
            }*/
        }        
        if(skillTree.connectionList == null)
        {
            return;
        }
        foreach (SkillConnection sc in skillTree.connectionList)
        {
            SkillTreeNode inNode = skillNodeLookup[sc.parentSkill];
            SkillConnection myConnection = sc;
            foreach(Skill outSkill in myConnection.prerequisiteSkills)
            {
                SkillTreeNode outNode = skillNodeLookup[outSkill];
                if(outNode != null)
                {
                    selectedInPoint = inNode.inPoint;
                    selectedOutPoint = outNode.outPoint;
                    CreateConnection();
                }
            }
        }
    }

    private void SaveData()
    {
        STNodeLocationData data = new STNodeLocationData(skillTree);
        if (skillNodes == null)
        {
            skillNodes = new List<SkillTreeNode>();
        }
        foreach (SkillTreeNode node in skillNodes)
        {
            data.AddEntry(node.skill, node.rect.position);
        }
        /*if(skillTree.nodeLocData ==null)
        {
            skillTree.nodeLocData = new STNodeLocationData(skillTree);
        }*/
        skillTree.nodeLocData = data;
    }

    private void OnEnable()
    {
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
        nodeStyle.alignment = TextAnchor.MiddleCenter;
        nodeStyle.normal.textColor = Color.white;

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
        selectedNodeStyle.alignment = TextAnchor.MiddleCenter;
        selectedNodeStyle.normal.textColor = Color.white;

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("button_red.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("button_blue.png") as Texture2D;
        inPointStyle.border = new RectOffset(32, 32, 32, 32);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("button_red.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("button_blue.png") as Texture2D;
        outPointStyle.border = new RectOffset(32, 32, 32, 32);

    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    //for emergency fixes if something goes wrong.
    private void UpdateSkillNodesToMatchTree()
    {
        if (skillTree != null)
        {
            bool mismatchFound = false;
            if(skillTree.skillList.Count == skillNodes.Count)
            {
                for(int i = 0; i<skillNodes.Count; ++i)
                {
                    if(skillNodes[i].skill !=skillTree.skillList[i])
                    {
                        mismatchFound = true;
                    }
                }
            }
            if (mismatchFound)
            {
                skillNodes = new List<SkillTreeNode>();
                skillNodeLookup = new Dictionary<Skill, SkillTreeNode>();
                foreach (Skill skill in skillTree.skillList)
                {
                   // Vector2 position
                    //SkillTreeNode node = new SkillTreeNode(, 200, 50, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
                    //skillNodes.Add(skill);
                }
            }
        }
    }
    private void DrawNodes()
    {
        //old but still probably useful//////////////////
        if (skillNodes != null)                        //
        {                                              //
            for (int i = 0; i < skillNodes.Count; i++) //
            {                                          //
                skillNodes[i].Draw();                  //
            }                                          //
        }                                              //
        /////////////////////////////////////////////////
    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.up * 50f,
                e.mousePosition - Vector2.up * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.up * 50f,
                e.mousePosition + Vector2.up * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;
            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
                else
                {
                    Debug.Log($"Ebutton = {e.button}");
                }
                break;
        }
    }
    private void ProcessNodeEvents(Event e)
    {
        if (skillNodes != null)
        {
            for (int i = skillNodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = skillNodes[i].ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    const string defaultName = "New Skill";
    private void OnClickAddNode(Vector2 mousePosition)
    {
        SkillTreeNode myNode = AddNode(mousePosition);
        skillTree.AddSkill(myNode.skill);        
    }

    private SkillTreeNode AddNode(Vector2 pos, Skill skill = null)
    {
        if (skillNodes == null)
        {
            skillNodes = new List<SkillTreeNode>();
        }
        SkillTreeNode myNode = new SkillTreeNode(pos, 200, 50, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, skill);
        skillNodes.Add(myNode);
        skillNodeLookup.Add(myNode.skill, myNode);
        return myNode;
    }


    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;

        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickRemoveConnection(Connection connection)
    {
        connections.Remove(connection);
        Skill parentSkill = connection.inPoint.node.skill;
        Skill prereqSkill = connection.outPoint.node.skill;
        skillTree.RemoveConnection(parentSkill, prereqSkill);
    }

    private void OnClickRemoveNode(SkillTreeNode node)
    {
        if (connections != null)
        {
            List<Connection> connectionsToRemove = new List<Connection>();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }
        skillNodes.Remove(node);
        skillNodeLookup.Remove(node.skill);
        skillTree.RemoveSkill(node.skill);
    }

    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }
        connections.Add(new Connection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
        skillTree.AddConnection(selectedInPoint.node.skill, selectedOutPoint.node.skill);
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    private void OnDrag(Vector2 delta)
    {
        Debug.Log("ON DRAG");
        drag = delta;

        if (skillNodes != null)
        {
            for (int i = 0; i < skillNodes.Count; i++)
            {
                skillNodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }
}
