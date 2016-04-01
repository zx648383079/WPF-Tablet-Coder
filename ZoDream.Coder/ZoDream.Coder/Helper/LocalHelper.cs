using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using ZoDream.Coder.Helper.Local;

namespace ZoDream.Coder.Helper
{
    public class LocalHelper
    {
        private const string Filters = "文本文件|*.txt|Javascript|*.js|Style|*.css|PHP|*.php|C#、WPF、.net|*.cs|Typescript|*.ts|C语言|*.c|C++|*.cpp|HTML|*.html;*.htm|所有文件|*.*";

        /// <summary>
        /// 浏览文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void ExploreFile(string filePath)
        {
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = "explorer",
                    Arguments = @"/select," + filePath
                }
            };
            //打开资源管理器
            //选中"notepad.exe"这个程序,即记事本
            proc.Start();
        }

        /// <summary>
        /// 浏览文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void ExplorePath(string path)
        {
            Process.Start("explorer.exe", path);
        }


        /// <summary>
        /// 遍历文件夹
        /// </summary>
        /// <param name="dir"></param>
        public static List<string> GetAllFile(string dir)
        {
            var files = new List<string>();
            if (string.IsNullOrWhiteSpace(dir))
            {
                return files;
            }
            var theFolder = new DirectoryInfo(dir);
            var dirInfo = theFolder.GetDirectories();
            //遍历文件夹
            foreach (var nextFolder in dirInfo)
            {
                files.AddRange(GetAllFile(nextFolder.FullName));
            }

            var fileInfo = theFolder.GetFiles();
            //遍历文件
            files.AddRange(fileInfo.Select(nextFile => nextFile.FullName));
            return files;
        }

        public static string ChooseSaveFile(string name = "")
        {
            var open = new SaveFileDialog
            {
                Title = "选择保存路径",
                Filter = Filters,
                FileName = name
            };
            return open.ShowDialog() == true ? open.FileName : null;
        }

        /// <summary>
        /// 选择个文件
        /// </summary>
        /// <returns></returns>
        public static string ChooseFile()
        {
            var open = new OpenFileDialog
            {
                Multiselect = true,
                Filter = Filters,
                Title = "选择文件"
            };
            return open.ShowDialog() == true ? open.FileName : string.Empty;
        }

        public static string GetText(string file)
        {
            if (string.IsNullOrEmpty(file)) return null;
            string content;
            using (var fs = new FileStream(file, FileMode.OpenOrCreate))
            {
                var reader = new StreamReader(fs, new TxtEncoder().GetEncoding(fs));
                content = reader.ReadToEnd();
                reader.Close();
                fs.Close();
            }
            return content;
        }

        public static void SaveFile(string content, string file)
        {
            var writer = new StreamWriter(file, false, Encoding.UTF8);
            writer.Write(content);
            writer.Close();
        }

        /// <summary>
        /// 遍历文件夹
        /// </summary>
        /// <param name="dir"></param>
        public static List<string> GetAllFileName(string dir)
        {
            var files = new List<string>();
            if (string.IsNullOrWhiteSpace(dir))
            {
                return files;
            }
            var theFolder = new DirectoryInfo(dir);
            var fileInfo = theFolder.GetFiles();
            files.AddRange(from info in fileInfo where info.Extension.Equals(".xml", StringComparison.CurrentCultureIgnoreCase) select info.Name.Replace(".xml", ""));
            return files;
        }
    }
}
