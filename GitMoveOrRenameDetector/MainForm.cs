using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace GitMoveOrRenameDetector
{
    public partial class MainForm : Form
    {
        private string[] _DeletedFiles;
        private string[] _NewFiles;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetProjectDirAsCurrentDirectory();

            //方案1:用户在提交页面手动复制要检测重命名的文件,然后从剪贴板直接读取即可.
            //操作步骤:用户在TortoiseGit提交页面选中OldFile和NewFile,然后Ctrl+C进行复制

            //方案2:直接调用Git命令遍历当前目录,直接读取新增的文件和缺失的文件.
            GuessGitExeFilePath();

            InitData();
        }

        /// <summary>
        /// 初始化界面数据
        /// </summary>
        private void InitData()
        {
            //CD /D "D:\2-CodingLife\我的项目\52.JS脚本库"

            //获取被删除的文件名列表
            //file:///D:/Program%20Files/Git/mingw64/share/doc/git-doc/git-diff.html
            //"D:\Program Files\Git\cmd\git.exe" diff --diff-filter=D --name-only
            //"D:\Program Files\Git\cmd\git.exe" ls-files --deleted
            _DeletedFiles = GetCmdResult(GitExeFilePath, "ls-files --deleted");
            richTextBox1.Lines = _DeletedFiles;

            //显示不受版本控制的文件列表
            //file:///D:/Program%20Files/Git/mingw64/share/doc/git-doc/git-ls-files.html
            //"D:\Program Files\Git\cmd\git.exe" ls-files --others
            _NewFiles = GetCmdResult(GitExeFilePath, "ls-files --others");
        }

        private static string[] GetCmdResult(string exeFilePath, string args)
        {
            string result;
            using (var myProcess = new Process())
            {
                myProcess.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                myProcess.StartInfo.FileName = exeFilePath;
                myProcess.StartInfo.Arguments = args;

                myProcess.StartInfo.UseShellExecute = false;

                myProcess.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                myProcess.StartInfo.RedirectStandardOutput = true;

                myProcess.Start();

                result = myProcess.StandardOutput.ReadToEnd();

                myProcess.WaitForExit();
            }

            return result.Split(new[] {'\r', '\n'}, int.MaxValue, StringSplitOptions.RemoveEmptyEntries);
        }

        #region 探测到的Git.exe文件所在位置,方便调用

        /// <summary>
        /// 探测Git.exe文件所在位置,方便调用
        /// </summary>
        private string GitExeFilePath { get; set; }

        private void GuessGitExeFilePath()
        {
            var filePath = GuessGitExeFilePathInProgramFiles();
            if (!string.IsNullOrEmpty(filePath))
            {
                GitExeFilePath = filePath;
                return;
            }

            MessageBox.Show("无法探测到Git.exe程序所在位置!\n\n暂只支持固定的路径:\"?:\\Program Files\\Git\\cmd\\git.exe\"", "探测失败");
            Environment.Exit(2);
        }

        private static string GuessGitExeFilePathInProgramFiles()
        {
            foreach (var driver in Environment.GetLogicalDrives())
            {
                foreach (var pf in new[] {"Program Files", "Program Files (x86)"})
                {
                    var filePath = driver + pf + @"\Git\cmd\git.exe";
                    if (File.Exists(filePath))
                        return filePath;
                }
            }

            return null;
        }

        #endregion

        #region 设置当前工作目录为调用者指定的目录

        /// <summary>
        /// 设置当前工作目录为调用者指定的目录
        /// </summary>
        private static void SetProjectDirAsCurrentDirectory()
        {
            var commandLineArgs = Environment.GetCommandLineArgs();
            if (commandLineArgs.Length < 2)
            {
                MessageBox.Show("请传递的Git项目所在路径", "参数不全");
                Environment.Exit(1);
                return;
            }

            var pDir = commandLineArgs[1];
            if (!Directory.Exists(pDir))
            {
                MessageBox.Show(pDir + " 不存在!", "参数不对");
                Environment.Exit(1);
                return;
            }

            if (!Directory.Exists(pDir))
            {
                MessageBox.Show(pDir + " 不存在!", "参数不对");
                Environment.Exit(1);
                return;
            }

            var gitDir = pDir + "\\.git";
            if (!Directory.Exists(gitDir))
            {
                MessageBox.Show(pDir + " 不是正确的被Git版本控制的目录!", "目录不对");
                Environment.Exit(1);
                return;
            }

            //设置当前工作目录为调用者指定的目录
            Directory.SetCurrentDirectory(pDir);
        }

        #endregion

        private void btnChcek_Click(object sender, EventArgs e)
        {
            //遍历被删除的文件, 找到其最新的文件名
            var delAndNewDic = new Dictionary<string, NewFileInfo>();
            foreach (var delFileStr in _DeletedFiles)
            {
                var delFile = new FileInfo(delFileStr);

                //遍历最新文件名, 找到相似度最高的
                foreach (var newFileStr in _NewFiles)
                {
                    var newFile = new FileInfo(newFileStr);

                    //更改后缀的情况.
                    if (newFile.Extension != delFile.Extension)
                        continue;

                    //场景1: 同一个文件名,只是移动了到不同目录了.
                    if (newFile.Name == delFile.Name)
                    {
                        delAndNewDic.Add(delFileStr, new NewFileInfo(newFileStr));
                        break; //文件名一样,优先级最高.直接覆盖并返回.
                    }

                    //场景2: 同一个目录,只是重命名了
                    if (newFile.Directory?.FullName == delFile.Directory?.FullName)
                    {
                        //场景3: 同一个目录,只是重命名了,并且前缀是一样的.例如Config.220101.reg -> COnfig.220202.reg
                        var theSamePrefixCount = GetTheSamePrefixCount(newFile.Name, delFile.Name);
                        if (theSamePrefixCount > 0)
                        {
                            NewFileInfo newFileInfo;
                            if (!delAndNewDic.TryGetValue(delFileStr, out newFileInfo))
                                delAndNewDic.Add(delFileStr, newFileInfo = new NewFileInfo());
                            newFileInfo.AddWithTheSamePrefixCount(theSamePrefixCount, newFileStr);
                            //NO:不能结束循环,因为可能有比它更相似的前缀存在break;
                        }
                    }
                }
            }

            //将检测到重命名文件显示到界面
            var renameList = new List<String>();
            int i = 0;
            foreach (var kv in delAndNewDic)
            {
                renameList.Add(++i + "." + kv.Key);
                renameList.Add("└→ " + kv.Value.getNewFileStr());
                renameList.Add("");
            }

            rtbRename.Lines = renameList.ToArray();
        }

        private class NewFileInfo
        {
            public NewFileInfo()
            {
            }

            private readonly string _NewFileStr;

            public NewFileInfo(string newFileStr)
            {
                _NewFileStr = newFileStr;
            }

            public string getNewFileStr()
            {
                if (_NewFileStr != null)
                    return _NewFileStr;

                //取前缀最相似的路径返回
                KeyValuePair<int, String> maxKv = new KeyValuePair<int, string>();
                foreach (var kv in _TheSamePrefixCountAndFileStrDic)
                {
                    if (kv.Key > maxKv.Key)
                        maxKv = kv;
                }

                return maxKv.Value;
            }

            private readonly Dictionary<int, string> _TheSamePrefixCountAndFileStrDic = new Dictionary<int, string>();

            public void AddWithTheSamePrefixCount(int theSamePrefixCount, string newFileStr)
            {
                _TheSamePrefixCountAndFileStrDic.Add(theSamePrefixCount, newFileStr);
            }
        }

        private int GetTheSamePrefixCount(string newFileName, string delFileName)
        {
            if (newFileName == delFileName)
                return newFileName.Length;
            int TheSamePrefixCount = 0;
            for (int i = 0; i < delFileName.Length; i++)
            {
                if (i >= newFileName.Length)
                    break;
                if (delFileName[i] == newFileName[i])
                    TheSamePrefixCount++;
                else
                    return TheSamePrefixCount;
            }

            return TheSamePrefixCount;
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}