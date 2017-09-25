//=========================================================================================
//
//    Author: zhudianyu
//    Email:  1462415060@qq.com
//    CreateTime:  2016-03-25 15:40:44Z
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
public class RoleInstance
{
    static RoleInstance instance = null;

    List<GameObject> roleList = new List<GameObject>();
    public static RoleInstance Instance
    {
        get
        {
            if ( instance == null )
                instance = new RoleInstance();
            return instance;
        }
    }

    public void CreateRole(Vector3 genpos , Vector3 lookat,int roleID)
    {
       GameObject role = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
       role.name = roleID.ToString();
       roleList.Add(role);
        role.transform.localPosition = genpos;
        Debug.Log("CreateRole ==== " + genpos.ToString() + " " + lookat.ToString());
    }
    public void MoveRole(Vector3 destPos, int roleID)
    {
        GameObject role = null;
        foreach(var go in roleList)
        {
            if (go.name == roleID.ToString())
                role = go;
        }
        if (role != null)
            role.transform.DOMove(destPos, 2);
        Debug.Log( "MoveRole +++ " + destPos.ToString() );
    }
}
