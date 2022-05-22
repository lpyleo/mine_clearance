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
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
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
            if(this.textBox1.Text==""||this.textBox2.Text==""||this.textBox3.Text=="")
            {
                MessageBox.Show("用户名或密码不能为空！");
            }
            else
            {
                if (mc.HasName())
                {
                    MessageBox.Show("用户名已存在！");
                }
                else
                {
                    if (!(this.textBox2.Text == this.textBox3.Text))
                    {
                        MessageBox.Show("两次密码输入不一致！");
                    }
                    else
                    {
                        if(mc.Register())
                        {
                            //用一个DialogResult存放点击弹窗的结果
                            DialogResult dialogResult = MessageBox.Show("注册成功，请重新登录！", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (dialogResult == DialogResult.Yes)
                            {
                                Form5 f5 = new Form5();
                                this.Close();
                                f5.Show();
                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                this.textBox1.Text = "";
                                this.textBox2.Text = "";
                                this.textBox3.Text = "";
                            }
                        }
                        else
                        {
                            MessageBox.Show("注册失败！");
                        }
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form5 f5 = new Form5();
            this.Close();
            f5.Show();
        }

    }
}
