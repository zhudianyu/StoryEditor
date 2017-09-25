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
using UnityEngine.UI;
public class SystemInstance 
{

    static SystemInstance instance = null;
    public static SystemInstance Instance
    {
        get
        {
            if (instance == null)
                instance = new SystemInstance();
            return instance;
        }
    }
	public void ShowTips(string text,float duration)
    {
        Debug.Log("text is " + text + " duration is " + duration.ToString());
        GameObject go = GameObject.Find("Text");
        Text t = go.GetComponent<Text>();
        t.color = Color.red;
        t.DOText(text, duration);
    }
}
