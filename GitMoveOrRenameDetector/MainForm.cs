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
            Text = $"GitMoveOrRenameDetector (By:AsionTang) v220614.01.01.001 {Directory.GetCurrentDirectory()}";

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

                myProcess.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                myProcess.StartInfo.RedirectStandardError = true;

                myProcess.Start();

                result = myProcess.StandardOutput.ReadToEnd();
                result += myProcess.StandardError.ReadToEnd();

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

        private readonly Dictionary<string, NewFileInfo> _DelAndNewDic = new Dictionary<string, NewFileInfo>();

        private void btnChcek_Click(object sender, EventArgs e)
        {
            _DelAndNewDic.Clear();
            //遍历被删除的文件, 找到其最新的文件名
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
                        _DelAndNewDic.Add(delFileStr, new NewFileInfo(newFileStr));
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
                            if (!_DelAndNewDic.TryGetValue(delFileStr, out newFileInfo))
                                _DelAndNewDic.Add(delFileStr, newFileInfo = new NewFileInfo());
                            newFileInfo.AddWithTheSamePrefixCount(theSamePrefixCount, newFileStr);
                            //NO:不能结束循环,因为可能有比它更相似的前缀存在break;
                        }
                    }
                }
            }

            //将检测到重命名文件显示到界面
            var renameList = new List<String>();
            int i = 0;
            foreach (var kv in _DelAndNewDic)
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
            //::先还原修改, 将已经移动后的文件, 移动回来, 否则后续git-mv会因为找不到源文件报错, 使用-f -i 也没有效果.
            //::需要替换为 Windows 路径分隔符\
            //move dir2\new.ext dir1\old.ext
            //
            //::再使用git的mv命令来移动, 以便保持变更记录
            //git mv dir1/old.ext dir2/new.ext
            //
            //碰到的问题
            //1. 参考 始终在独立提交中移动和重命名 Git 文件
            //1. 原因就是实际操作过程中发现, 没法在一个提交里, 既操作了MV, 又修改了文件,类似HG一样
            //1. 必须先将未经修改的源文件使用MV移动到新位置,
            //1. 再修改该文件, 这样才不会出现问题.
            //1. 否则会出现无法正常提交, 无法选中要提交的文件等等异常情况.

            //麻烦的操作步骤
            //>Git 部分提交
            // git stash 将工作目录的修改,暂存起来,然后再执行以下新的git stash pop
            //  stash（暫存） | 連猴子都能懂的Git入門指南 | 貝格樂（Backlog）
            //1. 先备份新文件为MOVE  dir2\new.ext dir2\new.ext.bak
            //1. 再还原老文件git checkout -- dir1\old.ext
            //1. 将老文件使用GIT移动到新文件名git mv dir1/old.ext dir2/new.ext
            //1. 将移动这一操作单独提交为一个版本git commit -m 'commit message'
            //1. 将新文件备份还原MOVE  dir2\new.ext.bak dir2\new.ext
            //1. 最后可以正常提交业务修改了.

            //将检测到重命名文件显示到界面
            var renameList = new List<String>();

            renameList.Add("【１】先把重命名后的新文件备份起来");
            renameList.Add("------------------------------------------------------------");
            int i = 0;
            foreach (var kv in _DelAndNewDic)
            {
                //var oldFileStr = kv.Key;
                var newFileStr = kv.Value.getNewFileStr();
                renameList.Add($"{++i}.{newFileStr}.bak");
                File.Move(newFileStr, newFileStr + ".bak");
            }

            renameList.Add("");

            //必须先还原,否则后续的暂存时会将旧文件被删除的状态给暂存起来,导致后续还原暂存时出现状态冲突
            renameList.Add("【２】再把被重命名的旧文件还原回来");
            renameList.Add("------------------------------------------------------------");
            i = 0;
            foreach (var kv in _DelAndNewDic)
            {
                var oldFileStr = kv.Key;
                renameList.Add($"{++i}.{oldFileStr}");
                renameList.AddRange(GetCmdResult(GitExeFilePath, $"checkout -- \"{oldFileStr}\""));
            }

            renameList.Add("");

            renameList.Add("【３】把工作目录状态暂存起来(把工作目录弄干净,方便后续临时提交一个纯纯的重命名的版本");
            renameList.Add("------------------------------------------------------------");
            //file:///D:/Program%20Files/Git/mingw64/share/doc/git-doc/git-stash.html
            renameList.AddRange(GetCmdResult(GitExeFilePath, $"stash"));
            renameList.Add("");

            i = 0;
            foreach (var kv in _DelAndNewDic)
            {
                var oldFileStr = kv.Key;
                var newFileStr = kv.Value.getNewFileStr();

                renameList.Add($"{++i}.{oldFileStr}");
                renameList.Add($"└→ {newFileStr}");
                renameList.Add("└→ 使用Git MV指令重命名为新文件");
                renameList.AddRange(GetCmdResult(GitExeFilePath, $"mv \"{oldFileStr}\" \"{newFileStr}\""));
                renameList.Add("");
            }

            renameList.Add("【４】提交一个纯纯的重命名的版本");
            renameList.Add("------------------------------------------------------------");
            renameList.AddRange(GetCmdResult(GitExeFilePath, $"commit -m \"+移动了:或重命名了 {i} 个文件\""));
            renameList.Add("");

            renameList.Add("【５】将暂存起来的状态还原到工作目录");
            renameList.Add("------------------------------------------------------------");
            renameList.AddRange(GetCmdResult(GitExeFilePath, $"stash pop"));
            renameList.Add("");

            renameList.Add("【６】最后把重命名后的新文件备份还原回来");
            renameList.Add("------------------------------------------------------------");
            i = 0;
            foreach (var kv in _DelAndNewDic)
            {
                var newFileStr = kv.Value.getNewFileStr();
                renameList.Add($"{++i}.{newFileStr}");

                File.Delete(newFileStr);
                File.Move(newFileStr + ".bak", newFileStr);
            }

            renameList.Add("");

            rtbRename.Lines = renameList.ToArray();

            //打开提交界面
            btnOpenCommit_Click(null, null);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            InitData();
            rtbRename.Clear();
        }

        #region Git常用功能按钮区

        private void StartExe(string fileName, string arguments)
        {
            try
            {
                Process.Start(fileName, arguments);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void btnOpenGitCmd_Click(object sender, EventArgs e)
        {
            StartExe(GitExeFilePath + @"\..\..\git-cmd.exe", null);
        }

        // REM 可用的后缀列表 :push :commit :reflog
        // REM Appendix D. Automating TortoiseGit – TortoiseGit – Documentation – TortoiseGit – Windows Shell Interface to Git		
        // REM https://tortoisegit.org/docs/tortoisegit/tgit-automation.html

        private void btnOpenGitLog_Click(object sender, EventArgs e)
        {
            StartExe(GitExeFilePath + @"\..\..\..\TortoiseGit\bin\TortoiseGitProc.exe", "/command:log");
        }

        private void btnOpenGitReflog_Click(object sender, EventArgs e)
        {
            StartExe(GitExeFilePath + @"\..\..\..\TortoiseGit\bin\TortoiseGitProc.exe", "/command:reflog");
        }

        private void btnOpenCommit_Click(object sender, EventArgs e)
        {
            StartExe(GitExeFilePath + @"\..\..\..\TortoiseGit\bin\TortoiseGitProc.exe", "/command:commit");
        }

        private void btnOpenSync_Click(object sender, EventArgs e)
        {
            StartExe(GitExeFilePath + @"\..\..\..\TortoiseGit\bin\TortoiseGitProc.exe", "/command:sync");
        }

        private void btnOpenPush_Click(object sender, EventArgs e)
        {
            StartExe(GitExeFilePath + @"\..\..\..\TortoiseGit\bin\TortoiseGitProc.exe", "/command:push");
        }

        #endregion
    }
}