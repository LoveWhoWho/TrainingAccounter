using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TrainingAccounter
{
    /// <summary>
    /// versionChange.xaml 的交互逻辑
    /// </summary>
    public partial class versionChange : Window
    {
        public versionChange()
        {
            InitializeComponent();
            ShowVersion();
        }
        private void ShowVersion()
        {
            try
            {
                StreamReader str;
                var textPath = Environment.CurrentDirectory;
                using (str = new StreamReader(textPath + @"\Refrence\vertionInformation.txt", System.Text.Encoding.Default))
                {
                    // String input = file.ReadToEnd();
                    String line = null;
                    while ((line = str.ReadLine()) != null)
                    {
                        this.rtxVersion.AppendText(line + Environment.NewLine);
                    }
                    str.Close();

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("显示版本信息出错，无措信息：" + e.Message.ToString());
                return;
            }

        }
        private void LoadRtfFile()
        {
            var textPath = Environment.CurrentDirectory;
            FileStream fs = File.Open(textPath + @"\Refrence\vertionInformation.txt", FileMode.Open);

            TextRange textRange = new TextRange(rtxVersion.Document.ContentStart, rtxVersion.Document.ContentEnd);
            textRange.Load(fs, DataFormats.Rtf);

            fs.Close();
        }
        //path为完整保存路径名  
        private void SaveRtfFile(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);

            TextRange range;
            range = new TextRange(rtxVersion.Document.ContentStart, rtxVersion.Document.ContentEnd);
            range.Save(fs, DataFormats.Rtf);//DataFormats.Xaml 或者 DataFormats.XamlPackage  

            fs.Close();
        }  
    }
}
