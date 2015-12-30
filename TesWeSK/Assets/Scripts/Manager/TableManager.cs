using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using System.Xml;

public sealed class TableManager
{
    private static TableManager m_instance;
    public static TableManager instance
    {
        get { return m_instance; }
    }
    public static void Create()
    {
        if (m_instance != null)
            m_instance = null;
        m_instance = new TableManager();
    }

    private TableManager()
    {
        tables = new Dictionary<Type, Dictionary<int, object>>();
        Init();
    }

    private Dictionary<Type, Dictionary<int, object>> tables;

    public T GetPropertiesById<T>(int id)
    {
        Type type = typeof(T);
        Dictionary<int, object> table = tables[type];
        if (table != null)
        {
            if (table.ContainsKey(id))
                return (T)table[id];
            else
                return default(T);
        }
        return default(T);
    }
    public Dictionary<int, T> GetAlllPropertiesByType<T>()
    {
        Type type = typeof(T);
        Dictionary<int, T> ret = new Dictionary<int, T>();
        var itr = tables[type].GetEnumerator();

        while (itr.MoveNext())
        {
            ret.Add(itr.Current.Key, (T)(itr.Current.Value));
        }
        return ret;
    }
    public void Init()
    {
        GeneTables();
    }

    public static void ClearXmlData<T>()
    {
        Type t = typeof(T);
        string filePath = Application.dataPath + "/Resources/Properties/" + t.Name + ".xml";
        string content = FileOpt.ReadFile(filePath);
        int index = 0;
        for (int i = 0; i < 3; i++)
        {
            if (index >= 0)
                index = content.IndexOf("</Row>", index + 6);
        }
        if (index >= 0)
        {
            index += 7;
            int lastIndex = content.IndexOf("</Table>", index);
            content = content.Substring(0, index) + content.Substring(lastIndex, content.Length - lastIndex);
        }
        FileOpt.WriteFile(filePath, content);
    }

    public static void AddItemToXml<T>(T info)
    {
        T[] infos = { info };
        AddItemToXml(infos);
    }

    public static void AddItemToXml<T>(T[] infos)
    {
        Type t = typeof(T);
        FieldInfo[] fields = t.GetFields();

        string insertTag = "</Table>";

        string filePath = Application.dataPath + "/Resources/Properties/" + t.Name + ".xml";
        string content = FileOpt.ReadFile(filePath);
        int insertIndex = content.IndexOf(insertTag);
        string s_pre = content.Substring(0, insertIndex);
        string s_tail = content.Substring(insertIndex, content.Length - insertIndex);

        //<Row ss:AutoFitHeight="0">
        //    <Cell><Data ss:Type="String">int</Data></Cell>
        //    <Cell><Data ss:Type="String">Vector3</Data></Cell>
        //    <Cell><Data ss:Type="String">string</Data></Cell>
        //</Row>

        string insertStr = "";
        for (int k = 0; k < infos.Length; k++)
        {
            T info = infos[k];
            insertStr += "   <Row>\n";
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fi = fields[i];
                insertStr += "    <Cell><Data ss:Type=\"String\">" + fi.GetValue(info) + "</Data></Cell>\n";
            }
            insertStr += "   </Row>\n";
        }
        content = s_pre + insertStr + s_tail;

        int fixIndex = content.IndexOf("ss:ExpandedRowCount");
        if (fixIndex >= 0)
        {
            int endFixInde = content.IndexOf(" ", fixIndex);
            content = content.Substring(0, fixIndex) + content.Substring(endFixInde, content.Length - endFixInde);
        }

        FileOpt.WriteFile(filePath, content);
    }

    /// <summary>
    /// 生成整表
    /// </summary>
    private void GeneTables()
    {
        string xmlListPath = "PropertiesList";
        string[] lists = GetPropertiesList(xmlListPath, "ForCSharp");
        foreach (string filename in lists)
        {
            Type type = Type.GetType(filename);
            Dictionary<int, object> obj1 = GeneProperties(type, filename);
            tables.Add(type, obj1);
        }
    }

    /// <summary>
    /// 根据PropertiesList文件，获取配置文件名的List
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fileType"></param>
    /// <returns></returns>
    private string[] GetPropertiesList(string filePath, string fileType)
    {
        TextAsset textAsset = ResourceManager.instance.GetResourceByPath(filePath) as TextAsset;
        string text = textAsset.text;
        text = text.Replace("\r", "");
        string[] lists = text.Split('\n');
        int index = 0;
        while (!lists[index].Contains(fileType))
            index++;

        List<string> result = new List<string>();

        for (int i = index + 1; i < lists.Length; i++)
        {
            if (String.IsNullOrEmpty(lists[i]))
                continue;
            if (lists[i].Contains("["))
            {
                break;
            }
            result.Add(lists[i]);
        }
        return result.ToArray();
    }

    /// <summary>
    /// 根据类型，生成对应的数据表
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns></returns>
    private Dictionary<int, object> GeneProperties(Type type, string filename)
    {
        string xmlText = LoadXmlText("Properties/" + filename);
        XmlDocument doc = LoadXmlDocByText(xmlText);
        return InitDicByXmlDocWithIDInt(doc, type);
    }

    /// <summary>
    /// XmlDocument解析不允许出现 xx:XX 形式（xx会被认为是命名空间）
    /// 这里使用 此 文本作为 : 的替代
    /// </summary>
    private const string replaceTag = "-rplc-";

    /// <summary>
    /// 加载xml文本
    /// </summary>
    /// <returns></returns>
    public string LoadXmlText(string path)
    {
        TextAsset textAsset = ResourceManager.instance.GetResourceByPath(path) as TextAsset;
        string text = textAsset.text.Replace(":", replaceTag);
        return text;
    }

    /// <summary>
    /// 根据文本加载XmlDocument
    /// </summary>
    /// <param name="text">xml文本数据</param>
    /// <returns></returns>
    public XmlDocument LoadXmlDocByText(string text)
    {
        XmlDocument doc = new XmlDocument();
        byte[] array = System.Text.Encoding.UTF8.GetBytes(text);

        Stream stream = new MemoryStream(array);
        doc.Load(stream);

        return doc;
    }

    /// <summary>
    /// 解析Xml
    /// </summary>
    /// <param name="doc"></param>
    public Dictionary<int, object> InitDicByXmlDocWithIDInt(XmlDocument doc, Type type)
    {
        Dictionary<int, object> result = new Dictionary<int, object>();
        string[] titles = null;                     // 标题行
        string[] s_types = null;                    // 数据类型行
        string[] descriptions = null;               // 数据描述
        object[] constuctParms = null;              // 数据封装
        int constuctKey = -1;                  // Key

        int rowIndex = 0;                           // 行Index
        int columnIndex = 0;                        // 区分当前列的类型的index
        int constuctParmsIndex = 0;                 // 数据列Index，不受columnIndex影响
        int sheetCount = 0;
        XmlNode workbook = doc.DocumentElement;     // root 节点
        foreach (XmlNode worksheet in workbook.ChildNodes)
        {
            if (sheetCount > 0)
                break;
            if (worksheet.Name != "Worksheet")
                continue;
            sheetCount++;
            XmlNode table = worksheet.FirstChild;
            rowIndex = 0;

            foreach (XmlNode row in table.ChildNodes)
            {
                if (row.Name != "Row")
                    continue;

                if (rowIndex == 0)
                {
                    if (row.ChildNodes.Count < 2)
                        continue;
                    titles = new string[row.ChildNodes.Count];
                    s_types = new string[row.ChildNodes.Count];
                    descriptions = new string[row.ChildNodes.Count];
                    constuctParms = new object[row.ChildNodes.Count];
                }

                columnIndex = 0;
                constuctParmsIndex = 0;

                foreach (XmlNode cell in row.ChildNodes)
                {
                    if (cell.Name != "Cell" || cell.FirstChild == null)
                        continue;
                    // 重置 index
                    XmlElement xe2 = (XmlElement)cell;
                    string ssIndex = xe2.GetAttribute("ss" + replaceTag + "Index");
                    if (!string.IsNullOrEmpty(ssIndex))
                    {
                        columnIndex = int.Parse(ssIndex) - 1;
                    }

                    // 获取单独的Data标签数据
                    XmlNode data = cell.FirstChild;
                    // 将节点转换为元素，便于得到节点的属性值
                    XmlElement xe = (XmlElement)data;
                    // 得到ss:Type和innerText两个值
                    string dataType = xe.GetAttribute("ss" + replaceTag + "Type");
                    string value = data.InnerText;

                    // 标题行 index赋值
                    if (rowIndex == 0) // 数据名行
                    {
                        titles[columnIndex] = value;
                    }
                    else if (rowIndex == 1) // 数据类型行
                    {
                        s_types[columnIndex] = value;
                    }
                    else if (rowIndex == 2) // 数据描述信息行
                    {
                        descriptions[columnIndex] = value;
                    }
                    else // 数据行
                    {
                        if (titles[columnIndex] == "id")
                        {
                            constuctKey = int.Parse(value);
                        }
                        //else
                        {
                            switch (s_types[columnIndex])
                            {
                                case "float":
                                    constuctParms[constuctParmsIndex] = float.Parse(value);
                                    break;
                                case "int":
                                    constuctParms[constuctParmsIndex] = int.Parse(value);
                                    break;
                                case "string":
                                    constuctParms[constuctParmsIndex] = value.Replace(replaceTag, ":");
                                    break;
                                case "bool":
                                    constuctParms[constuctParmsIndex] = bool.Parse(value);
                                    break;
                                case "Vector2":
                                    constuctParms[constuctParmsIndex] = Vector2Parse(value);
                                    break;
                                case "Vector3":
                                    constuctParms[constuctParmsIndex] = Vector3Parse(value);
                                    break;
                                case "Color":
                                    constuctParms[constuctParmsIndex] = ColorParse(value);
                                    break;
                                case "int[]":
                                    constuctParms[constuctParmsIndex] = ArrayParse<int>(value);
                                    break;
                                case "float[]":
                                    constuctParms[constuctParmsIndex] = ArrayParse<float>(value);
                                    break;
                                case "Vector3[]":
                                    constuctParms[constuctParmsIndex] = ArrayParse<Vector3>(value);
                                    break;
                                case "IntFloat":
                                    constuctParms[constuctParmsIndex] = IntFloatParse(value);
                                    break;
                                case "IntFloat[]":
                                    constuctParms[constuctParmsIndex] = ArrayParse<IntFloat>(value);
                                    break;
                                default:
                                    throw new ArgumentException("参数类型错误：" + s_types[columnIndex]);
                            }
                            constuctParmsIndex++;
                        }
                    }
                    columnIndex++;
                }

                if (rowIndex >= 3)
                {
                    //数据封装
                    // constuctParms

                    //根据类型创建对象
                    object dObj = Activator.CreateInstance(type, constuctParms);
                    result.Add(constuctKey, dObj);
                }

                rowIndex++;
            }
        }

        return result;
    }

    /// <summary>
    /// 解析Xml
    /// </summary>
    /// <param name="doc"></param>
    public Dictionary<string, object> InitDicByXmlDocWithIDString(XmlDocument doc, Type type)
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        string[] titles = null;                     // 标题行
        string[] s_types = null;                    // 数据类型行
        string[] descriptions = null;               // 数据描述
        object[] constuctParms = null;              // 数据封装
        string constuctKey = "";                  // Key

        int rowIndex = 0;                           // 行Index
        int columnIndex = 0;                        // 区分当前列的类型的index
        int constuctParmsIndex = 0;                 // 数据列Index，不受columnIndex影响

        XmlNode workbook = doc.DocumentElement;     // root 节点
        foreach (XmlNode worksheet in workbook.ChildNodes)
        {
            if (worksheet.Name != "Worksheet")
                continue;

            XmlNode table = worksheet.FirstChild;
            rowIndex = 0;

            foreach (XmlNode row in table.ChildNodes)
            {
                if (row.Name != "Row")
                    continue;

                if (rowIndex == 0)
                {
                    if (row.ChildNodes.Count < 2)
                        continue;
                    titles = new string[row.ChildNodes.Count];
                    s_types = new string[row.ChildNodes.Count];
                    descriptions = new string[row.ChildNodes.Count];
                    constuctParms = new object[row.ChildNodes.Count];
                }

                columnIndex = 0;
                constuctParmsIndex = 0;

                foreach (XmlNode cell in row.ChildNodes)
                {
                    if (cell.Name != "Cell" || cell.FirstChild == null)
                        continue;
                    // 重置 index
                    XmlElement xe2 = (XmlElement)cell;
                    string ssIndex = xe2.GetAttribute("ss" + replaceTag + "Index");
                    if (!string.IsNullOrEmpty(ssIndex))
                    {
                        columnIndex = int.Parse(ssIndex) - 1;
                    }

                    // 获取单独的Data标签数据
                    XmlNode data = cell.FirstChild;
                    // 将节点转换为元素，便于得到节点的属性值
                    XmlElement xe = (XmlElement)data;
                    // 得到ss:Type和innerText两个值
                    string dataType = xe.GetAttribute("ss" + replaceTag + "Type");
                    string value = data.InnerText;

                    // 标题行 index赋值
                    if (rowIndex == 0) // 数据名行
                    {
                        titles[columnIndex] = value;
                    }
                    else if (rowIndex == 1) // 数据类型行
                    {
                        s_types[columnIndex] = value;
                    }
                    else if (rowIndex == 2) // 数据描述信息行
                    {
                        descriptions[columnIndex] = value;
                    }
                    else // 数据行
                    {
                        if (titles[columnIndex] == "id")
                        {
                            constuctKey = value;
                        }
                        //else
                        {
                            switch (s_types[columnIndex])
                            {
                                case "float":
                                    constuctParms[constuctParmsIndex] = float.Parse(value);
                                    break;
                                case "int":
                                    constuctParms[constuctParmsIndex] = int.Parse(value);
                                    break;
                                case "string":
                                    constuctParms[constuctParmsIndex] = value.Replace(replaceTag, ":");
                                    break;
                                case "bool":
                                    constuctParms[constuctParmsIndex] = bool.Parse(value);
                                    break;
                                case "Vector2":
                                    constuctParms[constuctParmsIndex] = Vector2Parse(value);
                                    break;
                                case "Vector3":
                                    constuctParms[constuctParmsIndex] = Vector3Parse(value);
                                    break;
                                case "Color":
                                    constuctParms[constuctParmsIndex] = ColorParse(value);
                                    break;
                                case "int[]":
                                    constuctParms[constuctParmsIndex] = ArrayParse<int>(value);
                                    break;
                                case "float[]":
                                    constuctParms[constuctParmsIndex] = ArrayParse<float>(value);
                                    break;
                                case "Vector3[]":
                                    constuctParms[constuctParmsIndex] = ArrayParse<Vector3>(value);
                                    break;
                                case "IntFloat":
                                    constuctParms[constuctParmsIndex] = IntFloatParse(value);
                                    break;
                                case "IntFloat[]":
                                    constuctParms[constuctParmsIndex] = ArrayParse<IntFloat>(value);
                                    break;
                                default:
                                    throw new ArgumentException("参数类型错误：" + s_types[columnIndex]);
                            }
                            constuctParmsIndex++;
                        }
                    }
                    columnIndex++;
                }

                if (rowIndex >= 3)
                {
                    //数据封装
                    // constuctParms

                    //根据类型创建对象
                    object dObj = Activator.CreateInstance(type, constuctParms);
                    result.Add(constuctKey, dObj);
                }

                rowIndex++;
            }
        }

        return result;
    }

    private Vector2 Vector2Parse(string sv2)
    {
        sv2 = sv2.Trim('(');
        sv2 = sv2.Trim(')');
        Vector3 vec = new Vector3();
        string[] sv2s = sv2.Split(',');
        vec.x = float.Parse(sv2s[0]);
        vec.y = float.Parse(sv2s[1]);
        return vec;
    }

    private Vector3 Vector3Parse(string sv3)
    {
        sv3 = sv3.Trim('(');
        sv3 = sv3.Trim(')');
        Vector3 vec = new Vector3();
        string[] sv3s = sv3.Split(',');
        vec.x = float.Parse(sv3s[0]);
        vec.y = float.Parse(sv3s[1]);
        vec.z = float.Parse(sv3s[2]);
        return vec;
    }

    private Color ColorParse(string color)
    {
        color = color.Trim('#');
        float max = 255;
        Color c = new Color();
        if (color.Length == 8)
        {
            c.a = Int32.Parse(color.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) / max;
        }
        else if (color.Length == 6)
        {
            c.a = 1;
        }
        else
            Debug.LogError("Font Color Error :" + color);
        c.r = Int32.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / max;
        c.g = Int32.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / max;
        c.b = Int32.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / max;
        return c;
    }

    private IntFloat IntFloatParse(string if2)
    {
        IntFloat intFloat = new IntFloat();
        if2 = if2.Trim('(', ')');
        string[] farray = if2.Split(',');
        if (farray.Length == 2)
        {
            intFloat.intValue = int.Parse(farray[0]);
            intFloat.floatValue = float.Parse(farray[1]);
        }
        return intFloat;
    }

    private T[] ArrayParse<T>(string val)
    {
        bool isClass = false;
        if (val.Contains("("))
            isClass = true;
        val = val.Replace("[", "");
        val = val.Replace("]", "");
        val = val.Replace("),", ")|");
        val = val.Replace("(", "");
        val = val.Replace(")", "");
        val = val.Trim(' ');
        string[] arrays;
        if (isClass)
            arrays = val.Split('|');
        else
            arrays = val.Split(',');
        T[] tArray = new T[arrays.Length];

        string type = typeof(T).Name;

        for (int i = 0; i < arrays.Length; i++)
        {
            switch (type)
            {
                case "Single":
                    tArray[i] = (T)(object)float.Parse(arrays[i]);
                    break;
                case "Int32":
                    tArray[i] = (T)(object)int.Parse(arrays[i]);
                    break;
                case "String":
                    tArray[i] = (T)(object)arrays[i];
                    break;
                case "Boolean":
                    tArray[i] = (T)(object)bool.Parse(arrays[i]);
                    break;
                case "Vector2":
                    tArray[i] = (T)(object)Vector2Parse(arrays[i]);
                    break;
                case "Vector3":
                    tArray[i] = (T)(object)Vector3Parse(arrays[i]);
                    break;
                case "IntFloat":
                    tArray[i] = (T)(object)IntFloatParse(arrays[i]);
                    break;
                default:
                    throw new ArgumentException("数组中，参数类型错误：" + arrays[i]);
            }
        }
        return tArray;
    }
}

class IntFloat
{
    public int intValue;
    public float floatValue;

    public IntFloat()
    { }
    public IntFloat(int a, float b)
    {
        intValue = a;
        floatValue = b;
    }
}
