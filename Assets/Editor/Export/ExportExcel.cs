using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Export
{
    public static class ExportExcel
    {
        const string ConfigClassTemplateFileAssetPath = "Assets/Editor/Export/ConfigClassTemplate.txt";//程序具体调用的配置类
        const string ConfigDefineClassTemplateFileAssetPath = "Assets/Editor/Export/ConfigDefineClassTemplate.txt";//每行数据定义的类
        const string ConfigDataTemplateFileAssetPath = "Assets/Editor/Export/ConfigDataTemplate.txt";//初始化数据到配置类的类
        const string skipFieldMarkPrefix = "#";
        static string excelDirFullPath = Application.dataPath.Replace("Assets", "Excel");
        static string exportDirPath = Application.dataPath + "/Scripts/Data/";

        static int divisionCount = 1000;

        public static Dictionary<string, System.Func<object, string>> checkFieldTypeDic = new Dictionary<string, System.Func<object, string>>()
        {
            { "string",ConvertValueToString},//测试文
            { "string[]",ConvertValueToStringArray}, //"测,试，文本","测,试"
            {"string[][]",ConvertValueToMultipleStringArray },//{"1","2"},{"2","3"},{"45","1试"}
            { "int",ConvertValueToInt},//3872
            { "int[]",ConvertValueToIntArray},//1232,34232,1234
            { "int[,]",ConvertValueTo2DIntArray},//{0,0,0},{0,0,0}
            { "int[][]",ConvertValueToMultipleIntArray},//{80,10,10},{80,10,10},{80,10,10},{80,10,10}
            {"Vector3",ConvertValueToVector3 },
            {"Vector3[]",ConvertValueToVector3Array },
            { "float",ConvertValueToFloat},//38.72
            { "float[]",ConvertValueToFloatArray},//12.32,342.32,12.34
            { "float[,]",ConvertValueTo2DFloatArray},//{0,0,0},{0,0,0}
            { "float[][]",ConvertValueToMultipleFloatArray},//{80,10,10},{80,10,10},{80,10,10},{80,10,10}
            { "bool",ConvertValueToBool},
            {"long[]",ConvertValueToLongArray},
            {"long",ConvertValueToLong },
            {"double",ConvertValueToDouble },
            {"double[]",ConvertValueToDoubleArray },
            {"double[][]",ConvertValueToDoubleTwoArray },
        };

        [MenuItem("配置/导出/配置表")]
        public static void ExportExcels()
        {
            string[] tablePaths = Directory.GetFiles(excelDirFullPath, "*", SearchOption.TopDirectoryOnly);
            ExportExcelInFilePath(tablePaths);
        }

        /// <summary>
        /// 挑选出excel文件并导出
        /// </summary>
        /// <param name="tableFilePaths"></param>
        public static void ExportExcelInFilePath(string[] tableFilePaths)
        {
            foreach (string fullPath in tableFilePaths)
            {
                if (fullPath.EndsWith(".xls") || fullPath.EndsWith(".xlsx"))
                {
                    DebugCtrl.Log($"导出配置: {Path.GetFileName(fullPath)}", Color.cyan);
                    ExportExcelFile(fullPath);
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            DebugCtrl.Log("====配置表导出完成====", Color.cyan);
        }

        /// <summary>
        /// 提取单个excel文件里信息
        /// </summary>
        /// <param name="tablePath"></param>
        public static void ExportExcelFile(string fileFullPath)
        {
            Dictionary<string, List<List<object>>> excelInfoMap = new Dictionary<string, List<List<object>>>();
            // 处理每个表
            using (FileStream stream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = null;
                if (fileFullPath.EndsWith(".xlsx"))
                    workbook = new XSSFWorkbook(fileFullPath);//2007
                else if (fileFullPath.EndsWith(".xls"))
                    workbook = new HSSFWorkbook(stream);//2003

                // 处理每个sheet
                int sheetNumber = workbook.NumberOfSheets;
                for (int sheetIndex = 0; sheetIndex < sheetNumber; sheetIndex++)
                {
                    string sheetName = workbook.GetSheetName(sheetIndex);
                    //表名是以#开头，则跳过
                    if (sheetName.StartsWith(skipFieldMarkPrefix) || sheetName.StartsWith("Sheet"))
                        continue;

                    List<List<object>> pickInfoList = new List<List<object>>();
                    ISheet sheet = workbook.GetSheetAt(sheetIndex);
                    sheet.ForceFormulaRecalculation = true; //强制公式计算

                    int maxColumnNum = sheet.GetRow(0).LastCellNum;
                    // 处理每行
                    for (int rowId = 0; rowId <= sheet.LastRowNum; rowId++)
                    {
                        List<object> rowInfoList = new List<object>();
                        IRow sheetRowInfo = sheet.GetRow(rowId);
                        if (sheetRowInfo == null)
                        {
                            Debug.LogErrorFormat("无法获取行数据 sheetName={0} ;rowId={1};rowMax={2}", sheetName, rowId, sheet.LastRowNum);
                        }
                        var rowFirstCell = sheetRowInfo.GetCell(0);
                        //跳过空行
                        if (null == rowFirstCell)
                            continue;
                        if (rowFirstCell.CellType == CellType.Blank || rowFirstCell.CellType == CellType.Unknown || rowFirstCell.CellType == CellType.Error)
                            continue;

                        for (int columnId = 0; columnId < maxColumnNum; columnId++)
                        {
                            ICell pickCell = sheetRowInfo.GetCell(columnId);

                            if (pickCell != null && pickCell.IsMergedCell)
                            {
                                pickCell = GetMergeCell(sheet, pickCell.RowIndex, pickCell.ColumnIndex);
                            }
                            else if (pickCell == null)
                            {// 有时候合并的格子索引为空,就直接通过索引去找合并的格子
                                pickCell = GetMergeCell(sheet, rowId, columnId);
                            }

                            //公式结果
                            if (pickCell != null && pickCell.CellType == CellType.Formula)
                            {
                                pickCell.SetCellType(CellType.String);
                                rowInfoList.Add(pickCell.StringCellValue.ToString());
                            }
                            else if (pickCell != null)
                            {
                                rowInfoList.Add(pickCell.ToString());
                            }
                            else
                            {
                                rowInfoList.Add("");
                            }

                        }

                        pickInfoList.Add(rowInfoList);
                    }

                    excelInfoMap.Add(sheetName, pickInfoList);
                }
            }

            foreach (var item in excelInfoMap)
            {
                ParseExcelToCS(item.Value, item.Key, item.Key, exportDirPath);
            }
        }

        /// <summary>
        /// 获取合并的格子的原格子
        /// 合并格子的首行首列就是合并单元格的信息
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        private static ICell GetMergeCell(ISheet sheet, int rowIndex, int colIndex)
        {
            for (int ii = 0; ii < sheet.NumMergedRegions; ii++)
            {
                var cellrange = sheet.GetMergedRegion(ii);
                if (colIndex >= cellrange.FirstColumn && colIndex <= cellrange.LastColumn
                    && rowIndex >= cellrange.FirstRow && rowIndex <= cellrange.LastRow)
                {
                    var row = sheet.GetRow(cellrange.FirstRow);
                    var mergeCell = row.GetCell(cellrange.FirstColumn);

                    return mergeCell;
                }
            }
            Debug.LogWarning($"找不到合并的格子：{sheet.SheetName},[{rowIndex},{colIndex}]格子为空！");
            return null;
        }

        static void ParseExcelToCS(List<List<object>> totalRowInfoList, string tableName, string saveFileName, string exportDirPath)
        {
            TextAsset formatContent = (TextAsset)AssetDatabase.LoadAssetAtPath(ConfigDefineClassTemplateFileAssetPath, typeof(TextAsset));
            TextAsset exSubContent = (TextAsset)AssetDatabase.LoadAssetAtPath(ConfigDataTemplateFileAssetPath, typeof(TextAsset));
            if (!exportDirPath.EndsWith("/"))
                exportDirPath += "/";
            string saveFullPath = exportDirPath + saveFileName + ".cs";
            if (File.Exists(saveFullPath))
                File.Delete(saveFullPath);

            StringBuilder fieldContent;
            List<StringBuilder> dataContent;
            StringBuilder classParamContent;
            StringBuilder assignmentContent;
            StringBuilder firstFieldTypeContent;
            PickTableClassFields(totalRowInfoList, tableName, out fieldContent, out dataContent, out classParamContent, out assignmentContent, out firstFieldTypeContent);

            string convertTableName = tableName;
            StringBuilder saveContent = new StringBuilder(formatContent.text);
            //表名
            saveContent = saveContent.Replace("[CONFIGNAME]", convertTableName);
            //字段定义
            saveContent = saveContent.Replace("[FIELDNAMES]", fieldContent.ToString());
            //实例化时的字段名
            saveContent = saveContent.Replace("[CLASSPARAMS]", classParamContent.ToString());
            //赋值
            saveContent = saveContent.Replace("[ASSIGNMENT]", assignmentContent.ToString());

            int maxDataClassIndex = dataContent.Count;
            TextAsset configClassFormat = (TextAsset)AssetDatabase.LoadAssetAtPath(ConfigClassTemplateFileAssetPath, typeof(TextAsset));
            StringBuilder configClassContent = new StringBuilder(configClassFormat.text);
            configClassContent = configClassContent.Replace("[CONFIGNAME]", tableName);
            configClassContent = configClassContent.Replace("[CONFIGNAME_CONVERT]", convertTableName);
            configClassContent = configClassContent.Replace("[FIRSTFIELDTYPE]", firstFieldTypeContent.ToString());
            StringBuilder addListContent = new StringBuilder();
            for (int i = 0; i < maxDataClassIndex; i++)
                addListContent.AppendFormat("{0}{1}.data , ", tableName, i.ToString());
            configClassContent = configClassContent.Replace("[ADDLIST]", addListContent.ToString());

            saveContent.Append("\n\r");
            saveContent.Append(configClassContent.ToString());


            for (int i = 0; i < maxDataClassIndex; i++)
            {
                StringBuilder unitDataContent = new StringBuilder(exSubContent.text);
                unitDataContent = unitDataContent.Replace("[CONFIGNAME]", tableName);
                unitDataContent = unitDataContent.Replace("[CONFIGNAME_CONVERT]", convertTableName);
                unitDataContent = unitDataContent.Replace("[INDEX]", i.ToString());
                unitDataContent = unitDataContent.Replace("[FIRSTFIELDTYPE]", firstFieldTypeContent.ToString());
                //添加具体配置参数
                unitDataContent = unitDataContent.Replace("[CONFIG_DATA]", dataContent[i].ToString());
                saveContent.Append("\n\r");
                saveContent.Append(unitDataContent.ToString());
            }

            using (StreamWriter sw = new StreamWriter(File.Create(saveFullPath)))
                sw.Write(saveContent.ToString());
        }

        static void PickTableClassFields(List<List<object>> totalRowInfoList, string tableName, out StringBuilder fieldContent, out List<StringBuilder> dataContent, out StringBuilder classParamContent, out StringBuilder assignmentContent, out StringBuilder firstFieldTypeContent)
        {
            //第一行是备注，第二行是字段类型，第三行是字段名
            List<object> oneRowInfoList = totalRowInfoList[0];
            List<object> twoRowInfoList = totalRowInfoList[1];
            List<object> threeRowInfoList = totalRowInfoList[2];

            fieldContent = new StringBuilder(); //替换[FIELDNAMES]
            dataContent = new List<StringBuilder>();//替换[ELEMENTS]
            classParamContent = new StringBuilder();//替换[CLASSPARAMS]
            assignmentContent = new StringBuilder("");//替换[ASSIGNMENT]
            firstFieldTypeContent = new StringBuilder(twoRowInfoList[0].ToString());//替换[FIRSTFIELDTYPE]

            StringBuilder unitDataFormatContent = new StringBuilder(string.Format("new {0}Class(", tableName));
            List<int> validIndexList = new List<int>();
            List<string> checkSameParamNameList = new List<string>();

            string prefixStr = "    public";
            int maxIndex = oneRowInfoList.Count - 1;
            for (int i = 0; i <= maxIndex; i++)
            {
                //当字段说明是以#开头，则跳过该字段
                string remarkStr = oneRowInfoList[i].ToString();
                if (remarkStr.StartsWith(skipFieldMarkPrefix))
                    continue;
                string fieldNameStr = threeRowInfoList[i].ToString();
                if (fieldNameStr.StartsWith(skipFieldMarkPrefix))
                    continue;

                if (checkSameParamNameList.Contains(fieldNameStr))
                {
                    Debug.LogErrorFormat("{0}存在相同的参数名字{1}", tableName, fieldNameStr);
                    continue;
                }
                checkSameParamNameList.Add(fieldNameStr);

                string fieldTypeStr = twoRowInfoList[i].ToString();

                if (checkFieldTypeDic.ContainsKey(fieldTypeStr))
                {
                    fieldContent.AppendFormat("    /// <summary>\r\n    /// {0}\r\n    /// </summary>\r\n", remarkStr.Replace("\n", "     "));
                    fieldContent.AppendFormat("{0} {1} {2};\r\n", prefixStr, fieldTypeStr, fieldNameStr);

                    if (classParamContent.Length > 0)
                    {
                        classParamContent.Append(", ");
                        unitDataFormatContent.Append(", ");
                    }

                    classParamContent.AppendFormat("{0} {1}", fieldTypeStr, fieldNameStr);
                    unitDataFormatContent.AppendFormat("[p{1}]", fieldTypeStr, i.ToString());
                    assignmentContent.AppendFormat("        this.{0} = {0};\r\n", fieldNameStr, fieldNameStr);

                    validIndexList.Add(i);
                }
                else
                {
                    Debug.LogErrorFormat("不支持解析字段类型{0}\n表名={1} ; 列={2}", fieldTypeStr, tableName, i.ToString());
                    continue;
                }
            }

            //unitDataFormatContent.Append(")");
            //这里加上[]是为了防止部分重名参数
            unitDataFormatContent.Insert(0, "        {[p0] , ").Append(") }");


            int dataAppendIndex = 0;
            StringBuilder eachDataContent = new StringBuilder();
            //从第四行开始读取
            int rowInfoCount = totalRowInfoList.Count;
            for (int i = 3; i < rowInfoCount; i++)
            {
                List<object> rowInfoList = totalRowInfoList[i];
                StringBuilder unitContent = new StringBuilder(unitDataFormatContent.ToString());
                for (int j = 0; j < validIndexList.Count; j++)
                {
                    int realTableColumn = validIndexList[j];
                    object value = rowInfoList[realTableColumn];
                    string fieldTypeStr = twoRowInfoList[realTableColumn].ToString();
                    string convertStr = checkFieldTypeDic[fieldTypeStr].Invoke(value);
                    if (convertStr.StartsWith("error"))
                    {
                        Debug.LogErrorFormat("配置表中值的格式错误。表名={0} ; 行={1} ; 列={2}", tableName, (i + 1).ToString(), (realTableColumn + 1).ToString());
                    }
                    else
                    {
                        string replaceTagStr = "[p" + realTableColumn.ToString() + "]";
                        unitContent = unitContent.Replace(replaceTagStr, convertStr);
                    }
                }

                eachDataContent.Append(unitContent);
                eachDataContent.Append(",\n");
                dataAppendIndex++;
                //超过1000行另外新建一个表存储
                if (dataAppendIndex >= divisionCount)
                {
                    dataAppendIndex = 0;
                    dataContent.Add(eachDataContent);
                    eachDataContent = new StringBuilder();
                }
                else if (i == rowInfoCount - 1)
                {
                    dataAppendIndex = 0;
                    dataContent.Add(eachDataContent);
                    eachDataContent = new StringBuilder();
                }
            }
        }
        static string ConvertValueToString(object value)
        {
            if (value == null)
                return "null";

            string fValue = value.ToString();
            if (fValue == "null")
                return "null";

            return '"' + fValue + '"';
        }

        static string ConvertValueToStringArray(object value)
        {
            if (value == null)
                return "new string[] {" + "}";
            var strs = value.ToString().Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            for (int i = 0; i < strs.Length; i++)
            {
                sb.Append(ConvertValueToString(strs[i]));
                if (i != strs.Length - 1) sb.Append(",");
            }
            return "new string[] {" + sb.ToString() + "}";
        }

        static string ConvertValueToMultipleStringArray(object value)
        {
            if (value == null)
                return "new string[][] {" + "}";

            return "new string[][]{" + "new string[]" + value.ToString().Replace("},{", "},new string[] {") + "}";
        }

        static string ConvertValueToInt(object value)
        {
            if (value == null)
                return "error：整形元素必须赋值";
            return value.ToString();
        }

        static string ConvertValueToIntArray(object value)
        {
            return "new int[] {" + value.ToString() + "}";
        }

        static string ConvertValueTo2DIntArray(object value)
        {
            return "new int[,]{" + value.ToString() + "}";
        }

        static string ConvertValueToMultipleIntArray(object value)
        {
            return "new int[][]{" + "new int[]" + value.ToString().Replace("},{", "},new int[] {") + "}";
        }

        static string ConvertValueToFloat(object value)
        {
            if (value == null)
                return "error：浮点元素必须赋值";
            return string.Format("{0}f", value.ToString());
        }

        //配置表中格式1.1,2.2,3.3  转为
        static string ConvertValueToFloatArray(object value)
        {
            string content = value.ToString().Replace(" ", "");
            string[] splitContentArray = content.Split(',');

            StringBuilder result = new StringBuilder("new float[] {");
            int arrayLength = splitContentArray.Length;
            int arrayMaxIndex = arrayLength - 1;
            for (int i = 0; i < arrayLength; i++)
            {
                result.Append(string.Format("{0}f", splitContentArray[i]));
                if (i < arrayMaxIndex)
                    result.Append(", ");
            }

            result.Append(" }");
            return result.ToString();
        }

        //配置表中格式{0,0,0},{0,0,0}
        static string ConvertValueTo2DFloatArray(object value)
        {
            return ("new float[,]{" + value.ToString().Replace(",", "f,") + "f}").Replace("}f", "f}");
        }

        //配置表中格式{80,10,10},{80,10,10},{80,10,10},{80,10,10}
        static string ConvertValueToMultipleFloatArray(object value)
        {
            return "new float[][]{" + "new float[]" + (value.ToString().Replace(",", "f,").Replace("}", "f}").Replace("}f", "}")).Replace("},{", "},new float[] {") + "}";
        }


        //配置表中格式(1,2.0,3.0)
        static string ConvertValueToVector3(object value)
        {
            string floatRegexStr = @"(-?\d+\.?\d*)";
            string vector3RegexStr = @"^\((-?\d+\.?\d*),(-?\d+\.?\d*),(-?\d+\.?\d*)\)$";
            Regex regex = new Regex(vector3RegexStr);
            string content = value.ToString().Replace(" ", "");
            if (regex.IsMatch(content))
            {
                MatchCollection collection = regex.Matches(content);
                string result = regex.Replace(content, "new Vector3($1f,$2f,$3f)");
                return result;
            }
            else
                return "error: Vector3类型错误";
        }

        static string ConvertValueToVector3Array(object value)
        {
            string content = (value as string).Replace(" ", "");
            if (content.Contains("),("))
            {
                string[] splitContent = content.Split(new System.String[] { "),(" }, System.StringSplitOptions.RemoveEmptyEntries);
                StringBuilder result = new StringBuilder("new Vector3[] {");
                int splitCount = splitContent.Length;
                int maxIndex = splitCount - 1;
                for (int i = 0; i < splitCount; i++)
                {
                    string tmpStr = splitContent[i];
                    if (!tmpStr.StartsWith("("))
                        tmpStr = "(" + tmpStr;
                    if (!tmpStr.EndsWith(")"))
                        tmpStr = tmpStr + ")";

                    result.Append(ConvertValueToVector3(tmpStr));
                    if (i < maxIndex)
                    {
                        result.Append(", ");
                    }
                }
                result.Append("}");
                return result.ToString();
            }
            else
            {
                return "new Vector3[] {" + ConvertValueToVector3(content) + "}";
            }
        }


        static string ConvertValueToBool(object value)
        {
            return value.ToString().ToLower();
        }

        static string ConvertValueToLong(object value)
        {
            if (value == null)
                return "error：整型元素必须赋值";
            return value.ToString();
        }
        static string ConvertValueToLongArray(object value)
        {
            if (value == null)
                return "error：整型元素必须赋值";

            return "new long[] {" + value.ToString() + "}";
        }

        static string ConvertValueToDouble(object value)
        {
            if (value == null)
                return "error：值类型必须赋值";

            return value.ToString() + "D";
        }

        static string ConvertValueToDoubleArray(object value)
        {
            if (value == null)
                return "error：值类型必须赋值";

            string content = value.ToString().Replace(" ", "");
            string[] splitContentArray = content.Split(',');

            StringBuilder result = new StringBuilder("new double[] {");
            int arrayLength = splitContentArray.Length;
            int arrayMaxIndex = arrayLength - 1;
            for (int i = 0; i < arrayLength; i++)
            {
                result.Append(string.Format("{0}D", splitContentArray[i]));
                if (i < arrayMaxIndex)
                    result.Append(",");
            }

            result.Append(" }");
            return result.ToString();
        }

        static string ConvertValueToDoubleTwoArray(object value)
        {
            if (value == null)
                return "error：值类型必须赋值";

            return "new double[][]{" + "new double[]" + (value.ToString().Replace(",", "D,").Replace("}", "D}").Replace("}D", "}")).Replace("},{", "},new double[] {") + "}";
        }
    }
}


