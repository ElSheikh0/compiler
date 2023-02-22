using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp2;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {

        //public string[] rsrvdwrds = { "read", "write", "repeat", "until", "if", "elseif", "else", "then", "return", "endl", "while", "program", "main" };
        public List<String> dtatyps = new List<String>();
        public List<String> rsrvdwrds = new List<String>();
        public List<String> error = new List<String>();
        public List<String> comment = new List<String>();





        public void CheckKeyword(string word, Color color, int startIndex)
        {
            if (this.richTextBox1.Text.Contains(word))
            {
                int index = -1;
                int selectStart = this.richTextBox1.SelectionStart;

                while ((index = this.richTextBox1.Text.IndexOf(word, (index + 1))) != -1)
                {
                    this.richTextBox1.Select((index + startIndex), word.Length);
                    this.richTextBox1.SelectionColor = color;
                    this.richTextBox1.Select(selectStart, 0);
                    this.richTextBox1.SelectionColor = Color.White;
                }
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            // Refresh the dataGridView
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            listBox2.Items.Clear();
            listBox2.Refresh();
            //--------------------------
            List<int> removalList = new List<int>();
            Scanner sc = new Scanner();
            sc.StartScanner(richTextBox1.Text);
            int lien = 0;
            for (int i = 0; i < sc.tokens.Count; i++)
            {
               // if (sc.tokens[i].type != Type.NEWLINE)
                    dataGridView1.Rows.Add(sc.tokens[i].input, sc.tokens[i].type.ToString());
                if (sc.tokens[i].type == Type._FLOAT || sc.tokens[i].type == Type._INT || sc.tokens[i].type == Type._STRING)
                    dtatyps.Add(sc.tokens[i].input);
                if (sc.tokens[i].type == Type.READ || sc.tokens[i].type == Type.WRITE|| sc.tokens[i].type == Type.REPEATSTATEMENT || sc.tokens[i].type == Type.UNTIL||
                    sc.tokens[i].type == Type.IF || sc.tokens[i].type == Type.ELSEIF|| sc.tokens[i].type == Type.ELSE || sc.tokens[i].type == Type.RETURN|| sc.tokens[i].type == Type.END|| sc.tokens[i].type == Type.ENDLINE || sc.tokens[i].type == Type.MAIN || sc.tokens[i].type == Type.PROGRAM|| sc.tokens[i].type == Type.CONSTANT|| sc.tokens[i].type == Type.THEN)
                    rsrvdwrds.Add(sc.tokens[i].input);
                if (sc.tokens[i].type == Type.COMMENT)
                {
                    comment.Add(sc.tokens[i].input);
                    removalList.Add(i);
                }
            }
           // treeView1.Nodes.Add(Parse.PrintParseTree(Parse.treeroot));

          
            for (int i = 0; i < sc.error.Count; i++)
            {
                if (sc.error[i].type == Type.ERROR)
                {
                    listBox2.Items.Add("Error in Line " + (lien++ + 1).ToString() + ", you using ["+ sc.error[i].input +"] ");
                    error.Add(sc.error[i].input);
                }
                else if ( sc.tokens[i].type == Type.COMMENT)
                {
                    lien++;
                  
                };
            }


            for (int i = 0; i < removalList.Count; i++)
            {
                sc.tokens.RemoveAt(removalList[i]);
            }
            Parse ps = new Parse();
            // ps.StartParsing(sc.tokens);
            // if (ps.ll.Items.Count > 0) listBox1.Items.Add(ps.ll.Items[0]);
            //Parser
            ps.StartParsing(sc.tokens);
            Parse.treeroot = ps.root;
            treeView1.Nodes.Add(Parse.PrintParseTree(Parse.treeroot));
            // treeView1.Nodes.Add(ps.root);
            for (int i = 0; i < ps.errors.Count; i++)
            {
                
                    listBox2.Items.Add("Error in Line " + (lien++ + 1).ToString() + ", you using [" + ps.errors[i] + "] ");
                    
                
                

            }
            for (int i = 0; i < rsrvdwrds.Count; i++)
            {
                this.CheckKeyword(rsrvdwrds[i], Color.Violet, 0);
            }

            for (int i = 0; i < dtatyps.Count; i++)
            {
             this.CheckKeyword(dtatyps[i], Color.RoyalBlue, 0);
            }
            for (int i = 0; i < error.Count; i++)
            {
                this.CheckKeyword(error[i], Color.Red, 0);
            }
            for (int i = 0; i < comment.Count; i++)
            {
                this.CheckKeyword(comment[i], Color.DarkSeaGreen, 0);
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        int c = 0;
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            c++;
            if (c == 1)
                richTextBox1.Clear();
           
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
