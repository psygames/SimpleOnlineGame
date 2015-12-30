using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

class FileOpt
{
    public static void WriteFile(string path, string content, bool isAppend = false)
    {
        WriteFile(path, content, Encoding.UTF8, isAppend);
    }

    public static void WriteFile(string path, string content, Encoding encode, bool isAppend = false)
    {
        FileStream fs = new FileStream(path, isAppend ? FileMode.Append : FileMode.Create);
        StreamWriter sw = new StreamWriter(fs, encode);
        sw.Write(content);
        sw.Flush();
        sw.Close();
        fs.Close();
    }
    public static string ReadFile(string path)
    {
        return ReadFile(path, Encoding.UTF8);
    }

    public static string ReadFile(string path,Encoding encoding)
    {
        StreamReader sr = new StreamReader(path, encoding);
        string content = sr.ReadToEnd();
        sr.Close();
        return content;
    }
}
