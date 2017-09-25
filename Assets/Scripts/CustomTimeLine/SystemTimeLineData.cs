//=========================================================================================
//
//    Author: zhudianyu
//    Email:  1462415060@qq.com
//    CreateTime:  2016-03-25 09:40:23Z
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEditor;
using UnityEditorInternal;
namespace TimeLineCore
{
    [TimeLine("系统")]
    public class SystemTimeLineData : TimeLineData
    {

        [SaveField]
        public string ShowTips;
        
        [GridMenuItem("显示文字")]
        public void ShowText()
        {
            ShowTips = EditorGUILayout.TextField("要显示的文字", ShowTips);
        }
        public void ExecuteShowText()
        {
            SystemInstance.Instance.ShowTips(ShowTips, EndTime - StartTime);
        }
    }
}

