using DriftStorm;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameMaker : Editor
{
    [MenuItem("工具栏/清除数据", priority = -100)]
    public static void ClearData()
    {
        DataCtrl.Instance.Clear();
    }
}
