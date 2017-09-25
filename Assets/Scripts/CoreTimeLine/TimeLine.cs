//=========================================================================================
//
//    Author: zhudianyu
//    Email:  1462415060@qq.com
//    CreateTime:  2016-03-15 11:26:04Z
//
//=========================================================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEditorInternal;
using System.IO;
namespace TimeLineCore
{
    public class TimeLine
    {
        #region  fileds
        private Rect curTimeLineRect = new Rect( 0 , 0 , 0 , 0 );

        private int LABLE_WIDTH = 100;
        //时间线第一个格子的宽度 显示label信息
        public int LabelWidth
        {
            set
            {
                LABLE_WIDTH = value;
            }
            get
            {
                return LABLE_WIDTH;
            }
        }
        private int LABEL_HEIGHT = 30;
        //时间线上第一个格子的高度
        public int LabelHeight
        {
            set
            {
                LABEL_HEIGHT = value;
            }
            get
            {
                return LABEL_HEIGHT;
            }
        }
        private int GRID_WIDTH = 30;
        //时间线上格子宽度
        public int GridWidth
        {
            set
            {
                GRID_WIDTH = value;
            }
            get
            {
                return GRID_WIDTH;
            }

        }
        private int GRID_HEIGHT = 30;
        //时间线上格子高度
        public int GridHeight
        {
            set
            {
                GRID_HEIGHT = value;
            }
            get
            {
                return GRID_HEIGHT;
            }
        }
        private int totalGridCount = 500;
        public int TotalGridCount
        {
            get;
            set;
        }

        private int TITLE_WIDTH = 100;
        //标题栏格子宽度
        public int TitleWidth
        {
            set
            {
                TITLE_WIDTH = value;
            }
            get
            {
                return TITLE_WIDTH;
            }
        }
        private int TITLE_HEIGHT = 30;
        //标题栏格子高度
        public int TitleHeight
        {
            set
            {
                TITLE_HEIGHT = value;
            }
            get
            {
                return TITLE_HEIGHT;
            }
        }
        public int SETTING_HEIGHT = 20;
        //当前选中的grid的横向索引  从0开始
        private int curSelectGridX = -1;
        public int CurSelectGridX
        {
            set
            {
                curSelectGridX = value;
            }
            get
            {
                return curSelectGridX;
            }
        }

        //当前选中的grid的纵向索引 从0开始
        private int curSelectGridY = -1;
        public int CurSelectGridY
        {
            set
            {
                curSelectGridY = value;
                TimeLineDataManager.Instance.SelectRow = curSelectGridY;
            }
            get
            {
                return curSelectGridY;
            }
        }

        Vector2 _curScorllPos = Vector2.zero;
        private int timeLinesCount = 0;
        //时间线数目
        public int TimeLinesCount
        {
            get;
            set;
        }

        private Color selectColor = new Color( 0.2f , 0f , 1f );

        #endregion

        static bool bEdit = true;
        static bool bSetting = false;
        static bool bFilemenu = false;
        //  Dictionary<string, TimeLineData> lineDataDic = new Dictionary<string, TimeLineData>();
        List<Type> typeList = new List<Type>();
        static int SelectAddTimeLineIndex = 0;
        static string addLineName = "name";

        public TimeLine()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach ( var t in types )
            {
                if ( IsSubClassOf( t , typeof( TimeLineData ) ) )
                {
                    typeList.Add( t );
                }
            }
        }
        bool IsSubClassOf(Type type , Type baseType)
        {
            var b = type.BaseType;
            while ( b != null )
            {
                if ( b.Equals( baseType ) )
                {
                    return true;
                }
                b = b.BaseType;
            }
            return false;
        }
        #region TABGUI
        void OnTabGUI()
        {
            GUILayout.BeginHorizontal();
            if ( GUILayout.Button( "编辑" ) )
            {
                bEdit = true;
                bSetting = false;
                bFilemenu = false;
            }
            if ( GUILayout.Button( "设置" ) )
            {
                bEdit = false;
                bSetting = true;
                bFilemenu = false;
            }
            if ( GUILayout.Button( "文件" ) )
            {
                bEdit = false;
                bSetting = false;
                bFilemenu = true;
            }
            GUILayout.EndHorizontal();
        }
        public static void ShowEditUI()
        {
            bEdit = true;
            bSetting = false;
            bFilemenu = false;
        }
        public static void ShowSettingUI()
        {
            bEdit = false;
            bSetting = true;
            bFilemenu = false;
        }
        public static void ShowFileUI()
        {
            bEdit = false;
            bSetting = false;
            bFilemenu = true;
        }
        #endregion
        public void OnGUI()
        {
            OnTabGUI();
            if ( bEdit )
            {
                GUILayout.BeginArea( new Rect( 0 , SETTING_HEIGHT , Screen.width , Screen.height - SETTING_HEIGHT ) );
                OnDrawTimeRect();
                GUILayout.EndArea();
                Rect editRect = new Rect( 0 , 5 * GRID_HEIGHT + TITLE_HEIGHT + SETTING_HEIGHT + 20 + 10 , Screen.width , 30 );
                GUILayout.BeginArea( editRect );
                OnDrawEdit();
                GUILayout.EndArea();

                Rect customRect = new Rect( 0 , 5 * GRID_HEIGHT + TITLE_HEIGHT + SETTING_HEIGHT + 20 + 10 + 30 , Screen.width , 300 );

                GUILayout.BeginArea( customRect );
                OnDrawCustomGUI();
                GUILayout.EndArea();
            }

            if ( bSetting )
            {
                OnDrawSettingGUI();
            }
            if ( bFilemenu )
            {
                OnDrawFileMenuGUI();
            }

        }
        void OnDrawFileMenuGUI()
        {
            GUILayout.BeginArea( new Rect( 0 , SETTING_HEIGHT , Screen.width , Screen.height - SETTING_HEIGHT ) );
            if ( GUILayout.Button( "New File" ) )
            {
                bEdit = false;
                bSetting = true;
                bFilemenu = false;
            }
            if(GUILayout.Button("Load"))
            {
               TimeLineDataManager.Instance.filePath = EditorUtility.OpenFilePanel("", TimeLineDataManager.DEFAULT_SAVE_PATH, TimeLineDataManager.JSON_EXTENTION);
               if(!string.IsNullOrEmpty(TimeLineDataManager.Instance.filePath))
               {
                   TimeLineDataManager.Instance.LoadEditData(TimeLineDataManager.Instance.filePath);
                   ShowEditUI();
               }
               else
               {
                   EditorUtility.DisplayDialog("error", "路径为空", "ok");
               }
            }
            GUILayout.EndArea();
        }
        void OnDrawSettingGUI()
        {
            GUILayout.BeginArea( new Rect( 0 , SETTING_HEIGHT , Screen.width , Screen.height - SETTING_HEIGHT ) );
            TimeLineDataManager.Instance.ID = EditorGUILayout.IntField( "ID" , TimeLineDataManager.Instance.ID );
            TimeLineDataManager.Instance.fileName = EditorGUILayout.TextField( "Name" , TimeLineDataManager.Instance.fileName );
          
            TimeLineDataManager.Instance.filePath = EditorGUILayout.TextField( "文件保存路径" , TimeLineDataManager.Instance.filePath );
            if ( GUILayout.Button( "Create And Save" ) )
            {
                if ( string.IsNullOrEmpty( TimeLineDataManager.Instance.fileName ) )
                    EditorUtility.DisplayDialog( "提示" , "请先填写文件名!" , "确定" );
                else
                {

                    if ( !Directory.Exists( TimeLineDataManager.DEFAULT_SAVE_PATH ) )
                    {
                        Directory.CreateDirectory(TimeLineDataManager.DEFAULT_SAVE_PATH);
                    }
                    TimeLineDataManager.Instance.filePath = EditorUtility.SaveFilePanel( "设置路径" , TimeLineDataManager.DEFAULT_SAVE_PATH ,
                        TimeLineDataManager.Instance.fileName , TimeLineDataManager.JSON_EXTENTION );
                    Debug.Log("文件保存路径 是 " + TimeLineDataManager.Instance.filePath);
                    if (string.IsNullOrEmpty(TimeLineDataManager.Instance.filePath))
                        return;
                    TimeLineDataManager.Instance.LoadEditData(TimeLineDataManager.Instance.filePath);
                }
            }

         
         
            GUILayout.EndArea();
        }
        #region GUIEDIT
        void OnDrawCustomGUI()
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            if ( assembly != null )
            {
                try
                {
                    Type[] timetypes = assembly.GetTypes();
                    for ( int k = 0; k < timetypes.Length; k++ )
                    {
                        Type t = timetypes[k];
                        TimeLineData curData = TimeLineDataManager.Instance.GetTimeLineByIndex( CurSelectGridY );
                        if ( curData == null )
                            return;
                        if (curData.GetType() == t)
                        {
                            if ( IsSubClassOf( t , typeof( TimeLineData ) ) )
                            {
                                var baseTarget = TimeLineDataManager.Instance.GetCurSelectTimeLine();
                                baseTarget.OnGUI();
                             
                                foreach ( var infodic in TimeLineDataManager.Instance.memDic )
                                {
                                    var info = infodic.Value;
                                    if ( info.IsDefined( typeof( GridMenuItemAttribute ) , false ) )
                                    {
                                        var target = curData;
                                        t.InvokeMember( info.Name , BindingFlags.InvokeMethod , null , target , null );
                                        baseTarget.OnDrawDeleteButton( info );
                                    }

                                }
                            }
                        }

                    }

                }
                catch ( Exception )
                {
                }
            }

        }
        public void OnDrawEdit()
        {

            List<GUIContent> strList = new List<GUIContent>();
            foreach ( var t in typeList )
            {
                strList.Add( new GUIContent( GetTimeLineName( t ) ) );
            }
            GUILayout.BeginHorizontal();
            SelectAddTimeLineIndex = EditorGUILayout.Popup( SelectAddTimeLineIndex , strList.ToArray() , GUILayout.Width( 100 ) , GUILayout.Height( 20 ) );
            Type type = null;
            if ( SelectAddTimeLineIndex < typeList.Count )
                type = typeList[SelectAddTimeLineIndex];
            string prefix = strList[SelectAddTimeLineIndex].text;
            addLineName = GUILayout.TextField(addLineName, GUILayout.Height(20), GUILayout.Width(150));
            string lineName = prefix + addLineName;
            if ( GUILayout.Button( "Add" , GUILayout.Height( 20 ) , GUILayout.Width( 100 ) ) )
            {
                if (!TimeLineDataManager.Instance.IsCanAdd(prefix+addLineName))
               {
                   EditorUtility.DisplayDialog("error", "已经有相同的名字的timeline", "OK");
                   return;
               }
                var data = TimeLineDataManager.Instance.GetCurSelectTimeLine();
                if ( data != null )
                    data.SaveLineData();
                TimeLineDataManager.Instance.curOperationAction = null;
                if ( SelectAddTimeLineIndex < typeList.Count )
                {
                    Type t = typeList[SelectAddTimeLineIndex];
                    var td = Activator.CreateInstance( t ) as TimeLineData;
                    td.Init(0, lineName, totalGridCount);
                    TimeLineDataManager.Instance.AddTimeLine( td );
                }
                else
                {
                    Debug.Log( "error SelectAddTimeLineIndex " + SelectAddTimeLineIndex + " list count is " + typeList.Count );
                }
            }
            if ( GUILayout.Button( "-" , GUILayout.Height( 20 ) , GUILayout.Width( 100 ) ) )
            {
                if ( EditorUtility.DisplayDialog( "tips" , "确认删除" , "OK" , "Cancle" ) )
                {
                    int index = CurSelectGridY;
                    if ( index >= 0 && index < TimeLineDataManager.Instance.GetTimeLineCount() )
                    {
                        TimeLineDataManager.Instance.DeleteTimeLine( index );
                    }
                }
                else
                {
                    return;
                }
            }
            GUILayout.EndHorizontal();

        }
        private void OnDrawTimeRect()
        {
            Rect rect = new Rect( 0 , 0 , Screen.width , 5 * GRID_HEIGHT + TITLE_HEIGHT + 20 );

            GUILayout.BeginArea( rect );

            _curScorllPos = GUI.BeginScrollView( rect , _curScorllPos , new Rect( 0 , 0 , totalGridCount * GRID_WIDTH + TITLE_WIDTH , timeLinesCount * GRID_HEIGHT + TITLE_HEIGHT + 20 ) , true , true );

            OnTitleGUI();
            OnTimeLineGUI();
            GUI.EndScrollView();
            OnEventHandler();
            GUILayout.EndArea();

        }
        private void OnTitleGUI()
        {
            //StoryData storyData = StoryEditor.curStoryData;
            DrawBox( "剧情:" + "ceshi" , 0 , 0 , TITLE_WIDTH , TITLE_HEIGHT , Color.green );
            DrawBox( "" , TITLE_WIDTH , 0 , totalGridCount * GRID_WIDTH , TITLE_HEIGHT , Color.white );
            for ( int i = 0; i < totalGridCount; i += 5 )
            {
                int textX = TITLE_WIDTH + i * GRID_WIDTH;
                int textY = 0;
                int textWidth = 5 * GRID_WIDTH;
                int textHeight = TITLE_HEIGHT;
                DrawText( ( i * TimeLineDataManager.Instance.TickDelta ) + "s" , textX , textY , textWidth , textHeight , Color.black );
                DrawBox( "" , textX , textY , GRID_WIDTH , TITLE_HEIGHT , Color.green );
            }
        }

        void OnTimeLineGUI()
        {
            curTimeLineRect = new Rect( 0 , TITLE_HEIGHT , totalGridCount * GRID_WIDTH , 600 );
            GUILayout.BeginArea( curTimeLineRect );

            int len = TimeLineDataManager.Instance.GetTimeLineCount();
            for ( int i = 0; i < len; i++ )
            {

                TimeLineData data = TimeLineDataManager.Instance.GetTimeLineByIndex( i );
                DrawBox( data.TimeLineName , 0 , LABEL_HEIGHT * i , LABLE_WIDTH , LABEL_HEIGHT , Color.white );
                for ( int j = 0; j < totalGridCount; j++ )
                {
                    Color c = Color.white;
                    SortedDictionary<string , ActionItemData> acDic = data.GetActionDic();

                    foreach ( var acpair in acDic )
                    {
                        //当期操作的action 不画
                        ActionItemData ad = acpair.Value;
                        int st = (int)( ad.actionStartTime / TimeLineDataManager.Instance.TickDelta );
                        int et = (int)( ad.actionEndTime / TimeLineDataManager.Instance.TickDelta );
                        if ( j == st )
                            c = Color.green;
                        if ( j > st && j <= et )
                        {
                            c = Color.red;
                        }
                    }
                    if ( CurSelectGridY == i )
                    {
                        //画当前操作的action
                        ActionItemData tempad = TimeLineDataManager.Instance.curOperationAction;
                        if ( tempad != null )
                        {
                            int st = (int)( tempad.actionStartTime / TimeLineDataManager.Instance.TickDelta );
                            int et = (int)( tempad.actionEndTime / TimeLineDataManager.Instance.TickDelta );

                            if ( j == st )
                                c = selectColor;
                            if ( j > st && j <= et )
                            {
                                c = Color.red;
                            }
                        }
                        else
                        {
                            if ( j == CurSelectGridX )
                                c = selectColor;
                        }
                        
                    }

                    //if ( CurSelectGridX == j && CurSelectGridY == i )
                    //{
                    //    c = selectColor;
                    //}
                    DrawBox( "" , LABLE_WIDTH + j * GRID_WIDTH , LABEL_HEIGHT * i , GRID_WIDTH , GRID_HEIGHT , c );
                }
            }
            GUILayout.EndArea();

        }
        private void OnEventHandler()
        {

            #region Event
            switch ( Event.current.type )
            {
                case EventType.KeyUp:
                    SetSelectIndex();
                    break;
                case EventType.KeyDown:
                    SetSelectIndex();
                    break;
                case EventType.MouseUp:
                    if(SetSelectIndex())
                    {
                        if (Event.current.button == 0)
                        {
                            LeftClick();
                        }
                        else
                        {
                            ShowMenu();
                        }
                        Event.current.Use();
                    }
                   
                    break;

                default:
                    break;
            }
            #endregion
        }
        void LeftClick()
        {

            //左键单击
            // CurMemInfo = null;
            TimeLineData data = TimeLineDataManager.Instance.GetCurSelectTimeLine();
            if ( data == null )
                return;
            data.InitDefaultTime();
            Type t = data.GetType();
            data.SetFieldValue( data );
            var memInfo = t.GetMethods();
            TimeLineDataManager.Instance.memDic.Clear();
            int selectKey = -1;
            TimeLineDataManager.Instance.curOperationAction = null;
            foreach ( MethodInfo info in memInfo )
            {
                foreach ( var item in data.GetActionDic() )
                {
                    ActionItemData ad = item.Value;
                    if ( ad.IsSelect() )
                    {
                        foreach ( var name in ad.methodNameList )
                        {
                            if ( name == info.Name )
                            {
                                TimeLineDataManager.Instance.AddGUIMemInfo( info );
                            }
                        }
                        //移除要操作的action 用临时变量保存
                        TimeLineDataManager.Instance.curOperationAction = ad.Clone();
                        selectKey = ad.index;
                    }
                    else
                    {
                        if ( LastSelectGridY != CurSelectGridY )
                        {
                            //更换timeline 
                            //  TimeLineDataManager.Instance.curOperationAction = null;
                        }
                    }
                }
            }

            data.DeleteAction( selectKey );
        }

        void ShowMenu()
        {

            GenericMenu menu = new GenericMenu();
            if ( CurSelectGridY < 0 )
                return;
            if ( CurSelectGridY < TimeLineDataManager.Instance.GetTimeLineCount() )
            {
                var data = TimeLineDataManager.Instance.GetTimeLineByIndex( CurSelectGridY );
                foreach ( var item in data.GetActionDic() )
                {
                    ActionItemData ad = item.Value;
                    if ( ad.IsSelect() )
                    {
                        int index = Mathf.CeilToInt(ad.actionStartTime / TimeLineDataManager.Instance.TickDelta);
                        if(index != CurSelectGridX)
                            return;
                    }
                }

                Type t = data.GetType();
                string testName = GetTimeLineName( t );

                menu.AddDisabledItem( new GUIContent( testName ) );
                menu.AddSeparator( "" );
                var memInfo = t.GetMethods();
              
                foreach ( MethodInfo info in memInfo )
                {
                    GridMenuItemAttribute[] attrArray = info.GetCustomAttributes( typeof( GridMenuItemAttribute ) , false ) as GridMenuItemAttribute[];
                    for ( int i = 0; i < attrArray.Length; i++ )
                    {

                        GridMenuItemAttribute attrbute = attrArray[i];
                        // menu.AddItem(new GUIContent(attrbute.MenuDescription), false, (GenericMenu.MenuFunction2)DelegateCreate(info, typeof(GenericMenu.MenuFunction2), target), "adc");
                        menu.AddItem( new GUIContent( attrbute.MenuDescription ) , false , ClickMenuItem , info );

                    }
                }

                menu.ShowAsContext();
            }

        }
        private void DrawBox(string text , float x , float y , float width , float height , Color color)
        {
            Color oldColor = GUI.color;
            GUI.color = color;
            GUI.Box( new Rect( x , y , width , height ) , new GUIContent( text ) );
            GUI.color = oldColor;
        }

        private void DrawText(string text , float x , float y , float width , float height , Color color)
        {
            Color oldColor = GUI.color;
            GUI.color = color;
            GUI.Label( new Rect( x , y , width , height ) , new GUIContent( text ) );
            GUI.color = oldColor;
        }
        #endregion

        #region editlogic
        void ClickMenuItem(object memInfo)
        {
            if ( memInfo != null )
                TimeLineDataManager.Instance.AddGUIMemInfo( (MemberInfo)memInfo );
            var data = TimeLineDataManager.Instance.GetCurSelectTimeLine();
            ActionItemData ad = data.GetActionItemData( TimeLineDataManager.Instance.SelectColumn );
            if ( ad == null )
            {
                List<string> list = new List<string>();
                MemberInfo info = memInfo as MemberInfo;
                list.Add( info.Name );
                ad = new ActionItemData(data.GetType(), list);
                TimeLineDataManager.Instance.curOperationAction = ad;
                data.AddAction( ad );

            }
        }

        public Delegate DelegateCreate(MethodInfo method , Type delegateType , object target = null)
        {
            if ( method == null )
                return null;
#if UNITY_METRO && !UNITY_EDITOR
			return target == null ? method.CreateDelegate(delegateType) : method.CreateDelegate(delegateType, target);
#else
            return target == null ? Delegate.CreateDelegate( delegateType , method ) : Delegate.CreateDelegate( delegateType , target , method );
#endif
        }

        int LastSelectGridY = -1;
        /// <summary>
        /// 设置选中的索引
        /// </summary>
        bool SetSelectIndex()
        {

            Vector2 mousePos = Event.current.mousePosition;
            int count = TimeLineDataManager.Instance.GetTimeLineCount();
            Vector2 clickPos = new Vector2( mousePos.x - curTimeLineRect.x - LABLE_WIDTH , mousePos.y - curTimeLineRect.y ) + _curScorllPos;
            Rect containRect = new Rect( 0 , 0 , curTimeLineRect.width , Mathf.Min( count , 3 ) * GRID_HEIGHT + TITLE_HEIGHT + 20 );
            if ( !containRect.Contains( clickPos ) )
            {
                //CurSelectGridY = -1;
                //CurSelectGridX = -1;
                return false;
            }
            SaveTempAction();

            CurSelectGridX = (int)clickPos.x / GRID_WIDTH;
            LastSelectGridY = CurSelectGridY;
            CurSelectGridY = (int)clickPos.y / GRID_HEIGHT;

            TimeLineDataManager.Instance.SelectColumn = curSelectGridX;
            return true;
            //Debug.Log("当前选中第" + (curSelectGridY + 1).ToString() + "行，第" + (CurSelectGridX + 1).ToString() + "列");
        }
        void SaveTempAction()
        {
            var data = TimeLineDataManager.Instance.GetCurSelectTimeLine();
            if ( data != null )
            {
                var tempad = TimeLineDataManager.Instance.curOperationAction;
                if ( tempad != null )
                {//切换要操作的action之前 先保存上一个
                    if ( tempad.methodNameList.Count == 0 )
                        return;
                    tempad = data.SetActionValue( tempad );
                    data.AddAction( tempad );
                }
            }
        }
        string GetTimeLineName(Type t)
        {
            TimeLineAttribute[] attArray = t.GetCustomAttributes( typeof( TimeLineAttribute ) , false ) as TimeLineAttribute[];
            foreach ( var att in attArray )
            {
                return att.MenuDescription;
            }
            return t.Name;
        }
        #endregion
    }
}