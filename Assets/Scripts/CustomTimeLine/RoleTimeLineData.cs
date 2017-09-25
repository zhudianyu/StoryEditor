//=========================================================================================
//
//    Author: zhudianyu
//    Email:  1462415060@qq.com
//    CreateTime:  2016-03-25 09:40:23Z
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using TimeLineCore;
[TimeLine("演员")]
public class RoleTimeLineData : TimeLineData 
{

    [SaveField]
    public Vector3 genpos = Vector3.zero;
    [SaveField]
    public Vector3 lookAt = Vector3.zero;

    [SaveField]
     public Vector3 destPos = Vector3.zero;

    [SaveField]
    public int RoleID = 0;
    [GridMenuItem("创建主角")]
    public void CreateRole()
    {
        genpos = EditorGUILayout.Vector3Field( "出生位置" , genpos );
        lookAt = EditorGUILayout.Vector3Field( "出生朝向" , lookAt );
        RoleID = EditorGUILayout.IntField("RoleID", RoleID);
    }
    public void ExecuteCreateRole()
    {
        RoleInstance.Instance.CreateRole( genpos , lookAt ,RoleID);
    }
    [GridMenuItem("主角移动")]
    public void MoveRole()
    {
        RoleID = EditorGUILayout.IntField("RoleID", RoleID);
        destPos = EditorGUILayout.Vector3Field( "移动位置" , destPos );
    }

    public void ExecuteMoveRole()
    {
        RoleInstance.Instance.MoveRole( destPos,RoleID );
    }
}
