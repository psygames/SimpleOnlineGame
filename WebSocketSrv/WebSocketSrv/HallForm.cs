using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebSocketSharp.Server;

namespace ShitMan
{
    public partial class HallForm : Form
    {
        public HallForm()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(OnFormClosiong);
            HallManager.Instance.SetHallForm(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SessionManager.Instance.Start();
            HallManager.Instance.Start();
            this.button1.Enabled = false;
        }

        private void OnFormClosiong(object sender, FormClosingEventArgs e)
        {
            SessionManager.Instance.Start();
        }

        private delegate void DelegatePrintText(string str, System.Drawing.Color color);
        public void PrintText(string str, System.Drawing.Color color)
        {
            if (richTextBox1.InvokeRequired)
            {
                DelegatePrintText du = new DelegatePrintText(PrintText);
                richTextBox1.Invoke(du, new object[] { str, color });
            }
            else
            {
                RtbAppend(str, color);
            }
        }

        public void PrintText(string str)
        {
            PrintText(str + "\n", System.Drawing.Color.Black);
        }

        private void RtbAppend(string strInput, System.Drawing.Color fontColor)
        {
            int p1 = richTextBox1.TextLength;  //取出未添加时的字符串长度。 
            richTextBox1.AppendText(strInput);  //保留每行的所有颜色。 //  rtb.Text += strInput + "/n";  //添加时，仅当前行有颜色。 
            int p2 = strInput.Length;  //取出要添加的文本的长度 
            richTextBox1.Select(p1, p2);        //选中要添加的文本 
            richTextBox1.SelectionColor = fontColor;  //设置要添加的文本的字体色 
            richTextBox1.Focus();
        }

        private void SendComp()
        {

        }
    }
}
