using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace なんでも検索ツール
{
    public partial class Form2 : Form
    {
        XElement xml = null;
        XElement xElement = null;
        int _intNumber = 0;

        public Form2(int intNumber)
        {
            InitializeComponent();
            _intNumber = intNumber;
            string strOptionPath = Application.StartupPath + "/option.xml";
            xml = XElement.Load(strOptionPath);
            xElement = (from item in xml.Elements("property" + intNumber.ToString()) select item).Single();
            textBox1.Text = xElement.Element("ExePath").Value;
            textBox2.Text = xElement.Element("Argument").Value;
            textBox3.Text = xElement.Element("Text").Value;
            textBox4.Text = xElement.Element("KeyAction").Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            xElement.Element("ExePath").Value = textBox1.Text;
            xElement.Element("Argument").Value = textBox2.Text;
            xElement.Element("Text").Value = textBox3.Text;
            xElement.Element("KeyAction").Value = textBox4.Text;
            xml.Save(Application.StartupPath + "/option.xml");
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            XElement xElement = (from item in xml.Elements("property" + _intNumber.ToString()) select item).Single();
            xElement.Remove();
            xml.Save(Application.StartupPath + "/option.xml");
            this.Dispose();
        }
    }
}
