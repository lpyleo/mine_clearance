using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mine_clearance
{
    public partial class Form5 : Form
    {
        public static string name;
        public static string password;
        public Form5()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form4 f4 = new Form4();
            this.Close();
            f4.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MysqlConnection mc = new MysqlConnection();
            mc.name = this.textBox1.Text;
            mc.password = this.textBox2.Text;
            if(this.textBox1.Text==""||this.textBox2.Text=="")
            {
                MessageBox.Show("用户名或密码不能为空！");
            }
            else
            {
                if (mc.Login())
                {
                    name = this.textBox1.Text;
                    password = this.textBox2.Text;
                    Form1 f1 = new Form1();
                    this.Close();
                    f1.Show();
                }
                else
                {
                    MessageBox.Show("用户名或密码错误，登录失败！");
                }
            }
        }

    }
}
