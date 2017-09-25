//=========================================================================================
//
//    Author: zhudianyu
//    Email:  1462415060@qq.com
//    CreateTime:  2016-03-25 15:19:05Z
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using DG.Tweening;
public class CameraInstance  
{
    static CameraInstance instance = null;
    public static CameraInstance Instance
    {
        get
        {
            if ( instance == null )
                instance = new CameraInstance();
            return instance;
        }
    }

    public void ShakeCamera(float strength ,float duration)
    {
        Debug.Log("ShakeCamera ==== " + strength.ToString() + " " + duration.ToString());
        Camera.main.DOShakePosition(duration, strength);
    }
    public void MoveCamera( Vector3 destPos,float duartion)
    {
        Debug.Log( "movecamera +++ " + destPos.ToString() );
        Camera.main.transform.DOMove(destPos, duartion);
    }
}
