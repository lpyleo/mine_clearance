using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mine_clearance
{
    /// <summary>
    /// 实现与数据库的连接
    /// </summary>
    public class MysqlConnection
    {
        public string name { get; set; }         //用户名
        public string password { get; set; }     //密码
        public int score { get; set; }           //得分
        public int mine_num { get; set; }        //雷数
        public string[] n = new string[10000];
        public string[] s = new string[10000];
        public string[] m = new string[10000];

        private MySqlConnection conn = null;            //数据库连接对象
        /// <summary>
        /// 构造函数
        /// </summary>
        public MysqlConnection()
        {
            String connetStr = "server=127.0.0.1;port=3306;user=root;password=123456; database=mine_clearance";
            if(conn==null)
            {
                conn = new MySqlConnection(connetStr);
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                Console.WriteLine("数据库连接成功");
            }
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~MysqlConnection()
        {
            conn.Close();
        }
        /// <summary>
        /// 验证登录函数
        /// </summary>
        /// <returns>true/false</returns>
        public Boolean Login()
        {
            String sql = "select *  from users where name='"+name+"'";
            MySqlCommand msc = new MySqlCommand(sql,conn);
            MySqlDataReader mdr = msc.ExecuteReader();
            if(mdr.Read())
            {
                if(password == mdr.GetString("password"))
                {
                    mdr.Close();
                    return true;
                }
                else
                {
                    mdr.Close();
                    return false;
                }
            }
            else
            {
                mdr.Close();
                return false;
            }
        }
        /// <summary>
        /// 验证注册用户名是否存在
        /// </summary>
        /// <returns>true/false</returns>
        public Boolean HasName()
        {
            String sql = "select *  from users where name='" + name + "'";
            MySqlCommand msc = new MySqlCommand(sql, conn);
            MySqlDataReader mdr = msc.ExecuteReader();
            if (mdr.Read())
            {
                mdr.Close();
                return true;
            }
            else
            {
                mdr.Close();
                return false;
            }
        }
        /// <summary>
        /// 注册函数
        /// </summary>
        /// <returns>true/false</returns>
        public Boolean Register()
        {
            String sql = "insert into users value('" + name + "','" + password + "')";
            using (MySqlCommand msc = new MySqlCommand(sql, conn))
            {
                
                int ex = msc.ExecuteNonQuery();
                if (ex > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 将成绩记录到数据库
        /// </summary>
        /// <returns>true/false</returns>
        public Boolean Record()
        {
            String sql = "insert into record value(null , '" + name + "','" + score + "','" + mine_num + "')";
            using (MySqlCommand msc = new MySqlCommand(sql, conn))
            {

                int ex = msc.ExecuteNonQuery();
                if (ex > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 将成绩记录从数据库中读取出来
        /// </summary>
        /// <returns></returns>
        public void ReadRecoeds()
        {
            int i = 0;
            String sql = "select *  from record";
            MySqlCommand msc = new MySqlCommand(sql, conn);
            MySqlDataReader mdr = msc.ExecuteReader();
            while (mdr.Read())
            {
                n[i] = Convert.ToString(mdr[1]);
                s[i] = Convert.ToString(mdr[2]);
                m[i] = Convert.ToString(mdr[3]);
                i++;
            }
            mdr.Close();
        }
        /// <summary>
        /// 清空记录表中该用户的记录
        /// </summary>
        /// <returns>true/false</returns>
        public bool ClearRecoeds()
        {
            int i = 0;
            String sql = "delete from `record` where user_name='"+name+"';";
            MySqlCommand msc = new MySqlCommand(sql, conn);
            int r = msc.ExecuteNonQuery();
            if (r>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns>true/false</returns>
        public bool ChangePassword()
        {
            int i = 0;
            String sql = "update users set password='"+password+"' where name='"+name+"';";
            MySqlCommand msc = new MySqlCommand(sql, conn);
            int r = msc.ExecuteNonQuery();
            if (r > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 注销账户
        /// </summary>
        /// <returns>true/false</returns>
        public bool DeleteUser()
        {
            int i = 0;
            String sql = "delete from users where name='"+name+"';";
            MySqlCommand msc = new MySqlCommand(sql, conn);
            int r = msc.ExecuteNonQuery();
            if (r > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
