//=========================================================================================
//
//    Author: zhudianyu
//    Email:  1462415060@qq.com
//    CreateTime:  2016-03-16 17:02:47Z
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using System;
using DG.Tweening;
using TimeLineCore;
[TimeLine("摄像机")]
public class CameraTimeLineData : TimeLineData 
{
    /// <summary>
    /// 震动时间
    /// </summary>
    [SaveFieldAttribute]
    public float duration = 0f;
    /// <summary>
    /// 震动强度
    /// </summary>
    [SaveFieldAttribute]
    public float strength = 0;

    /// <summary>
    /// 移动到位置
    /// </summary>
    [SaveField]
    public Vector3 destPos;
    public CameraTimeLineData()
    {

    }
    public CameraTimeLineData(int index, string name, int gridCount)
        : base(index,name,gridCount)
    {

    }

    [GridMenuItem("震动Camera")]
    public void ShakeCamera( )
    {
        duration = EditorGUILayout.FloatField( "震动时间" , duration );
        strength = EditorGUILayout.FloatField( "震动强度" , strength );
   

    }
    public void ExecuteShakeCamera()
    {
        CameraInstance.Instance.ShakeCamera( strength , duration );
    }
    [GridMenuItem("Move")]
    public void MoveCamera( )
    {
        destPos = EditorGUILayout.Vector3Field( "移动到位置" , destPos );
    }
    public void ExecuteMoveCamera()
    {
        CameraInstance.Instance.MoveCamera( destPos,0.5f );
    }
}
