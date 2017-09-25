//=========================================================================================
//
//    Author: zhudianyu
//    Email:  1462415060@qq.com
//    CreateTime:  2016-03-21 10:05:37Z
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System;
namespace TimeLineCore
{
    [Serializable]
    public class EditData
    {
        public string dataName = null;
        public string dataPath = null;
        public List<TimeLineData> lineDataList = new List<TimeLineData>();
        public EditData()
        {

        }
        public void AddLineData(TimeLineData data)
        {
            int index = -1;
            if ( data != null )
            {
             
                if ( lineDataList.Contains( data ) )
                {
                   index = lineDataList.IndexOf(data);
                   lineDataList[index] = data;
                }
                else
                {

                    if(HasAddLine(data.TimeLineName,out index))
                    {
                        lineDataList[index] = data;
                    }
                    else
                    {
                        lineDataList.Add(data);
                    }
                   
                }
                
            }
        }
        public bool HasAddLine(string lineName, out int index)
        {
            for (int i = 0; i < lineDataList.Count;i++ )
            {
                var line = lineDataList[i];
                if (line.TimeLineName == lineName)
                {
                    index = i;
                    return true;
                }
                    
            }
            index = -1;
            return false;
        }
        public void DeleteTimeLine(int index)
        {
            lineDataList.RemoveAt( index );
        }
        public int GetTimeLineCount()
        {
            if ( lineDataList == null )
                return 0;
            return lineDataList.Count;
        }
        public TimeLineData GetTimeLineByIndex(int index)
        {
            if ( index < lineDataList.Count )
                return lineDataList[index];
            //  Debug.Log("error index = " + index + " length is " + lineDataList.Count);
            return null;
        }
        public TimeLineData GetCurSelectTimeLine(int SelectRow)
        {
            if ( lineDataList != null )
            {
                if ( SelectRow < lineDataList.Count && SelectRow >= 0 )
                    return lineDataList[SelectRow];
            }
            return null;
        }
        public void WriteToBinary(BinaryWriter wr)
        {
            wr.Write( dataName );
            wr.Write( dataPath );
            var datalist = lineDataList;
            if ( datalist != null )
            {
                wr.Write( datalist.Count );
                foreach ( var item in datalist )
                {
                    wr.Write( item.GetType().Name );
                    item.WriteToBinary( wr );
                }
            }
            else
            {
                wr.Write( 0 );
                Debug.LogError( "lineDataList is null" );
            }
        }

        public void ReadFromBinary(BinaryReader br)
        {

            dataName = br.ReadString();
            dataPath = br.ReadString();

            int len = br.ReadInt32();
            lineDataList.Clear();
            for ( int i = 0; i < len; i++ )
            {
                string type = br.ReadString();
                Type t = Type.GetType( type );

                TimeLineData data = Activator.CreateInstance( t ) as TimeLineData;
                data.ReadFromBinary( br );
                data.InitDefaultTime();
                lineDataList.Add( data );

            }
        }

    }
}