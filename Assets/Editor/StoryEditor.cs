//=========================================================================================
//
//    Author: zhudianyu
//
//    CreateTime:  2016-03-15 11:48:00Z
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
using TimeLineCore;
public class StoryEditor : EditorWindow
{
    static StoryEditor window = null;

    static readonly int TimeRectBeginY = 0;
    static readonly int TimeRectBeginX = 0;

  
     TimeLine timeLine ;
	// Use this for initialization
	[MenuItem("Custom/剧情编辑器")]
	static public void Init()
    {
        window = EditorWindow.GetWindow(typeof(StoryEditor)) as StoryEditor;
        window.Show();
        //Application.targetFrameRate = 20;
    }
    void OnEnable()
    {
       
        timeLine = new TimeLine();
        timeLine.TimeLinesCount = 1;
        TimeLineDataManager.Instance.Release();
        TimeLineDataManager.Instance.Line = timeLine;
        TimeLineDataManager.Instance.LoadDefaultData();
    }
  
    void OnGUI()
    {
      
       
        GUILayout.BeginArea(new Rect(TimeRectBeginX, TimeRectBeginY, Screen.width, Screen.height));
     
        timeLine.OnGUI();
        GUILayout.EndArea();
      
        
    }
  
	// Update is called once per frame
	void Update () {
	
	}
}
