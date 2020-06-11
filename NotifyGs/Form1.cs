using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Http;

using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace NotifyGs
{




    public partial class Form1 : Form
    {
        private int byosu;

        private System.Windows.Forms.NotifyIcon NotifyIcon1;

        public Form1()
        {
            InitializeComponent();
        }

        private void kosin()
        {
            //Processオブジェクトを作成
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            //ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
            p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            //出力を読み取れるようにする
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = false;
            //ウィンドウを表示しないようにする
            p.StartInfo.CreateNoWindow = true;
            //コマンドラインを指定（"/c"は実行後閉じるために必要）
            p.StartInfo.Arguments = @"/c schtasks /query /tn ""NotifyGs"" ";

            //起動
            p.Start();

          

    
            string results = "";
            try
            {
                results = p.StandardOutput.ReadToEnd();
                //プロセス終了まで待機する
                //WaitForExitはReadToEndの後である必要がある
                //(親プロセス、子プロセスでブロック防止のため)
                Match matche = Regex.Match(results, "\\d\\d\\d\\d/\\d\\d/\\d\\d \\d+:\\d+:\\d+");
                String st = matche.Value;
                if (st.Trim().TrimEnd() != "")
                {
                    toolStripStatusLabel1.Text = "動作中　次回の未読チェック日時：" + st;
                }
                else
                {
                    toolStripStatusLabel1.Text = "動作停止中";
                }
                 //MessageBox.Show(results+"<=results  match_val=>"+matche.Value);
              

            }
            catch(Exception ee)
            {
                toolStripStatusLabel1.Text = "エラー";
            }
          
       
            p.WaitForExit();
            p.Close();

        }


        async private void Form1_Load(object sender, EventArgs e)
        {
            bool flg = true; ////flgにより　false:設定プログラム(NotifySet)か　true:チェックプログラム(NotifyGs)か　内容かえる

            kosin();
            this.TopMost = true;

            byosu = 0;
            this.Opacity = 0;
         //   string[] cmds = System.Environment.GetCommandLineArgs();
            try
            {
                // MessageBox.Show(cmds[1]);
                if (flg) //flgにより　設定プログラム(Setup)か　チェックプログラム(NotifyGs)か　内容をかえる
                {
                    // MessageBox.Show(cmds[1]);
                    this.Opacity = 0;
                    this.ShowInTaskbar = false;
                    timer1.Enabled = true;
                }
                else
                {
                    // MessageBox.Show(cmds[1]);
                    this.Opacity = 100;
                    this.ShowInTaskbar = true;
                    timer1.Enabled = false;
                }
            }
            catch (Exception ee)
            {
                this.Opacity = 100;
                this.ShowInTaskbar = true;
                timer1.Enabled = false;
            }




            if (Properties.Settings.Default.IpAdressPort == "")
            {

            }
            else
            {

                textBox1.Text = Properties.Settings.Default.IpAdressPort;

            }


            if (Properties.Settings.Default.username == "")
            {
                textBoxU.Text = Environment.UserName;
            }
            else
            {
                textBoxU.Text = Properties.Settings.Default.username;
            }

            if (Properties.Settings.Default.password == "")
            {

            }
            else
            {

                PassWord.Text = Properties.Settings.Default.password;

            }

            if (Properties.Settings.Default.kankaku == "")
            {

            }
            else
            {
                textBox2.Text = Properties.Settings.Default.kankaku;
            }


            //NotifyIconオブジェクトを作成する
            //this.componentsが存在しないならば、省略する
            this.NotifyIcon1 = new System.Windows.Forms.NotifyIcon();
            //アイコンを設定する
            this.NotifyIcon1.Icon = new System.Drawing.Icon(System.AppDomain.CurrentDomain.BaseDirectory+"NotifyGs.ico");
            //NotifyIconをタスクトレイに表示する
            this.NotifyIcon1.Visible = true;
            //アイコンの上にマウスポインタを移動した時に表示される文字列
            this.NotifyIcon1.Text = "NotifyGs";
            //アイコンを右クリックしたときに表示するコンテキストメニュー
            //ContextMenuStrip1はすでに用意されているものとする
            // 第1階層のメニュー
          

          

            //Clickイベントハンドラを追加する
            this.NotifyIcon1.Click += new EventHandler(NotifyIcon1_Click);


            try
            {
                String kcou = "";
                XElement rs = await res("/api/circular/countm.do?");
                //MessageBox.Show(rs.ToString());
                kcou= StripHTMLTag(rs.ToString());
               // MessageBox.Show(kcou);
                if (kcou.Trim() !="0")
                {
                    // MessageBox.Show("接続Ok");
                    NotifyIcon1.BalloonTipTitle = "グループセッション新着情報";
                    //  'バルーンヒントに表示するメッセージ
                    NotifyIcon1.BalloonTipText = textBoxU.Text + "さんに、新着の回覧(" +kcou+ "件)が届いています。ここクリックしてください。";
                    //  'バルーンヒントに表示するアイコン
                    NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                    //   'バルーンヒントを表示する
                    //   '表示する時間をミリ秒で指定する
                    NotifyIcon1.ShowBalloonTip(1000 * 60 * 3);
                    this.NotifyIcon1.BalloonTipClicked += new EventHandler(NotifyIcon1_BalloonTipClicked);

                }
                else
                {
                   // MessageBox.Show("接続できません。UserName,PassWordやURLを再度確認してください");
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("UserName,PassWordやURLを再度確認してください:"+ee.Message.ToString());
            }



        }

        public static string StripHTMLTag(string strHtml)
        {
            string strOutMd = strHtml;
            Regex regex = new Regex(@"<[^>]+>|</[^>]+>");

            strOutMd = regex.Replace(strOutMd,"");
            return strOutMd;
        }


        private void NotifyIcon1_BalloonTipClicked(object sender, EventArgs e) 
        {

            System.Diagnostics.Process.Start("http://"+textBox1.Text+"/gsession");
        }
     


        private void NotifyIcon1_Click(object sender, EventArgs e)
        {
            //  System.Windows.Forms.MessageBox.Show("アイコンがクリックされました。");
            System.Diagnostics.Process.Start("http://"+textBox1.Text+"/gsession");
        }


        async private void Button1_Click(object sender, EventArgs e)
        {
            

        }

        async private void accesscheck()
        {
            //String rs = await res("/api/circular/countm.do?");
            try
            {
                XElement rs = await res("/api/user/whoami.do?");
                if (textBoxU.Text == (String)rs.Element("Result").Element("LoginId"))
                {
                   // MessageBox.Show("接続Ok");
                }
                else
                {
                    MessageBox.Show("接続できません。UserName,PassWordやURLを再度確認してください");
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("UserName,PassWordやURLを再度確認してください");
            }



            // MessageBox.Show((String)rs.Element("Result").Element("LoginId"));
            //MessageBox.Show(rs.ToString());
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            accesscheck();

            

            //アプリケーションの設定を保存する
            Properties.Settings.Default.IpAdressPort = textBox1.Text;
            Properties.Settings.Default.username = textBoxU.Text;
            Properties.Settings.Default.password = PassWord.Text;
            Properties.Settings.Default.kankaku = textBox2.Text;
            Properties.Settings.Default.Save();

            String appPath= System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');// saigoni \nasi
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/c ";
            startInfo.Arguments += "schtasks ";
            startInfo.Arguments += " /create /f /tn \"NotifyGs\" /tr \"" + appPath + "\\NotifyGs.exe\" /sc minute /mo " + textBox2.Text;
            var proc = Process.Start(startInfo);
            proc.WaitForExit();

            kosin();
            MessageBox.Show("設定を保存しました。");

        }



        async private Task<XElement> res(String Url)
        {
            // Basic認証するユーザ名とパスワード
            var userName = textBoxU.Text;
            var userPassword = PassWord.Text;

            // リクエストの生成
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://"+textBox1.Text +"/gsession"+ Url)

            };


            // Basic認証ヘッダを付与する
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", userName, userPassword))));

            XElement st = null;
            // リクエストの送信
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.SendAsync(request);
                //MessageBox.Show(response.ToString());
                var responseBody = await response.Content.ReadAsStringAsync();
                // MessageBox.Show(Regex.Replace(responseBody, "<[^>]*?>", ""));
                st = XElement.Parse(responseBody);
            }

            return st;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
           // System.Diagnostics.Process.Start(@"schtasks /delete /f /tn 'NotifyGs'");

            // ●プロセス起動情報の構築
            ProcessStartInfo startInfo = new ProcessStartInfo();

            // バッチファイルを起動する人は、cmd.exeさんなので
            startInfo.FileName = "cmd.exe";

            // コマンド処理実行後、コマンドウィンドウ終わるようにする。
            //（↓「/c」の後の最後のスペース1文字は重要！）
            startInfo.Arguments = "/c ";

            // コマンド処理であるバッチファイル （ここも最後のスペース重要）
            //startInfo.Arguments += @"d:\bin\YourBatFile.bat ";
            startInfo.Arguments += "schtasks ";
            // バッチファイルへの引数 
            startInfo.Arguments += "/delete /f /tn \"NotifyGs\" ";


            // ●バッチファイルを別プロセスとして起動
            var proc = Process.Start(startInfo);

            //出力を読み取る
            //string results = proc.StandardOutput.ReadToEnd();

            // ●上記バッチ処理が終了するまで待ちます。
            proc.WaitForExit();
            kosin();
            MessageBox.Show("設定を削除しました。");
           // this.Close();
        }



        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
           
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
         
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.NotifyIcon1.Dispose();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            byosu++;
            if (byosu > 10)
            {
                this.Close();
            }
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            kosin();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }




}

