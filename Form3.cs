using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mine_clearance
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            Read();
        }

        public void Read()//读取数据库中的内容
        {
            string mr1 = "名字\t成绩\t总雷数\r\n", mystr1 = "";
            int i = 0;
            MysqlConnection mc = new MysqlConnection();
            mc.ReadRecoeds();
            while (!string.IsNullOrEmpty(mc.n[i]))
            {
                mystr1 += mc.n[i] + "\t";
                mystr1 += mc.s[i] + "\t";
                mystr1 += mc.m[i] + "\r\n";
                i++;
            }
            textBox1.Text = mr1 + mystr1;
        }
        private void button1_Click(object sender, EventArgs e)//关闭
        {
            this.Close();
        }

        private void reset_Click(object sender, EventArgs e)//重置按钮
        {
            MysqlConnection mc = new MysqlConnection();
            mc.name = Form5.name;
            if(mc.ClearRecoeds())
            {
                MessageBox.Show("重置成功！");
            }
            else
            {
                MessageBox.Show("重置失败！");
            }
            Read();
        }

        
    }
}
