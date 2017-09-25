//=========================================================================================
//
//    Author: zhudianyu
//    Email:  1462415060@qq.com
//    CreateTime:  2016-03-16 16:41:30Z
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Reflection;
namespace TimeLineCore
{
    [Serializable]
    public class TimeLineData
    {
        public string TimeLineName;
        public int TimeLineIndex;

        public int GridCount;
        public SortedDictionary<string , ActionItemData> actionDic = new SortedDictionary<string , ActionItemData>();
       [NonSerialized]
        public List<object> actionParamList = new List<object>();
        private float startTime = 0;

        public string className;
        public float StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                if ( value > endTime )
                {
                    // if(EditorUtility.DisplayDialog("error","大于结束时间","OK"))
                    return;

                }
                if ( value < 0 )
                {
                    //if (EditorUtility.DisplayDialog("error", "不能小于0", "OK"))
                    return;
                }
                if ( IsCanChange() )
                    startTime = Mathf.CeilToInt( value / TimeLineDataManager.Instance.TickDelta ) * TimeLineDataManager.Instance.TickDelta;

            }
        }
        private float endTime = 10000000;
        public float EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                if ( value < startTime )
                {
                    // if (EditorUtility.DisplayDialog("error", "小于开始时间", "OK"))
                    return;

                }
                if ( value < 0 )
                {
                    // if (EditorUtility.DisplayDialog("error", "不能小于0", "OK"))
                    return;
                }
                if ( IsCanChange() )
                    endTime = Mathf.CeilToInt( value / TimeLineDataManager.Instance.TickDelta ) * TimeLineDataManager.Instance.TickDelta;
            }
        }
        public void AddAction(ActionItemData ac)
        {
            int key = Mathf.CeilToInt( ac.actionStartTime / TimeLineDataManager.Instance.TickDelta );
            ac.index = key;
            string keyStr = string.Format("{0:D5}", key);
            if (actionDic.ContainsKey(keyStr))
            {
                actionDic[keyStr] = ac;
            }
            else
            {
                actionDic.Add(keyStr, ac);
            }
        }
        public void DeleteAction(int key)
        {
            string keyStr = string.Format("{0:D5}", key);
            if (actionDic.ContainsKey(keyStr))
            {
                actionDic.Remove(keyStr);
            }
        }
        public void DeleteAction(ActionItemData ac)
        {
            int key = Mathf.CeilToInt( ac.actionStartTime / TimeLineDataManager.Instance.TickDelta );
            string keyStr = string.Format("{0:D5}", key);
            if (actionDic.ContainsKey(keyStr))
            {
                actionDic.Remove(keyStr);
            }
        }
        public void Clone(TimeLineData data)
        {
            StartTime = data.StartTime;
            EndTime = data.EndTime;
            TimeLineIndex = data.TimeLineIndex;
            TimeLineName = data.TimeLineName;
            GridCount = data.GridCount;
            foreach(var dic in data.actionDic)
            {
                actionDic.Add(dic.Key, dic.Value);
            }
        }
        public int GetActionCount()
        {
            if ( actionDic != null )
                return actionDic.Count;
            return 0;
        }
        public SortedDictionary<string , ActionItemData> GetActionDic()
        {
            if ( actionDic != null )
                return actionDic;
            return null;
        }
        public TimeLineData()
        {
            className = GetType().ToString();
        }
        public TimeLineData(int index , string name , int gridCount)
        {
            TimeLineIndex = index;
            TimeLineName = name;
            className = GetType().ToString();
        }
        //public Type GetType()
        //{
        //    Type t = Type.GetType(typename);
        //    return t;
        //}
        public void Init(int index , string name , int gridCount)
        {
            TimeLineIndex = index;
            TimeLineName = name;
            GridCount = gridCount;
            className = GetType().ToString();
            InitDefaultTime();
        }
        public void InitDefaultTime()
        {
            ActionItemData ad = GetCurActionItemData();
            if ( ad != null )
            {
                if ( ad.IsSelect() )
                {
                    endTime = ad.actionEndTime;
                    startTime = ad.actionStartTime;
                    return;
                }

            }
            endTime = ( TimeLineDataManager.Instance.SelectColumn ) * TimeLineDataManager.Instance.TickDelta;
            startTime = TimeLineDataManager.Instance.SelectColumn * TimeLineDataManager.Instance.TickDelta;
        }
        public ActionItemData GetActionByKey(int key)
        {
            ActionItemData ad = null;
            string keystr = key.ToString();
            if (actionDic.TryGetValue(keystr, out ad))
                return ad;
            return null;
        }
        public ActionItemData GetActionByKey(string key)
        {
            ActionItemData ad = null;
        
            if (actionDic.TryGetValue(key, out ad))
                return ad;
            return null;
        }
        public ActionItemData GetActionItemData(int index)
        {
            if ( actionDic != null )
            {
                foreach ( var itemdata in actionDic )
                {
                    ActionItemData ad = itemdata.Value;
                    int st = (int)( ad.actionStartTime / TimeLineDataManager.Instance.TickDelta );
                    int et = (int)( ad.actionEndTime / TimeLineDataManager.Instance.TickDelta );
                    if ( st <= index && index < et )
                    {
                        return ad;
                    }
                }
            }
            return null;
        }
        public ActionItemData GetCurActionItemData()
        {
            if ( actionDic != null )
            {
                foreach ( var itemdata in actionDic )
                {
                    ActionItemData ad = itemdata.Value;
                    if ( ad.IsSelect() )
                    {
                        return ad;
                    }

                }
            }
            return null;
        }
        bool IsCanChange()
        {

            var data = TimeLineDataManager.Instance.GetCurSelectTimeLine();
            if ( data == null )
                return true;
            ActionItemData ad = TimeLineDataManager.Instance.curOperationAction;
            if ( ad == null )
                return true;
            List<int> keyList = new List<int>();
            foreach ( var pari in data.actionDic )
            {
                if(ad.index != pari.Value.index)
                {
                    keyList.Add(pari.Value.index);
                }
               
            }
            int lastKey = -1 , afterKey = -1;
            for ( int i = 0; i < keyList.Count; i++ )
            {
                if ( keyList[i] > ad.index )
                {
                    int curKey = Mathf.CeilToInt( ad.actionEndTime / TimeLineDataManager.Instance.TickDelta);
                    if(keyList[i] != curKey)
                    {//过滤当前操作的action的key
                        afterKey = keyList[i];
                    }
                    
                }
                if(keyList[i] < ad.index)
                {
                    lastKey = keyList[i];
                }
            }

            ActionItemData lastAd = GetActionByKey( lastKey );
            if ( lastAd != null )
            {
                // Debug.Log( "starttime is " + startTime.ToString() + " lastad endtime is " + lastAd.actionEndTime.ToString() );
                if ( startTime <= lastAd.actionEndTime )
                {

                    startTime = lastAd.actionEndTime + TimeLineDataManager.Instance.TickDelta;
                  //  Debug.LogWarning("lastKey is " + lastKey.ToString());
                    Debug.LogError( "前方已有事件" );
                    return false;
                }

            }
            ActionItemData afterad = GetActionByKey( afterKey );
            if ( afterad != null )
            {
                // Debug.Log( "endtime is " + startTime.ToString() + " afterad actionStartTime is " + afterad.actionStartTime.ToString() );
                if ( endTime >= afterad.actionStartTime )
                {
                   
                    endTime = afterad.actionStartTime - TimeLineDataManager.Instance.TickDelta;
                    Debug.LogError( "后方已有事件" );
                    return false;
                }
            }
          
            return true;
        }

        public void OnGUI()
        {
            TimeLineData data = TimeLineDataManager.Instance.GetCurSelectTimeLine();
            if ( data == null )
                return;
            if ( TimeLineDataManager.Instance.GetGUIMemInfoCount() == 0 )
                return;

            StartTime = EditorGUILayout.FloatField( "开始时间" , startTime );
            EndTime = EditorGUILayout.FloatField( "结束时间" , endTime );
            ActionItemData tempad = TimeLineDataManager.Instance.curOperationAction;
            if ( tempad != null )
            {
                tempad.actionStartTime = StartTime;
                tempad.actionEndTime = EndTime;
            }
            if ( GUILayout.Button( "Save" ) )
            {
                SaveLineData();
            }

        }
        public ActionItemData SetActionValue(ActionItemData ad)
        {
            TimeLineData data = TimeLineDataManager.Instance.GetCurSelectTimeLine();
            if ( data == null )
                return null;
            SetAtctionParamList( data );
            ad.paramList = actionParamList.ToArray();
            return ad;
        }
        void SetAtctionParamList(TimeLineData data)
        {

            Type t = data.GetType();
            FieldInfo[] arr = t.GetFields();
            actionParamList.Clear();
            foreach ( var info in arr )
            {
                if ( info.IsDefined( typeof( SaveFieldAttribute ) , false ) )
                {
                    if ( info.GetValue( data ) != null )
                    {
                        actionParamList.Add( info.Name );
                        actionParamList.Add( info.GetValue( data ) );
                    }

                }

            }
        }
        public void SaveLineData()
        {
            TimeLineData data = TimeLineDataManager.Instance.GetCurSelectTimeLine();
            if ( data == null )
                return;
            SetAtctionParamList( data );
            var memDic = TimeLineDataManager.Instance.memDic;
            List<string> nameList = new List<string>();
            foreach ( var name in memDic.Keys )
            {
                nameList.Add( name );
            }
            TimeLineDataManager.Instance.SaveData(data.GetType(), nameList, actionParamList);
        }
        public void OnDrawDeleteButton(MemberInfo info)
        {
            if ( GUILayout.Button( "X" ) )
            {
                TimeLineDataManager.Instance.DeleteMemInfo( info );
                if ( TimeLineDataManager.Instance.memDic.Count == 0 )
                {
                    ActionItemData ad = TimeLineDataManager.Instance.curOperationAction;
                    TimeLineData data = TimeLineDataManager.Instance.GetCurSelectTimeLine();
                    if ( data != null )
                    {
                        DeleteAction(ad.index);
                   
                        TimeLineDataManager.Instance.curOperationAction = null;
                    }
                }
            }
        }
        public void WriteToBinary(BinaryWriter wr)
        {

            wr.Write( TimeLineName );
            wr.Write( TimeLineIndex );
            wr.Write( GridCount );
            wr.Write( StartTime );
            wr.Write( EndTime );

            if ( actionDic != null )
            {
                wr.Write( actionDic.Count );
                foreach ( var item in actionDic )
                {
                    //wr.Write( item.Key );
                    item.Value.WriteToBinary( wr );
                    //  item.WriteToBinary(wr);
                }
            }
            else
            {
                Debug.LogError( "actionlist is null" );
            }
        }
        public void ReadFromBinary(BinaryReader br)
        {

            TimeLineName = br.ReadString();
            TimeLineIndex = br.ReadInt32();
            GridCount = br.ReadInt32();
            StartTime = br.ReadSingle();
            EndTime = br.ReadSingle();

            int len = br.ReadInt32();
            actionDic.Clear();
            for ( int i = 0; i < len; i++ )
            {
               // int key = br.ReadInt32();
                ActionItemData data = new ActionItemData();
                data.ReadFromeBinary( br );
                
                AddAction(data);

            }
        }
        public void SetValueByAction(ActionItemData ad)
        {
            if ( ad == null )
                return;
            var paramList = ad.paramList;
            if ( paramList == null )
                return;
            endTime = ad.actionEndTime;

            startTime = ad.actionStartTime;
            SetObject(paramList);
        
        }
        void SetObject(object[] paramList)
        {
            var target = this;
            foreach (var info in target.GetType().GetFields())
            {
                if (info.IsDefined(typeof(SaveFieldAttribute), false))
                {
                    for (int j = 0; j < paramList.Length - 1; j++)
                    {
                        if (paramList[j].GetType() == typeof(string))
                        {
                            if (info.Name == (string)paramList[j])
                            {
                                Type ft = info.FieldType;
                                Type t = paramList[j + 1].GetType();
                                if (ft.Name == "Vector3")
                                {//json 数据在此处理
                                    if (TimeLineDataManager.Instance.IsJson)
                                    {
                                        if (t.Name != "Vector3")
                                        {
                                            Dictionary<string, object> vecDic = Convert.ChangeType(paramList[j + 1], t) as Dictionary<string, object>;

                                            float x = 0, y = 0, z = 0;
                                            if (vecDic == null)
                                                return;
                                            foreach (var pair in vecDic)
                                            {
                                                if (pair.Key == "x")
                                                    x = (float)Convert.ChangeType(pair.Value, typeof(float));
                                                if (pair.Key == "y")
                                                    y = (float)Convert.ChangeType(pair.Value, typeof(float));
                                                if (pair.Key == "z")
                                                    z = (float)Convert.ChangeType(pair.Value, typeof(float));
                                            }
                                            Vector3 newVec = new Vector3(x, y, z);
                                            info.SetValue(target, Convert.ChangeType(newVec, ft));
                                        }
                                        else
                                        {
                                            info.SetValue(target, Convert.ChangeType(paramList[j + 1], ft));
                                        }
                                    }
                                    else
                                    {
                                        info.SetValue(target, Convert.ChangeType(paramList[j + 1], ft));
                                    }
                                }
                                else
                                {
                                    info.SetValue(target, Convert.ChangeType(paramList[j + 1], ft));
                                }
                            }
                        }
                    }
                }
            }
        }
        public void SetFieldValue(TimeLineData target)
        {
            InitDefaultTime();
            ActionItemData ad = GetCurActionItemData();
            if (ad == null)
                return;
            var paramList = ad.paramList;
            if (paramList == null)
                return;
            endTime = ad.actionEndTime;

            startTime = ad.actionStartTime;
            SetObject(paramList);

        }

       



    }
}