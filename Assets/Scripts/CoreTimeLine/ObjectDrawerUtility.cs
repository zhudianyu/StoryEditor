//=========================================================================================
//
//    Author: zhudianyu
//    Email:  1462415060@qq.com
//    CreateTime:  2016-03-18 10:58:12Z
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
namespace TimeLineCore
{
    internal static class ObjectDrawerUtility
    {
        private static Dictionary<string , TimeLineData> objDic = new Dictionary<string , TimeLineData>();


        public static TimeLineData GetTimeLineData(Type t , int index , string name , int gridCount)
        {
            string keyStr = t.Name + name;
            if ( objDic.ContainsKey( keyStr ) )
                return objDic[keyStr];
            else
            {
                var td = Activator.CreateInstance( t ) as TimeLineData;
                td.Init( index , name , gridCount );

                objDic.Add( keyStr , td );
                return td;
            }
        }
        public static bool IsCanAdd(Type t , string name)
        {
            string keyStr = t.Name + name;
            if ( objDic.ContainsKey( keyStr ) )
            {

                return false;
            }

            return true;
        }
    }
}