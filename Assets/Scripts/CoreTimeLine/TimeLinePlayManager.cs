//=========================================================================================
//
//    Author: zhudianyu
//    Email:  1462415060@qq.com
//    CreateTime:  2016-03-25 16:42:41Z
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;
namespace TimeLineCore
{
    public class TimeLinePlayManager : Singleton<TimeLinePlayManager>
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void StartPlayTimeLine(string path)
        {
            TimeLineDataManager.Instance.Release();
            StartCoroutine(Play(path));
        }
        IEnumerator Play(string path)
        {
          
            TimeLineDataManager.Instance.LoadEditData(path);
            int count = TimeLineDataManager.Instance.GetTimeLineCount();
            for (int i = 0; i < count; i++)
            {
                TimeLineData data = TimeLineDataManager.Instance.GetTimeLineByIndex(i);

                StartCoroutine(PlayLine(data));
            }
            yield return null;
        }
        IEnumerator PlayLine(TimeLineData data)
        {
            Type t = data.GetType();
            List<string> keyList = new List<string>();
            foreach (var actionPair in data.GetActionDic())
            {
                Debug.Log("key is " + actionPair.Key);
                keyList.Add(actionPair.Key);
            }
            for (int i = 0; i < keyList.Count;i++ )
            {
                string key = keyList[i];
                if(i == 0)
                {
                    ActionItemData ad = data.GetActionByKey(key);
                    float st = ad.actionStartTime;
                    yield return new WaitForSeconds(st);
                    data.SetValueByAction(ad);
                    foreach (var name in ad.methodNameList)
                    {
                        string methodName = "Execute" + name;
                        t.InvokeMember(methodName, BindingFlags.InvokeMethod, null, data, null);
                    }
                }
                else
                {
                    string lastkey = keyList[i - 1];
                    ActionItemData lastad = data.GetActionByKey(lastkey);
                    float lastst = lastad.actionStartTime;
                    ActionItemData ad = data.GetActionByKey(key);
                    float st = ad.actionStartTime;
                    float time = st - lastst;
                    Debug.Log(" time is " + time.ToString());
                    yield return new WaitForSeconds(time);
                    data.SetValueByAction(ad);
                    foreach (var name in ad.methodNameList)
                    {
                        string methodName = "Execute" + name;
                        t.InvokeMember(methodName, BindingFlags.InvokeMethod, null, data, null);
                    }
                }
            }
        }
    }
}