using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace なんでも検索ツール
{
    public partial class Form1 : Form
    {
        public XElement xml;

        private List<Button> buttonList = new List<Button>();
        int intButtonSize = 70;
        int Buttons = 12;
    
        public Form1()
        {
            InitializeComponent();
            string strClip = ClipGet();
            this.MaximizeBox = false;
            comboBox1.Text = strClip;
            Update();
            HistoryGet();
            FormLoad();
        }
        public void Clear()
        {
            for (int r = 1; r <= 12; r++)
            {
                buttonList[r - 1].Dispose();
            }
            buttonList = new List<Button>();
        }

        public void Update()
        {
            string strOptionPath = Application.StartupPath + "/option.xml";
            xml = XElement.Load(strOptionPath);
        }

        public void FormLoad()
        {
            for (int r = 1; r <= Buttons; r++)
            {
                Button button = new System.Windows.Forms.Button();

                if (r > 7)
                {
                    ButtonSet(ref button, "property" + r.ToString(), 3 + intButtonSize * (r-9),114, r);
                    
                }
                else
                {
                    ButtonSet(ref button, "property" + r.ToString(), 3 + intButtonSize * (r-1), 44, r);
                }
                buttonList.Add(button);
            }
        }

        private void ButtonSet(ref Button button,string Property,int x,int y,int Number)
        {
            var a = from item in xml.Elements(Property) select item;
            if (a.Count() > 0)
            {
                XElement xElement = (from item in xml.Elements(Property) select item).Single();
                Console.WriteLine(xElement.Element("Text").Value);
                button.Location = new System.Drawing.Point(x, y);
                button.Name = Number.ToString();
                button.Size = new System.Drawing.Size(intButtonSize, intButtonSize);
                button.TabIndex = 11;
                button.BackColor = Color.LightGray;
                button.Text = xElement.Element("Text").Value;
                button.TextAlign = ContentAlignment.BottomCenter;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                //button.ForeColor = Color.WhiteSmoke;
                if (File.Exists(xElement.Element("ExePath").Value))
                {
                    Icon appIcon = Icon.ExtractAssociatedIcon(xElement.Element("ExePath").Value);
                    button.Image = appIcon.ToBitmap();
                }
                else
                {
                    string strFolderPath = Application.StartupPath + "/option/folder.png";
                    button.BackgroundImage = Image.FromFile(strFolderPath);
                    button.BackgroundImageLayout = ImageLayout.Center;
                }
                float fontSize = 7;
                if (xElement.Element("Text").Value.Length>8)
                {
                    fontSize = 6;
                }

                button.Font = new Font(button.Font.OriginalFontName, fontSize);

                this.toolTip1.SetToolTip(button, xElement.Element("Text").Value);
               // button.KeyDown += new System.Windows.Forms.KeyEventHandler(Keydownaction);
                button.MouseDown += new System.Windows.Forms.MouseEventHandler(buttonClickaction);
                this.Controls.Add(button);
            }

        }

        private void buttonClickaction(object sender, MouseEventArgs e)
        {
            XElement xElement = (from item in xml.Elements("property" + ((System.Windows.Forms.Control)sender).Name) select item).Single();
            int intActionNumber = 0;
            int intCount = 0;

            if (xElement.Element("Argument").Value == "")
            {
                intActionNumber = 0;
            }
            if (xElement.Element("Argument").Value != "")
            {
                intActionNumber = 1;
            }
            if (xElement.Element("ExePath").Value == "explorer.exe")
            {
                intActionNumber = 2;
            }


            if (e.Button == MouseButtons.Left)
            {
                switch (intActionNumber)
                {
                    case 0: //引数なし
                        System.Diagnostics.Process.Start(xElement.Element("ExePath").Value);                       
                        break;
                    case 1: // 引数あり
                        System.Diagnostics.Process.Start(xElement.Element("ExePath").Value, xElement.Element("Argument").Value + comboBox1.Text);
                        break;
                    case 2: // エクスプローラ用
                        System.Diagnostics.Process.Start(xElement.Element("ExePath").Value, comboBox1.Text);
                        break;
                    default:
                        break;
                }
                KeyAction(int.Parse(xElement.Element("KeyAction").Value));
                HistoyItms_Write();
                HistoryGet();

            }
            if (e.Button == MouseButtons.Right)
            {
                for (int r = 1; r <= Buttons; r++)
                {
                    var a = from item in xml.Elements("property" + r.ToString()) select item;
                    if (a.Count() == 0)
                    {
                        intCount = r-1;
                        break;
                    }
                }
                ButtonEdit(int.Parse(((System.Windows.Forms.Control)sender).Name));
            }

        }

        private void ButtonEdit(int intNumber)
        {
            Form2 form2 = new Form2(intNumber);
            form2.FormClosed += new FormClosedEventHandler(Form2_FormClosed);
            form2.Show();
        }

        //Form2が閉じた時
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Restart();
        }

        public void ClipInput()
        {
            Clipboard.SetText(comboBox1.Text);
        }

        public string ClipGet()
        {
            string InputText = "";
            IDataObject ClipboardData = Clipboard.GetDataObject(); // クリップボードからオブジェクトを取得する。
            if (ClipboardData.GetDataPresent(DataFormats.Text)) // テキストデータかどうか確認する。
            {
                InputText = (string)ClipboardData.GetData(DataFormats.Text); // オブジェクトからテキストを取得する。
            }
            else
            {
                MessageBox.Show("コピーしたデータが文字列ではありません。");
            }
            return InputText;
        }

        public void KeyAction(int intNumber)
        {
            switch (intNumber)
            {
                case 0:
                    break;
                case 1:
                    Thread.Sleep(1000);
                    ClipInput();
                    writeKeys("^E");
                    writeKeys("^V");
                    writeKeys("{ENTER}");
                    break;
                case 2:
                    ClipInput();
                    Thread.Sleep(1000);
                    writeKeys("^F");
                    writeKeys("^V");
                    writeKeys("{ENTER}");
                    break;
                case 3:
                    ClipInput();
                    Thread.Sleep(1000);
                    writeKeys("^V");
                    writeKeys("{ENTER}");
                    break;
                default:
                    break;
            }
        }
        public void writeKeys(string keys)
        {
            // 選択しているウィンドウを取得
            IntPtr targetWindowHandle = MyWindowApi.GetForegroundWindow();
            if (targetWindowHandle == IntPtr.Zero)
            {
                // 操作できるウィンドウがない
                return;
            }
            // 現在選択しているウィンドウに対してキーを送信
            SendKeys.SendWait(keys);
        }

        /// <summary>
        /// ウィンドウ周りのAPI参照クラス
        /// </summary>
        class MyWindowApi
        {
            /// <summary>
            /// 選択しているウィンドウのハンドルを取得
            /// </summary>
            /// <returns>ウィンドウのハンドル</returns>
            [DllImport("USER32.DLL")]
            public static extern IntPtr GetForegroundWindow();
        }

        //内訳リスト書き込み処理
        public void HistoyItms_Write()
        {
            string SearchWord = comboBox1.Text;
            string strHistroy_Kind = "";
            //Wordの場合
            strHistroy_Kind = "Word";

            //ファイルパスの場合
            if (SearchWord.Contains(":") && SearchWord.Contains("/"))
            {
                strHistroy_Kind = "path";
            }

            //websiteの場合
            if (SearchWord.Contains("http"))
            {
                strHistroy_Kind = "web";
            }
            string filePath = Application.StartupPath + "/" + strHistroy_Kind + ".txt";

            using (StreamWriter sw = new StreamWriter(filePath,true))
            {
                sw.WriteLine(SearchWord);
            }

        }

        public void HistoryGet()
        {
            string strHistroy_Kind = "web";
            comboBox1.Items.Clear();
            string filePath = Application.StartupPath + "/" + strHistroy_Kind +".txt";

            List<string> De_list = new List<string>();         //空のListを作成する

            //Usingはガベージコレクションが働く　ファイルの入出力で使うとリソースを自動的に破棄する
            using (StreamReader file = new StreamReader(filePath))
            {
                string line = "";

                // test.txtを1行ずつ読み込んでいき、末端(何もない行)までwhile文で繰り返す
                while ((line = file.ReadLine()) != null)
                {
                    De_list.Add(line);
                }

                foreach (var item in De_list)
                {
                    //comboBox1になければ追加する　重複無視
                    if (!comboBox1.Items.Contains(item) && item != "")
                    {
                        comboBox1.Items.Add(item);
                    }
                }

            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:   // 検索
                    Key_Action(1);
                    break;
                case Keys.F2:
                    Key_Action(2);
                    break;
                case Keys.F3:
                    Key_Action(3);
                    break;
                case Keys.F4:
                    Key_Action(4);
                    break;
                case Keys.F5:
                    Key_Action(5);
                    break;
                case Keys.F6:
                    Key_Action(6);
                    break;
                case Keys.F7:
                    Key_Action(7);
                    break;
                case Keys.F8:
                    Key_Action(8);
                    break;
                case Keys.F9:
                    Key_Action(9);
                    break;
                case Keys.F10:
                    Key_Action(10);
                    break;
                case Keys.F11:
                    Key_Action(11);
                    break;
                case Keys.F12:
                    Key_Action(12);
                    break;
            }
        }
        private void Key_Action(int FNumber)
        {
            XElement xElement = (from item in xml.Elements("property" + FNumber.ToString()) select item).Single();
            int intActionNumber = 0;
            int intCount = 0;

            if (xElement.Element("Argument").Value == "")
            {
                intActionNumber = 0;
            }
            if (xElement.Element("Argument").Value != "")
            {
                intActionNumber = 1;
            }
            if (xElement.Element("ExePath").Value == "explorer.exe")
            {
                intActionNumber = 2;
            }

                switch (intActionNumber)
                {
                    case 0: //引数なし
                        System.Diagnostics.Process.Start(xElement.Element("ExePath").Value);
                        break;
                    case 1: // 引数あり
                        System.Diagnostics.Process.Start(xElement.Element("ExePath").Value, xElement.Element("Argument").Value + comboBox1.Text);
                        break;
                    case 2: // エクスプローラ用
                        System.Diagnostics.Process.Start(xElement.Element("ExePath").Value, comboBox1.Text);
                        break;
                    default:
                        break;
                }
                KeyAction(int.Parse(xElement.Element("KeyAction").Value));
                HistoyItms_Write();
                HistoryGet();
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileName = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            int intCount = 0;
            for (int r = 1; r <= 20; r++)
            {
                var a = from item in xml.Elements("property" + r.ToString()) select item;
                if (a.Count() == 0)
                {
                    intCount = r;
                    break;
                }
            }

            var NewPropety = new XElement("property" + intCount.ToString(),
                            new XElement("type", "2"),
                            new XElement("ExePath", fileName[0]),
                            new XElement("Text", Path.GetFileName(fileName[0])),
                            new XElement("KeyAction", "2"),
                            new XElement("Argument", ""));
            xml.Add(NewPropety);
            xml.Save(Application.StartupPath + "/option.xml");
            ResetAction();
            FormLoad();
        }

        private void ResetAction()
        {
            Update();
            Clear();
            XmlSort();
            FormLoad();
        }

        private void wordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HistoryGet();
        }

        private void pathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HistoryGet();
        }

        private void webToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HistoryGet();
        }

        private void 新規追加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int intCount = 0;
            for (int r = 1; r <= 20; r++)
            {
                var a = from item in xml.Elements("property" + r.ToString()) select item;
                if (a.Count() == 0)
                {
                    intCount = r;
                    break;
                }
            }

            var NewPropety = new XElement("property" + intCount.ToString(),
                new XElement("type", "2"),
                new XElement("ExePath", ""),
                new XElement("Text", ""),
                new XElement("KeyAction", "2"),
                new XElement("Argument", ""));
            xml.Add(NewPropety);
            xml.Save(Application.StartupPath + "/option.xml");

            Form2 form2 = new Form2(intCount);
            form2.FormClosed += new FormClosedEventHandler(Form2_FormClosed);
            form2.Show();
        }

        private void XmlSort()
        {
            int intCount = 1;
            XElement NewXElement = new XElement("buttonList");
            for (int r = 1; r <= Buttons; r++)
            {
                var Check = from item in xml.Elements("property" + r.ToString()) select item;
                if (Check.Count() == 0)
                {
                    continue;
                }
                else
                {
                    XElement xElement = (from item in xml.Elements("property" + r.ToString()) select item).Single();
                    xElement.Name = "property" + intCount.ToString();
                    NewXElement.Add(xElement);
                    intCount += 1;
                }
            }

            xml = NewXElement;
            xml.Save(Application.StartupPath + "/option.xml");
        }

    }
}
