//=========================================================================================
//
//    Author: zhudianyu
//    Email:  1462415060@qq.com
//    CreateTime:  2016-03-21 19:56:28Z
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using TimeLineCore;
public class Test : MonoBehaviour {

    
    class Test1
    {
        public int a = 10;
        public Test2 b = new Test2();
    }
    class Test2
    {
        public int b = 5;
    }
	// Use this for initialization
	void Start () 
    {
        //SortedDictionary<string, int> dic = new SortedDictionary<string, int>();
        //dic.Add("3", 3);
        //dic.Add("11", 11);
        //foreach(var info in dic)
        //{
        //    Debug.Log(info.Key);
        //}
        //Debug.Log("==============================");
        //SortedDictionary<string, int> test = new SortedDictionary<string, int>();
        //test.Add("03", 3);
        //test.Add("11", 11);
        //foreach (var info in test)
        //{
        //    Debug.Log(info.Key);
        //}
        //string teststr = string.Format("{0:D5}", 4);
        //Debug.Log(teststr);
        string path = Application.dataPath + "/Resources/EditData/NewFile.json";
        TimeLinePlayManager.Instance.StartPlayTimeLine(path);
   
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
