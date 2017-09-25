//=========================================================================================
//
//    Author: zhudianyu
//    Email:  1462415060@qq.com
//    CreateTime:  2016-03-16 18:40:27Z
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using System;
namespace TimeLineCore
{
    [AttributeUsage( AttributeTargets.All , AllowMultiple = false , Inherited = false )]
    public class GridMenuItemAttribute : System.Attribute
    {

        public string MenuDescription
        {
            get
            {
                return mDesStr;
            }
        }
        private string mDesStr;
        public GridMenuItemAttribute(string desStr)
        {
            mDesStr = desStr;
        }
    }

    [AttributeUsage( AttributeTargets.Class , AllowMultiple = false , Inherited = false )]
    public class TimeLineAttribute : System.Attribute
    {

        public string MenuDescription
        {
            get
            {
                return mDesStr;
            }
        }
        private string mDesStr;
        public TimeLineAttribute(string desStr)
        {
            mDesStr = desStr;
        }
    }
    [AttributeUsage( AttributeTargets.All , AllowMultiple = false , Inherited = false )]
    public class CustomGUIAttribute : System.Attribute
    {

        private Type type;

        public Type Type
        {
            get
            {
                return this.type;
            }
        }

        public CustomGUIAttribute(Type type)
        {
            this.type = type;
        }

    }
    [AttributeUsage( AttributeTargets.Field , AllowMultiple = false , Inherited = false )]

    public class SaveFieldAttribute : System.Attribute
    {
        public SaveFieldAttribute()
        {

        }

    }
}