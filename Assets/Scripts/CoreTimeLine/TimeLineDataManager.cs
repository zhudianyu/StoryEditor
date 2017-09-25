//=========================================================================================
//
//    Author: zhudianyu
//    Email:  1462415060@qq.com
//    CreateTime:  2016-03-17 16:10:10Z
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using JsonFx.Json;
namespace TimeLineCore
{
    public class TimeLineDataManager
    {

        static TimeLineDataManager instance;
        public static TimeLineDataManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new TimeLineDataManager();
                return instance;
            }
        }
        List<TimeLineData> dataList = new List<TimeLineData>();
        //public List<TimeLineData> LineDataList
        //{
        //    get
        //    {
        //        if ( curData == null )
        //            LoadDefaultData();
        //        if ( curData == null )
        //            return dataList;
        //        dataList = curData.lineDataList;
        //        return curData.lineDataList;
        //    }
        //    set
        //    {
        //        dataList = value;
        //        curData.lineDataList = value;
        //    }
        //}
        Dictionary<string, TimeLineData> linedataDic = new Dictionary<string, TimeLineData>();
        public int SelectRow = -1;//当前选中行
        public int SelectColumn = -1;//当前选择列
        public float TickDelta = 0.5f;//刻度

        public string filePath = Application.dataPath + "/Resources/EditData/";
        public static readonly string DEFAULT_SAVE_PATH = Application.dataPath + "/Resources/EditData/";
        public static readonly string DATA_EXTENTION = "bytes";
        public static readonly string JSON_EXTENTION = "json";
        public int ID = 0;
        public string fileName = "NewFile.json";
        public bool IsJson = true;
        private EditData curData = null;
        public TimeLine Line
        {
            get;
            set;
        }
        /// <summary>
        /// 保存每一个Actionitme里面gui要画的函数列表
        /// </summary>
        public Dictionary<string, MemberInfo> memDic = new Dictionary<string, MemberInfo>();
        /// <summary>
        /// 当前正在改变的action
        /// </summary>
        public ActionItemData curOperationAction;
        public void Release()
        {
            if (instance != null)
            {
                if (memDic != null)
                {
                    memDic.Clear();
                    memDic = null;
                }
                if (linedataDic != null)
                {
                    linedataDic.Clear();
                    linedataDic = null;
                }
                if (dataList != null)
                {
                    dataList.Clear();
                    dataList = null;
                }
                if (curData != null)
                    curData = null;
                instance = null;
            }



        }
        public int GetGUIMemInfoCount()
        {
            if (memDic != null)
                return memDic.Count;
            else
                return 0;
        }
        public void AddGUIMemInfo(MemberInfo info)
        {
            if (memDic.ContainsKey(info.Name))
            {
                memDic[info.Name] = info;
            }
            else
            {
                memDic.Add(info.Name, info);
            }
        }
        public void DeleteMemInfo(MemberInfo info)
        {
            if (memDic.ContainsKey(info.Name))
                memDic.Remove(info.Name);
        }
        public EditData LoadDefaultData(bool isJson = true)
        {
            //string[] fileArray = Directory.GetFiles( filePath );
            //if ( fileArray.Length > 0 )
            //    return LoadEditData( fileArray[0] );
            //else
            //{
            if (isJson)
                return LoadEditData(DEFAULT_SAVE_PATH + fileName);
            return LoadEditData(DEFAULT_SAVE_PATH + fileName);
            // }
        }
        public void AddTimeLine(TimeLineData data)
        {
            if (curData != null)
                curData.AddLineData(data);
            else
            {
                curData = LoadDefaultData();
                curData.AddLineData(data);
            }
        }
        public void DeleteTimeLine(int index)
        {
            if (curData != null)
                curData.DeleteTimeLine(index);
            else
                Debug.Log(" cur data is null");
        }
        public int GetTimeLineCount()
        {
            if (curData != null)
                return curData.GetTimeLineCount();

            return 0;
        }
        public TimeLineData GetTimeLineByIndex(int index)
        {
            if (curData != null)
                return curData.GetTimeLineByIndex(index);
            else
                //Debug.Log( " cur data is null" );
                return null;
        }
        public TimeLineData GetCurSelectTimeLine()
        {
            if (curData != null)
                return curData.GetCurSelectTimeLine(SelectRow);
            else
                // Debug.Log( " cur data is null" );
                return null;
        }
        public bool IsCanAdd(string name)
        {
            int i = 0;
            if(curData != null)
            {
                return !curData.HasAddLine(name,out i);
            }
            return true;
        }
        public void SaveData(Type t, List<string> nameList, List<object> list, bool isJson = true)
        {
            ActionItemData data = new ActionItemData(t, nameList);
            data.paramList = list.ToArray();
            TimeLineData lineData = GetCurSelectTimeLine();
            lineData.AddAction(data);
            if (isJson)
            {
                string newfilepath = filePath;
                if (newfilepath.EndsWith(".json"))
                {
                    SaveToJson(newfilepath, lineData);
                }
                else
                {
                    if (EditorUtility.DisplayDialog("error", "此文件不存在，请先创建", "ok"))
                    {
                        TimeLine.ShowFileUI();
                    }

                }
            }
            else
            {
                string newfilepath = filePath ;
                if (newfilepath.EndsWith(".bytes"))
                {
                    SaveEditData(newfilepath, lineData);
                }
                else
                {
                    if (EditorUtility.DisplayDialog("error", "此文件不存在，请先创建", "ok"))
                    {
                        TimeLine.ShowFileUI();
                    }

                }
            }

        }
        public EditData SaveToJson(string path, TimeLineData data = null)
        {
            if (!File.Exists(path))
            {
                curData = CreateNewEditData();
                curData.dataPath = path;
             
                string jsonStr = JsonWriter.Serialize(curData);
                Debug.Log(jsonStr);

                File.WriteAllText(path, jsonStr);

            }
            else
            {

                string jsonstr = File.ReadAllText(path);
                if(curData == null)
                {
                    curData = JsonReader.Deserialize<EditData>(jsonstr) as EditData;
                    if(curData == null)
                    {
                        EditorUtility.DisplayDialog("Error", "读取文件失败", "OK");
                        return null;
                    }
                    for (int i = 0; i < curData.GetTimeLineCount(); i++)
                    {//json 反序列化会全部解析成基类 所以做clone操作 比较恶心
                        TimeLineData tempdata = curData.GetTimeLineByIndex(i);
                        Type t = Type.GetType(tempdata.className);
                        var subdata = Activator.CreateInstance(t) as TimeLineData;
                        subdata.Clone(tempdata);
                        curData.DeleteTimeLine(i);
                        curData.lineDataList.Insert(i, subdata);
                    }
                }
                else
                {
                    if (data != null)
                    {
                        curData.dataPath = path;
                        curData.AddLineData(data);
                        string jsonStr = JsonWriter.Serialize(curData);
                        Debug.Log(jsonStr);

                        File.WriteAllText(path, jsonStr);
                    }
                }
            }
            return curData;
        }
        public EditData LoadEditData(string path, bool isJson = true)
        {
            TimeLineDataManager.instance.filePath = path;
            curData = null;
            EditData story = LoadLineData(path);
            if (story != null)
            {
                curData = story;
                dataList = curData.lineDataList;
                return story;
            }

            return null;
        }
        EditData LoadLineData(string path, bool isJson = true)
        {
            if (isJson)
            {
                return SaveToJson(path);
            }
            else
            {
                #region binary
                if (File.Exists(path))
                {
                    using (FileStream fs = File.Open(path, FileMode.Open))
                    {
                        BinaryReader br = new BinaryReader(fs);
                        //文件头
                        string head = br.ReadString();
                        if (head != "EditData")
                        {
                            br.Close();
                            fs.Close();
                            return null;
                        }
                        EditData td = CreateNewEditData();
                        td.dataPath = path;
                        td.ReadFromBinary(br);

                        //结束
                        br.Close();
                        fs.Close();
                        return td;
                    }

                }
                else
                {
                    FileStream fs = null;
                    BinaryWriter bw = null;
                    try
                    {

                        fs = File.Open(path, FileMode.Create);
                        bw = new BinaryWriter(fs);
                        //文件头
                        string head = "EditData";
                        bw.Write(head);
                        EditData ed = CreateNewEditData();
                        ed.dataPath = path;
                        ed.WriteToBinary(bw);
                        bw.Close();
                        fs.Close();

                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                        if (bw != null)
                            bw.Close();
                        if (fs != null)
                            fs.Close();
                    }
                }
                #endregion
            }

            return null;
        }
        private EditData CreateNewEditData()
        {
            EditData td = new EditData();
            td.dataName = "newdata";

            return td;
        }
        public void SaveEditData(string filePath, TimeLineData data)
        {

            if (string.IsNullOrEmpty(filePath))
                return;
            if (data == null)
            {
                Debug.LogError("save linedata is null");
                return;
            }
            FileStream fs = null;
            BinaryWriter bw = null;
            try
            {

                fs = File.Open(filePath, FileMode.Create);
                bw = new BinaryWriter(fs);
                //文件头
                string head = "EditData";
                bw.Write(head);

                if (curData == null)
                {
                    curData = CreateNewEditData();
                    curData.dataPath = filePath;

                }

                curData.AddLineData(data);
                curData.WriteToBinary(bw);

                bw.Close();
                fs.Close();

            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                if (bw != null)
                    bw.Close();
                if (fs != null)
                    fs.Close();
            }
        }
    }
}