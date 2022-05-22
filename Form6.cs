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
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MysqlConnection mc = new MysqlConnection();
            mc.name = Form5.name;
            mc.password = Form5.password;
            if (this.textBox1.Text == "" || this.textBox2.Text == "" || this.textBox3.Text == "")
            {
                MessageBox.Show("输入不能为空！");
            }
            else
            {
                mc.password = this.textBox1.Text;
                if (!mc.Login())
                {
                    MessageBox.Show("原密码错误！");
                }
                else
                {
                    if(!(this.textBox2.Text == this.textBox3.Text))
                    {
                        MessageBox.Show("两次密码输入不一致！");
                    }
                    else
                    {
                        mc.password = this.textBox2.Text;
                        if (mc.ChangePassword())
                        {
                            MessageBox.Show("修改密码成功！");
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("修改密码失败！");
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            DialogResult = DialogResult.Cancel;
        }
    }
}
