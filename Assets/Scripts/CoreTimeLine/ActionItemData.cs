//=========================================================================================
//
//    Author: zhudianyu
//    Email:  1462415060@qq.com
//    CreateTime:  2016-03-16 16:44:42Z
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace TimeLineCore
{
    [Serializable]
    public class ActionItemData
    {

        public string name;
        public int index;
        public object[] paramList = null;
        public float actionStartTime = 0;
        public float actionEndTime = 0.5f;
        public List<string> methodNameList = new List<string>();
        public ActionItemData()
        {

        }
        public ActionItemData(Type t , List<string> methodList)
        {
            name = t.Name;
            methodNameList = methodList;

            actionStartTime = TimeLineDataManager.Instance.GetCurSelectTimeLine().StartTime;
            actionEndTime = TimeLineDataManager.Instance.GetCurSelectTimeLine().EndTime;
            index = Mathf.CeilToInt( actionStartTime / TimeLineDataManager.Instance.TickDelta );
        }
        public ActionItemData Clone()
        {
            ActionItemData ad = new ActionItemData();
            ad.name = name;
            ad.index = index;
            foreach ( var info in methodNameList )
            {
                ad.methodNameList.Add( info );
            }
            if ( paramList != null && paramList.Length > 0 )
            {
                ad.paramList = new object[paramList.Length];
                paramList.CopyTo( ad.paramList , 0 );
            }
            else
            {
                ad.paramList = null;
            }

            ad.actionEndTime = actionEndTime;
            ad.actionStartTime = actionStartTime;
            return ad;

        }
        public bool IsSelect()
        {
            int st = (int)( actionStartTime / TimeLineDataManager.Instance.TickDelta );
            int et = (int)( actionEndTime / TimeLineDataManager.Instance.TickDelta );
            if ( st <= TimeLineDataManager.Instance.SelectColumn && TimeLineDataManager.Instance.SelectColumn <= et )
            {
                return true;
            }
            return false;
        }
        public void WriteToBinary(BinaryWriter wr)
        {
            wr.Write( name );
            wr.Write( methodNameList.Count );
            foreach ( var mn in methodNameList )
            {
                wr.Write( mn );
            }
            // wr.Write(index);
            wr.Write( actionStartTime );
            wr.Write( actionEndTime );
            if ( paramList == null )
            {
                Debug.Log( "param list is null" );
                wr.Write( 0 );
                return;
            }
            
            wr.Write( paramList.Length );
            for ( int i = 0; i < paramList.Length; i++ )
            {
                object ob = paramList[i];
                wr.Write( ob.GetType().Name );
                //vector3 is NonSerialized 其他默认为基本类型
                if ( "Vector3".Equals( ob.GetType().Name ) )
                {
                    Vector3 vec3 = (Vector3)ob;
                    wr.Write( vec3.x );
                    wr.Write( vec3.y );
                    wr.Write( vec3.z );
                }
                else
                {
                    BinaryFormatter formater = new BinaryFormatter();
                    MemoryStream memStream;
                    try
                    {
                        memStream = new MemoryStream();
                        formater.Serialize( memStream , paramList[i] );
                    }
                    catch ( Exception ex )
                    {
                        Debug.LogError( "序列化失败,不能保存:" + ex.ToString() );
                        return;
                    }
                    byte[] toParam = memStream.GetBuffer();

                    wr.Write( toParam.Length );
                    wr.Write( toParam );
                }

            }

        }

        public void ReadFromeBinary(BinaryReader br)
        {
            name = br.ReadString();
            int len = br.ReadInt32();
            methodNameList.Clear();
            for ( int j = 0; j < len; j++ )
            {
                string mn = br.ReadString();
                methodNameList.Add( mn );
            }
            //  index = br.ReadInt32();
            actionStartTime = br.ReadSingle();
            actionEndTime = br.ReadSingle();
            index = Mathf.CeilToInt( actionStartTime / TimeLineDataManager.Instance.TickDelta );
            int length = br.ReadInt32();
            if ( length == 0 )
                return;
            paramList = new object[length];
            for ( int i = 0; i < length; i++ )
            {
                string typeName = br.ReadString();
                if ( "Vector3".Equals( typeName ) )
                {
                    paramList[i] = new Vector3( br.ReadSingle() , br.ReadSingle() , br.ReadSingle() );
                }
                else
                {
                    int paramLength = br.ReadInt32();

                    byte[] paramBytes = br.ReadBytes( paramLength );
                    if ( paramLength > 0 )
                    {
                        BinaryFormatter formater = new BinaryFormatter();
                        try
                        {
                            MemoryStream memStream = new MemoryStream( paramBytes );
                            paramList[i] = formater.Deserialize( memStream );
                        }
                        catch ( Exception ex )
                        {
                            Debug.LogError( "反序列化失败,:" + ex.ToString() );
                            return;
                        }
                    }
                    else
                    {
                        paramList[i] = null;
                    }
                }
            }
        }
    }
}