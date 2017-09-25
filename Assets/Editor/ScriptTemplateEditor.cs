//=========================================================================================
//
//    Author: zhudianyu
//
//    CreateTime:  2015-09-29 20:52:11
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
public class ScriptTemplateEditor : UnityEditor.AssetModificationProcessor 
{

    public static void OnWillCreateAsset(string path)
    {

        path = path.Replace(".meta", "");
        int npos = path.LastIndexOf(".");
        if (npos >= path.Length)
            return;
        //Debug.Log("npos = " + npos.ToString());
        //Debug.Log("path = " + path + " length " + path.Length.ToString());
        if (!path.EndsWith(".cs"))
            return;
        string file = path.Substring(npos);

        if (!file.EndsWith(".cs"))
            return;
        int index = Application.dataPath.LastIndexOf("Assets");
        path = Application.dataPath.Substring(0, index) + path;
        file = System.IO.File.ReadAllText(path);

        file = file.Replace("#CreateTime#", System.DateTime.Now.ToString("u"));

        System.IO.File.WriteAllText(path, file,System.Text.Encoding.UTF8);
        AssetDatabase.Refresh();

    }
}
