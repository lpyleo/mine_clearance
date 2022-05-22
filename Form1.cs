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
using System.Media;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace mine_clearance
{

    public partial class Form1 : Form
    {
        private Button[,] Mines;   //定义一个 二维动态数组 用于显示雷区
        private int XNum = 10;   //初始化累的列数（即为：初级时的行列数）
        private int YNum = 10;   //初始化雷的行数
        public static int zdyXNum;   //用于自定义中的列数
        public static int zdyYNum;   //用于自定义中的行数
        public static int MineNum = 10;   //初始化雷的总数
        public static int zdyMineNum;   //用于记录自定义中的雷数
        private int[,] Turn;   //用二维数组赋值：-1 表示这个位置已经翻开；0 表示这个位置没有翻开；1 表示这个位置插上红旗
        public static int CostTime = 0;   //计量所用的时间
        private Boolean music_click = false;   //音乐关闭/开启点击
        private int RestMine = 0;   //用于改变等级时载入剩余雷数
        private int MineWidth = 20;   // 控制雷块的大小

        public Form1()
        {
            Play();//当窗体打开时播放背景音乐
            InitializeComponent();
            // GamesBegin();
        }

        //载入背景音乐
        public static uint SND_ASYNC = 0x0001;
        public static uint SND_FILENAME = 0x00020000;
        [DllImport("winmm.dll")]
        public static extern uint mciSendString(string lpstrCommand, string lpstrReturnString, uint uReturnLength, uint hWndCallback);
        public void Play()
        {
            mciSendString(@"close temp_alias", null, 0, 0);
            mciSendString(@"open ""D:/C#/课程设计/mine_clearance/Resources/backgroundmusic.mp3"" alias temp_alias", null, 0, 0);
            mciSendString("play temp_alias repeat", null, 0, 0);
        }

        //背景音乐关闭
        public void Stop()
        {
            mciSendString(@"close temp_alias", null, 0, 0);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            stalls = new Stalls();//建立等级实例
            stalls.Onshift += new OnshiftHande(setLevel);
            Load_Mine();   //初始化雷区
            GameInit();    //游戏初始化
            timer1.Enabled = true;   //开启时钟计时
        }


        private void button2_Click(object sender, EventArgs e)
        {
            show();   //将地图中所有雷标识出来
            button1.Image = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/failed-smile.png");
            timer1.Enabled = false;//停止计时
            MessageBox.Show("扫雷失败！");
            Endall();
        }
        /// <summary>
        /// 将地图中所有雷标识出来
        /// </summary>
        private void show()//将地图中所有雷标识出来
        {
            for (int i = 0; i < XNum; i++)
                for (int j = 0; j < YNum; j++)
                    if (Convert.ToInt16(Mines[i, j].Tag) == 1)
                    {
                        //==1时，代表这个位置是地雷
                        Mines[i, j].BackgroundImage = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/mine.bmp");
                    }
        }

        private void 新游戏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            readytostart(XNum, YNum, MineNum);
        }
        /// <summary>
        /// 做好游戏开始准备
        /// </summary>
        private void readytostart(int xnum, int ynum, int minenum)
        {
            DelAllMines();   //删除所有的雷区控件
            XNum = xnum;   //定义雷区的列数
            YNum = ynum;   //定义雷区的行数
            MineNum = minenum;   //定义总雷数
            button1.Image = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/face.bmp");  //开始时，让button1按钮的Image设为face
            cost_time.Text = CostTime.ToString();   //用于在开始界面显示所用的时间
            GamesBegin();   //开始游戏
            GameInit();//游戏初始化
            timer1.Enabled = true;   //触发计时器
        }
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();   //单击退出
        }

        private void 自定义ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.Image = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/face.bmp");   //开始时，让button1按钮的Image设为face
            Form zdy = new Form2();   //通过定义调用新的Form窗体
            zdy.ShowDialog();   //显示自定义窗体
            if (zdy.DialogResult == DialogResult.OK)   //如果在自定义窗体中单击 确定
                zdyGames();   //开始自定义游戏
        }

        private void zdyGames()   //自定义游戏
        {
            readytostart(zdyXNum, zdyYNum, zdyMineNum);
        }

        private void 关于扫雷ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("游戏说明：\n\n  *登录游戏后，自动开始计时;\n  *可以通过按钮控制背景音乐的开关;\n\n注意:\n\n  * 鼠标左键单击便可翻开方块,若翻开的是地雷，您便输掉游戏;\n  * 如果方块上出现数字，则表示在它周围的八个方块中共有多少颗地雷，四条边上的表示在它四周五个方块中共有多少颗地雷，边角上的表示在它四周三个方块中共有多少颗地雷;\n  * 若要标记您认为可能有地雷的方块，请用鼠标右键单击它;\n  *游戏区包括菜单、雷区、地雷计数器、计时器和开始按钮,背景音乐播放/暂停按钮，请按游戏规则游戏！\n *本游戏的英雄榜是记录游戏者的成绩使用。并设置了重置功能，方便玩家查看成绩。\n\n   谢谢支持！", "关于扫雷：");   //游戏的使用说明，简介游戏使用问题
        }


        private void 关于版权ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("\n\n本游戏用于C#课程设计！ \n\n    ", "关于版权：");   //显示游戏版本及版权说明

        }
        private void 英雄榜ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form herof = new Form3();   //定义新的窗体，通过关联调用
            herof.Show();   //显示扫雷英雄棒的窗体
        }
        /// <summary>
        /// 判断是否已出雷区
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        /// <returns></returns>
        private bool IsInMineArea(int row, int col)
        {
            return (row >= 0 && row < XNum && col >= 0 && col < YNum);   //返回true or false
        }
        /// <summary>
        /// 删除所有的雷区
        /// </summary>
        private void DelAllMines()
        {
            for (int i = 0; i < XNum; i++)
                for (int j = 0; j < YNum; j++)   //二维数组逐个删除
                {
                    this.Controls.Remove(Mines[i, j]);   //删除所指雷区
                }
        }
        /// <summary>
        /// 用于开始游戏，主要是定义雷区
        /// </summary>
        private void GamesBegin()
        {

            SoundPlayer player = new SoundPlayer("D:/C#/课程设计/mine_clearance/Resources/expand.wav");//新游戏开始音效
            player.Load();
            player.Play();
            Turn = new int[XNum, YNum];   //定义新的二维数组
            Mines = new Button[XNum, YNum];   //定义按钮
            for (int x = 0; x < XNum; x++)
                for (int y = 0; y < YNum; y++)   //通过二维数组，逐个定义初始化button按钮
                {
                    Mines[x, y] = new Button();
                    this.Controls.Add(Mines[x, y]);   //增加新按钮
                    Mines[x, y].Left = 10 + MineWidth * x;   //定义雷区开始在Form窗体中的左边界
                    Mines[x, y].Top = 80 + MineWidth * y;   //定义雷区开始在Form窗体中的上边界
                    Mines[x, y].Width = MineWidth;   //定义雷块的宽度
                    Mines[x, y].Height = MineWidth;   //定义雷块的高度
                    Mines[x, y].Font = new Font("宋体", 10.5F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(134)));   //定义字体
                    Mines[x, y].BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;   //定义backgroundimageLayout 平铺
                    Mines[x, y].Name = "Mines" + (x + y * XNum).ToString();   //定义雷区的名字
                    Mines[x, y].MouseUp += new MouseEventHandler(bt_MouseUp);   //定义单击事件
                    Mines[x, y].Visible = true;   //控制Mines按钮的可见
                }
            detform();
        }
        /// <summary>
        /// 初始化雷区
        /// </summary>
        private void Load_Mine()
        {
            Turn = new int[XNum, YNum];
            Mines = new Button[XNum, YNum];
            for (int x = 0; x < XNum; x++)
                for (int y = 0; y < YNum; y++)
                {
                    Mines[x, y] = new Button();
                    this.Controls.Add(Mines[x, y]);
                    Mines[x, y].Left = 10 + MineWidth * x;
                    Mines[x, y].Top = 80 + MineWidth * y;
                    Mines[x, y].Width = MineWidth;
                    Mines[x, y].Height = MineWidth;
                    Mines[x, y].Font = new Font("宋体", 10.5F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(134)));
                    Mines[x, y].BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                    Mines[x, y].Name = "Mines" + (x + y * XNum).ToString();
                    Mines[x, y].MouseUp += new MouseEventHandler(bt_MouseUp);
                    Mines[x, y].Visible = true;
                }
        }
        /// <summary>
        /// 决定form1的整体框架
        /// </summary>
        private void detform()
        {
            button1.Location = new Point(-10 + XNum * MineWidth / 2, -1);   //控制form1中的button1按钮开始的位置
            button2.Location = new Point(-25 + XNum * MineWidth / 2, 85 + YNum * MineWidth);   //控制form1中的button2按钮开始的位置
            button3.Location = new Point(-15 + XNum * MineWidth, -1);//控制form1中button3开始的位置
            panel1.Size = new Size(30 + MineWidth * XNum, 35);   //控制panel1的大小
            Form1.ActiveForm.Width = 35 + MineWidth * XNum;   //用于控制form窗体的宽度
            Form1.ActiveForm.Height = 160 + MineWidth * YNum;   //用于控制form窗体的高度
        }
        /// <summary>
        /// 游戏初始化
        /// </summary>
        private void GameInit()
        {
            RestMine = MineNum;
            // cost_time.Text = RestMine.ToString();
            for (int x = 0; x < XNum; x++)
                for (int y = 0; y < YNum; y++)
                {
                    Mines[x, y].Text = "";
                    Mines[x, y].Visible = true;
                    Mines[x, y].Enabled = true;
                    Mines[x, y].Tag = null;
                    Mines[x, y].BackgroundImage = null;
                    Turn[x, y] = 0;        //刚开始都未插旗、未表示为雷
                }
            LayMines();
        }

        private void timer1_Tick(object sender, EventArgs e)   //定义计时器
        {
            CostTime++;
        }

        private void bt_MouseUp(object sender, MouseEventArgs e)    //这里处理事件方法
        {
            //Load_Mine();
            String btName;
            Button bClick = (Button)sender;   //将被击的按钮赋给定义的bClick变量
            btName = bClick.Name;   //获取按钮的Name
            int n = Convert.ToInt16(btName.Substring(5));
            int x = n % XNum;
            int y = n / XNum;
            //通过按钮Name属性来判断是哪个Button被点击，并执行相应的操作
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (Convert.ToInt16(Mines[x, y].Tag) != 1)
                    {
                        detpicture(GetAroundNum(x, y), x, y);
                        Mines[x, y].Enabled = false;
                        ExpandMines(x, y);
                        if (Victory())
                        {
                            button1.Image = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/success-smile.png");
                            SoundPlayer play = new SoundPlayer("D:/C#/课程设计/mine_clearance/Resources/victory.wav");
                            play.Load();
                            play.Play();
                            // 判断是否胜利，是则将地图中所有雷标识出来
                            show();
                            timer1.Enabled = false;//停止计时
                            MessageBox.Show("扫雷成功！");
                            string name = Form5.name;
                            int score = CostTime;
                            int mine_num = MineNum;
                            MysqlConnection mc = new MysqlConnection();
                            mc.name = name;
                            mc.score = score;
                            mc.mine_num = mine_num;
                            if (mc.Record())
                            {
                                MessageBox.Show("恭喜荣登英雄榜！");
                            }
                        }
                    }
                    else
                    {
                        Mines[x, y].BackgroundImage = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/mine.bmp");
                        button1.Image = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/failed-smile.png");
                        SoundPlayer player = new SoundPlayer("D:/C#/课程设计/mine_clearance/Resources/failure.wav");
                        player.Load();
                        player.Play();
                        timer1.Enabled = false;//停止计时
                        MessageBox.Show("扫雷失败！");
                        show();
                        Endall();
                    }
                    break;
                case MouseButtons.Right:
                    Mines[x, y].BackgroundImage = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/flag.bmp");
                    if (Turn[x, y] == 1)//表示这个位置插上红旗
                    {
                        Turn[x, y] = 0;//取消红旗,表示这个位置没有翻开
                        RestMine++;
                        Mines[x, y].BackgroundImage = null;
                    }
                    else
                    {
                        Turn[x, y] = 1;//表示这个位置插上红旗
                        RestMine--;
                    }
                    //cost_time.Text = RestMine.ToString();
                    if (Victory())
                    {
                        button1.Image = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/success-smile.png");
                        SoundPlayer play = new SoundPlayer("D:/C#/课程设计/mine_clearance/Resources/victory.wav");
                        timer1.Enabled = false;
                        play.Load();
                        play.Play();
                        MessageBox.Show("扫雷成功！");
                        string name = Form5.name;
                        int score = CostTime;
                        int mine_num = MineNum;
                        MysqlConnection mc = new MysqlConnection();
                        mc.name = name;
                        mc.score = score;
                        mc.mine_num = mine_num;
                        if(mc.Record())
                        {
                            MessageBox.Show("恭喜荣登英雄榜！");
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// 雷区按键不可用
        /// </summary>
        private void Endall()
        {
            for (int i = 0; i < XNum; i++)
                for (int j = 0; j < YNum; j++)
                {
                    Mines[i, j].Enabled = false;
                }
        }
        /// <summary>
        /// 布雷
        /// </summary>
        private void LayMines()
        {
            int x, y;
            Random s = new Random();
            for (int i = 0; i < MineNum;)
            {
                //取随机数
                x = s.Next(XNum);     //取随机数，返回一个小于所指定最大值的非负随机数
                y = s.Next(YNum);
                if (Convert.ToInt16(Mines[x, y].Tag) != 1)
                {
                    //==1时，代表这个位置是地雷
                    Mines[x, y].Tag = 1;//修改属性为雷
                    i++;
                }
            }
            CostTime = 0;
            cost_time.Text = CostTime.ToString();
        }

        private void detpicture(int n, int i, int j)   //用于调用不同的图片显示所单击按钮周围所剩的雷数
        {
            switch (n)
            {
                case 1:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/1.png");
                        break;
                    }
                case 2:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/2.png");
                        break;
                    }
                case 3:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/3.png");
                        break;
                    }
                case 4:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/4.png");
                        break;
                    }
                case 5:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/5.png");
                        break;
                    }
                case 6:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/6.png");
                        break;
                    }
                case 7:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/7.png");
                        break;
                    }
                case 8:
                    {
                        Mines[i, j].BackgroundImage = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/8.png");
                        break;
                    }
            }
        }

        private int GetAroundNum(int row, int col)   //用于获取所单击按钮周围8个雷块中所剩的雷数
        {
            int i, j;
            int around = 0;   //定义所生的雷数，开始为0
            int minRow = (row == 0) ? 0 : row - 1;
            int maxRow = row + 2;
            int minCol = (col == 0) ? 0 : col - 1;
            int maxCol = col + 2;
            for (i = minRow; i < maxRow; i++)
            {
                for (j = minCol; j < maxCol; j++)     //[row,col]处没有雷
                {
                    if (!IsInMineArea(i, j))   //判断是否在扫雷区域
                        continue;
                    if (Convert.ToInt16(Mines[i, j].Tag) == 1) around++;
                }
            }
            return around;   //返回所剩雷数
        }
        /// <summary>
        /// 递归
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        private void ExpandMines(int row, int col)
        {
            int i, j;
            int minRow = (row == 0) ? 0 : row - 1;
            int maxRow = row + 2;
            int minCol = (col == 0) ? 0 : col - 1;
            int maxCol = col + 2;
            int around = GetAroundNum(row, col);   //对周围一个雷都没有的空白区域拓展
            if (around == 0)
            {
                Mines[row, col].Enabled = false;
                for (i = minRow; i < maxRow; i++)
                {
                    for (j = minCol; j < maxCol; j++)
                    {
                        //对于周围可以拓展的区域进行的规拓展			
                        if (!IsInMineArea(i, j)) continue;
                        if (!(i == row && j == col) && Mines[i, j].Enabled != false)
                        {
                            ExpandMines(i, j);
                        }
                        Mines[i, j].Enabled = false;   //周围无雷的区域按钮无效
                        detpicture(GetAroundNum(i, j), i, j);
                    }
                }
            }
        }
        /// <summary>
        /// 检测是否胜利
        /// </summary>
        /// <returns>true/false</returns>
        private bool Victory()
        {
            for (int i = 0; i < XNum; i++)
                for (int j = 0; j < YNum; j++)
                {
                    //没翻开且未标示,则未成功
                    if (Mines[i, j].Enabled == true && Turn[i, j] != 1)
                        return false;
                    //不是雷却误标示为雷,则也未成功
                    if (Convert.ToInt16(Mines[i, j].Tag) != 1 && Turn[i, j] == 1)
                        return false;
                }
            return true;
        }


        private void timer1_Tick_1(object sender, EventArgs e)
        {
            CostTime += 1;
            this.cost_time.Text = CostTime.ToString();
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!music_click)
            {
                button3.Image = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/up-music.png");
                button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Tile;
                Stop();
            }
            else
            {
                button3.Image = Image.FromFile("D:/C#/课程设计/mine_clearance/Resources/down-music.png");
                button3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Tile;
                Play();
            }
            music_click = !music_click;
        }

        private Stalls stalls;//等级信息
        private void button_click(object sender, EventArgs e)
        {
            LevelArgs levelargs = new LevelArgs();//建立shiftArgs实例，并传入参数
            levelargs.Gear = Convert.ToInt32(((ToolStripMenuItem)sender).Tag);
            stalls.Shift(sender, levelargs);//引发事件
        }
        /// <summary>
        /// 等级处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setLevel(object sender, LevelArgs e)
        {
            switch (e.Gear)
            {
                case 0:
                    readytostart(10, 10, 10);//低级
                    break;
                case 1:
                    readytostart(18, 18, 30);//中级
                    break;
                case 2:
                    readytostart(25, 25, 90);//高级
                    break;
                case 3:
                    readytostart(XNum, YNum, MineNum);//新游戏,笑脸
                    break;
            }
        }

        private void 修改密码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form6 f6 = new Form6();
            f6.Show();
        }

        private void 注销账户ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("确定要注销账户吗？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dialogResult == DialogResult.Yes)
            {
                MysqlConnection mc = new MysqlConnection();
                mc.name = Form5.name;
                if(mc.DeleteUser())
                {
                    MessageBox.Show("注销账户成功！");
                    Form5 f5 = new Form5();
                    this.Close();
                    f5.Show();
                }
                else
                {
                    MessageBox.Show("注销账户失败！");
                }    
            }
            else if (dialogResult == DialogResult.No)
            {
                MessageBox.Show("注销账户失败！");
            }
        }
    }
}
