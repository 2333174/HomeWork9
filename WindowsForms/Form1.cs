using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sourse;
using Form2;
using Form3;
using Form4;
using Form5;
using Error;
using MySql.Data.MySqlClient;

namespace WindowsForms
{
    public partial class Form1 : Form
    {
        int CHECKED = 0;
        OrderService b = new OrderService();
        public Form1()
        {
            InitializeComponent();
            


        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2.Form1 form1 = new Form2.Form1();
            form1.ShowDialog();
            int t = 1;
            while (t == 1)
            {
                t = 0;
                if (form1.DialogResult == DialogResult.OK)
                {
                    //Order m = form1.returnOrder();
                    //b.addOrder(form1.returnOrder());
                    foreach (Order c in b.orderList)
                    {
                        if (c.orderNum == form1.textBox1.Text) { t = 1; break; }
                    }
                    if (t == 0)
                    {
                        form1.Dispose();
                        Order m = new Order(form1.textBox1.Text, form1.textBox2.Text, form1.textBox3.Text, form1.textBox7.Text, form1.textBox4.Text, form1.textBox6.Text, form1.textBox5.Text);
                        b.addOrder(m);
                        this.orderServiceBindingSource1.DataSource = new BindingList<Order>(b.orderList);
                    }
                    else
                    {
                        Error.Form1 formn = new Error.Form1();
                        formn.label1.Text = "订单号重复，请重新输入";
                        formn.ShowDialog();
                        if (formn.DialogResult == DialogResult.OK)
                        {
                            form1.textBox1.Text = "";
                            form1.ShowDialog();
                        }
                    }
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3.Form1 form2 = new Form3.Form1();
            form2.ShowDialog();
            if (form2.DialogResult == DialogResult.OK)
            {
                this.orderServiceBindingSource1.DataSource = b.searchOrderbyLinq(form2.textBox2.Text,int.Parse(form2.textBox1.Text));
                form2.Dispose();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form4.Form1 form3 = new Form4.Form1();
            form3.ShowDialog();
            if (form3.DialogResult == DialogResult.OK)
            {
                b.deleteOrder(int.Parse(form3.textBox1.Text));
                this.orderServiceBindingSource1.DataSource = new BindingList<Order>(b.orderList);
                form3.Dispose();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form5.Form1 form4 = new Form5.Form1();
            form4.ShowDialog();
            if (form4.DialogResult == DialogResult.OK)
            {
                b.changeOrder(int.Parse(form4.textBox1.Text),form4.textBox3.Text,int.Parse(form4.textBox2.Text));
                this.orderServiceBindingSource1.DataSource = new BindingList<Order>(b.orderList);
                form4.Dispose();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (CHECKED == 1) b.Export();
            else b.toHTML();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            CHECKED = 1;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            CHECKED = 2;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string connectStr = "server=127.0.0.1;port=3306;user=root;password="+this.textBox1.Text+";";
            MySqlConnection conn = new MySqlConnection(connectStr); 
            conn.Open();//跟数据库建立连接，并打开连接
            string createBase = "CREATE DATABASE IF NOT EXISTS t1";
            MySqlCommand cmd = new MySqlCommand(createBase, conn);
            cmd.ExecuteNonQuery();
            string useBase = "USE t1";
            cmd = new MySqlCommand(useBase, conn);
            int m=cmd.ExecuteNonQuery();
            string createStatement = "CREATE TABLE IF NOT EXISTS dataTable(OrderNum VARCHAR(20),OrderName VARCHAR(20),OrderClient VARCHAR(20),ClientPhone VARCHAR(20));";
            cmd = new MySqlCommand(createStatement, conn);
            cmd.ExecuteNonQuery();
            foreach (Order c in b.orderList)
            {
                string sql = "insert into dataTable(OrderNum,OrderName,OrderClient,ClientPhone) value('"+c.orderNum+"','"+c.orderName+"','" + c.orderClient + "','"+c.ClientPhone+"')";//DateTime.Now调用时间
                cmd = new MySqlCommand(sql, conn);
                int result = cmd.ExecuteNonQuery();            //插入 删除 返回值是数据库中受影响的数据的行数
            }
            Console.WriteLine("数据库创建完成，请自行查看");
            conn.Close();
        }
    }
}
