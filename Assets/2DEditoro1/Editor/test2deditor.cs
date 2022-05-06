using UnityEditor;
using UnityEngine;

public class test2deditor : EditorWindow
{
    private string pwd = "a pwd";

    //当按下按钮，选择"username"文本字段，也就是获得焦点
    private string username = "username";

    private void OnGUI()
    {
        //设置文本字段一个内部名字c
        GUI.SetNextControlName("MyTextField");

        //创建用户名和密码文本字段
        username = GUI.TextField(new Rect(10, 10, 100, 20), username);
        pwd = GUI.TextField(new Rect(10, 40, 100, 20), pwd);

        // 如果按下按钮键盘焦点将移动到用户名文本字段
        if (GUI.Button(new Rect(10, 70, 80, 20), "Move Focus"))
            GUI.FocusControl("MyTextField");
    }

    [MenuItem("Tools/2DEditoro1/test2deditor")]
    private static void ShowWindow()
    {
        var window = GetWindow<test2deditor>();
        window.titleContent = new GUIContent("test");
        window.Show();
    }
}