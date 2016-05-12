using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using FLInfocardIE.Util_Functions;


namespace FLInfocardIE
{
    public partial class MainWindow : Form, LogRecorderInterface
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The background processing thread. We do the processing in the background
        /// 'cause it's cool to do it.
        /// </summary>
        private BackgroundWorker bgWkr = null;

        private void buttonScan_Click(object sender, EventArgs e)
        {
            if (bgWkr == null)
            {                
                bgWkr = new BackgroundWorker();
                richTextBox1.Clear();
                bgWkr.DoWork += new DoWorkEventHandler(ScanIt);
                bgWkr.WorkerReportsProgress = true;
                bgWkr.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ScanItCompleted);
                bgWkr.WorkerSupportsCancellation = true;
                bgWkr.RunWorkerAsync();
                buttonScan.Enabled = false;
                buttonImportInfocards.Enabled = false;
                buttonExportInfocards.Enabled = false;
                buttonSaveChanges.Enabled = false;
            }
            else
            {
                bgWkr.CancelAsync();
            }
        }

        FLGameData gameData = new FLGameData();

        struct Infocard
        {
            public bool is_string_name;
            public string text;
            public bool has_error;
        };

        Dictionary<int, Infocard> infocards = new Dictionary<int, Infocard>();

        /// <summary>
        /// Unmanaged functions to access libraries
        /// </summary>
        static int DONT_RESOLVE_DLL_REFERENCES = 0x00000001;
        static int LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibraryExA(string lpLibFileName, int hFile, int dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int LoadStringW(IntPtr hInstance, int uID, byte[] lpBuffer, int nBufferMax);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int FreeLibrary(IntPtr hInstance);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int LockResource(int hResData);

        [DllImport("kernel32.dll")]
        static extern IntPtr FindResource(IntPtr hModule, int lpID, int lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int SizeofResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr BeginUpdateResource(string pFileName, [MarshalAs(UnmanagedType.Bool)]bool bDeleteExistingResources);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int UpdateResource(IntPtr hUpdate, int lpType, int lpName, ushort wLanguage, IntPtr lpData, int cbDataSize);

        struct ResourceDLL
        {
            public string path;
            public IntPtr hInstance;
        }

        /// <summary>
        /// Resource dlls containing strings.
        /// </summary>
        List<ResourceDLL> vDLLs = new List<ResourceDLL>();

        private void LoadLibrary(string dllPath)
        {
            IntPtr hInstance = LoadLibraryExA(dllPath, 0, DONT_RESOLVE_DLL_REFERENCES | LOAD_LIBRARY_AS_DATAFILE);
            if (hInstance != null)
            {
                ResourceDLL dll = new ResourceDLL();
                dll.path = dllPath;
                dll.hInstance = hInstance;
                vDLLs.Add(dll);
            }
        }

        void AddInfocard(int ids, bool is_string_name, string text, bool has_error)
        {
            Infocard info = new Infocard();
            info.is_string_name = is_string_name;
            info.text = text;
            info.has_error = has_error;
            infocards[ids] = info;
        }

        /// <summary>
        /// Read all infocards from dlls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScanIt(object sender, DoWorkEventArgs e)
        {
            AddLog("Loading game data...");
            infocards.Clear();
            vDLLs.Clear();

            string flExePath = Properties.Settings.Default.setFlDir + Path.DirectorySeparatorChar + "EXE";
            FLDataFile flIni = null;
            try
            {
                flIni = new FLDataFile(flExePath + Path.DirectorySeparatorChar + "Freelancer.ini", true);
            }
            catch (Exception ex)
            {
                AddLog("Error '" + ex.Message + "' when parsing '" + flExePath);
                return;
            }
            string flDataPath = Path.GetFullPath(Path.Combine(flExePath, flIni.GetSetting("Freelancer", "data path").Str(0)));
            // Load the string dlls into memory
            LoadLibrary(flExePath + Path.DirectorySeparatorChar + "resources.dll");
            foreach (FLDataFile.Setting flIniEntry in flIni.GetSettings("Resources", "DLL"))
                LoadLibrary(flExePath + Path.DirectorySeparatorChar + flIniEntry.Str(0));

            // Pull out all infocards into memory.
            for (int iDLL = 0; iDLL < vDLLs.Count; iDLL++)
            {
                IntPtr hInstance = vDLLs[iDLL].hInstance;

                AddLog("Parsing " + vDLLs[iDLL].path);

                for (int resId = 0; resId < 0x10000; resId++)
                {
                    int iIDS = (iDLL * 0x10000) + resId;

                    byte[] bufName = new byte[0x10000];
                    int len = LoadStringW(hInstance, resId, bufName, bufName.Length);
                    if (len > 0)
                    {
                        if (infocards.ContainsKey(iIDS))
                            AddLog("WARNING: duplicate ids " + iIDS);
                        string text = System.Text.Encoding.Unicode.GetString(bufName, 0, len * 2);
                        AddInfocard(iIDS, true, text, false);
                    }

                    IntPtr hFindRes = FindResource(hInstance, resId, 23);
                    if (hFindRes != IntPtr.Zero)
                    {
                        IntPtr resContent = LoadResource(hInstance, hFindRes);
                        if (resContent != IntPtr.Zero)
                        {
                            int size = SizeofResource(hInstance, hFindRes);
                            byte[] bufInfo = new byte[size];
                            Marshal.Copy(resContent, bufInfo, 0, (int)size);

                            int start = 0;
                            int end = size - 1;
                            
                            // Strip the unicode 16 little endian bom from the start
                            // and the \n\n and null at the end of the string. We will
                            // re-add these if we save changes.
                            if (size > 1 && bufInfo[0] == 0xFF && bufInfo[1] == 0xFE)
                                start += 2;

                            while (end > (start + 2) && bufInfo[end - 1] == 0x0A && bufInfo[end] == 0x00)
                                end -= 2;

                            if (end <= start)
                            {
                                AddLog("WARNING: no content for ids " + iIDS + "...discarding");
                                continue;
                            }

                            bool fix_failed = false;
                            if (end > start && bufInfo[end - 1] != 0x3E || bufInfo[end] != 0x00)
                            {
                                int end_before_fix_attempt = end;
                                AddLog("WARNING: '>' not found at end of ids " + iIDS + "...fixing");

                                // fix corrupted strings
                                if (end > (start + 1) && bufInfo[end - 1] == 0x00 && bufInfo[end] == 0x00)
                                    end -= 1;

                                // remove whitespace
                                while (end > (start + 2) && bufInfo[end - 1] == 0x20 && bufInfo[end] == 0x00)
                                    end -= 2;

                                // fix missing L
                                if (end > start && bufInfo[end - 1] == 0x44 && bufInfo[end] == 0x00)
                                {
                                    end += 2;
                                    bufInfo[end - 1] = 0x4c;
                                    bufInfo[end] = 0x00;
                                }

                                // fix missing >
                                if (end > start && bufInfo[end - 1] == 0x4c && bufInfo[end] == 0x00)
                                {
                                    end += 2;
                                    bufInfo[end - 1] = 0x3E;
                                    bufInfo[end] = 0x00;
                                }

                                if (bufInfo[end - 1] != 0x3E || bufInfo[end] != 0x00)
                                {
                                    fix_failed = true;
                                    end = end_before_fix_attempt;
                                }
                                else if (end <= start)
                                {
                                    fix_failed = true;
                                    end = end_before_fix_attempt;
                                }
                            }
                            
                            int count = end - start + 1;
                            string text = System.Text.Encoding.Unicode.GetString(bufInfo, start, count);

                            if (fix_failed)
                            {
                                AddLog("WARNING: '>' not found at end of ids " + iIDS + "...not fixed");
                                AddLog(iIDS.ToString());
                                AddLog("INFOCARD");
                                AddLog(text);
                                AddLog("");
                            }

                            if (infocards.ContainsKey(iIDS))
                            {
                                AddLog("WARNING: duplicate ids " + iIDS);
                                AddLog("WARNING: old type = " + (infocards[iIDS].is_string_name ? "NAME" : "INFOCARD"));
                                AddLog("WARNING: old text = " + infocards[iIDS].text);
                                AddLog("WARNING: new type = " + "INFOCARD");
                                AddLog("WARNING: new text = " + text);
                            }
                            AddInfocard(iIDS, false, text, fix_failed);
                        }
                    }
                }


                Infocards tempds = new Infocards();
                foreach (KeyValuePair<int, Infocard> item in infocards)
                {
                    string desc = String.Format("{0:000000}", item.Key) + (item.Value.is_string_name ? "" : "(i)");
                    tempds._Infocards.Rows.Add(new object[] { desc, item.Key, item.Value.is_string_name, item.Value.text });
                }

                e.Result = tempds;
            }

            // Unload the dlls.
            foreach (ResourceDLL dll in vDLLs)
                FreeLibrary(dll.hInstance);

            AddLog("Loaded " + infocards.Count + " infocards");
        }

        /// <summary>
        /// Called when scan it background processing thread completes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScanItCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBoxCardInfo.Clear();
            richTextBoxRawCard.Clear();
            richTextBoxFormattedCard.Clear();

            infocards1._Infocards.Clear();
            infocards1._Infocards.Merge(((Infocards)e.Result)._Infocards);

            buttonScan.Enabled = true;
            buttonImportInfocards.Enabled = true;
            buttonExportInfocards.Enabled = true;
            buttonSaveChanges.Enabled = true;

            bgWkr = null;
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Add an error entry to the log file.
        /// </summary>
        /// <param name="details">A human readable description.</param>
        /// <param name="accountID">If the log entry is related to a file operation 
        /// then this parameter contains a path to the directory containing the file.</param>
        public void AddLog(string details)
        {
            if (InvokeRequired)
            {

                UpdateUIAddLogDelegate updateUI = new UpdateUIAddLogDelegate(UpdateUIAddLog);
                this.Invoke(updateUI, new object[] { details });
            }
            else
            {
                UpdateUIAddLog(details);
            }
        }

        /// <summary>
        /// A delegate that always runs in the UI thread. This updates the database
        /// which in turn updates the log table.
        /// </summary>
        /// <param name="details"></param>
        /// <param name="dirPath"></param>
        delegate void UpdateUIAddLogDelegate(string details);
        protected void UpdateUIAddLog(string details)
        {
            richTextBox1.AppendText(details + "\r\n");
        }

        /// <summary>
        /// Import infocards from text file. This is a fussy parser to detect errors in the infocard
        /// import
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonImportInfocards_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {                
                BackgroundWorker bg = new BackgroundWorker();
                bg.DoWork += new DoWorkEventHandler(bg_ImportWork);
                bg.RunWorkerAsync(dialog.FileName);
            }
        }

        void bg_ImportWork(object sender, DoWorkEventArgs e)
        {
            using (StreamReader sr = File.OpenText((string)e.Argument))
            {
                HashSet<int> loadedIDSs = new HashSet<int>();
                int iIDS = 0;
                int state = 0;
                int lineNumber = 0;
                int imported_unchanged_infocards = 0;
                int imported_new_infocards = 0;
                int imported_changed_infocards = 0;
                int imported_changed_whitespace_infocards = 0;

                for (string strLine = sr.ReadLine(); strLine != null; strLine = sr.ReadLine())
                {
                    lineNumber++;
                    strLine = strLine.Trim();

                    switch (state)
                    {
                        case 0: // expecting IDS, comment or empty lines
                            if (strLine.StartsWith(";") || strLine.Length == 0)
                            {
                                continue;
                            }

                            if (!Int32.TryParse(strLine, out iIDS))
                            {
                                AddLog("WARNING: IDS number not found at line " + lineNumber);
                                continue;
                            }

                            if (loadedIDSs.Contains(iIDS))
                            {
                                AddLog("WARNING: duplicate IDS number found at line " + lineNumber + " duplicate IDS = " + iIDS);
                            }
                            loadedIDSs.Add(iIDS);

                            state = 1;
                            break;

                        case 1: // expecting NAME or INFOCARD
                            if (strLine.StartsWith(";") || strLine.Length == 0)
                            {
                                AddLog("WARNING: comment or empty line when expecting NAME or INFOCARD at line " + lineNumber);
                                continue;
                            }

                            if (strLine == "NAME")
                            {
                                state = 2;
                            }
                            else if (strLine == "INFOCARD")
                            {
                                state = 3;
                            }
                            else
                            {
                                AddLog("WARNING: NAME or INFOCARD not found at line " + lineNumber);
                                continue;
                            }

                            break;

                        case 2: // expecting NAME text
                            if (strLine.StartsWith(";"))
                            {
                                AddLog("WARNING: comment when expecting NAME at line " + lineNumber);
                            }
                            else if (strLine.StartsWith("<"))
                            {
                                AddLog("WARNING: xml found when expecting NAME text at line " + lineNumber);
                            }
                            else
                            {
                                if (strLine.Length == 0)
                                {
                                    AddLog("WARNING: empty line when expecting NAME text at line " + lineNumber);
                                }

                                // Replace \n with 0x0A
                                strLine = strLine.Replace("\\n", "\n");

                                if (!infocards.ContainsKey(iIDS))
                                {
                                    AddLog("");
                                    AddLog("New NAME " + iIDS);
                                    AddLog("text = " + strLine);

                                    AddInfocard(iIDS, true, strLine, false);
                                    imported_new_infocards++;
                                }

                                else if (string.Compare(infocards[iIDS].text, strLine, false) != 0)
                                {
                                    // is this a whitespace only change - don't report it.
                                    if (string.Compare(infocards[iIDS].text.Trim(), strLine, false) != 0)
                                    {
                                        AddLog("");
                                        AddLog("Changed NAME " + iIDS);
                                        AddLog("from = " + infocards[iIDS].text);
                                        AddLog("to = " + strLine);

                                        AddInfocard(iIDS, true, strLine, false);

                                        imported_changed_infocards++;
                                    }
                                    // changing type
                                    else if (!infocards[iIDS].is_string_name)
                                    {
                                        AddLog("");
                                        AddLog("Changed TYPE TO NAME " + iIDS);
                                        AddLog("from = " + infocards[iIDS].text);
                                        AddLog("to = " + strLine);

                                        AddInfocard(iIDS, true, strLine, false);

                                        imported_changed_infocards++;
                                    }
                                    else
                                    {
                                        imported_changed_whitespace_infocards++;
                                    }
                                }
                                else
                                {
                                    imported_unchanged_infocards++;
                                }

                                state = 0;
                            }
                            break;

                        case 3: // expecting INFOCARD text
                            if (strLine.StartsWith(";") || strLine.Length == 0)
                            {
                                AddLog("WARNING: comment or empty line when expecting INFOCARD at line " + lineNumber);
                            }
                            else if (!strLine.StartsWith("<"))
                            {
                                AddLog("WARNING: xml not found when expecting INFOCARD text at line " + lineNumber);
                            }
                            else
                            {
                                if (!infocards.ContainsKey(iIDS))
                                {
                                    AddLog("");
                                    AddLog("New INFOCARD " + iIDS);
                                    AddLog("text = " + strLine);

                                    AddInfocard(iIDS, false, strLine, false);

                                    imported_new_infocards++;
                                }
                                else if (string.Compare(infocards[iIDS].text, strLine, false) != 0)
                                {
                                    // is this a whitespace only change - don't report it.
                                    if (string.Compare(infocards[iIDS].text.Trim(), strLine, false) != 0)
                                    {
                                        AddLog("");
                                        AddLog("Changed INFOCARD " + iIDS);
                                        AddLog("from = " + infocards[iIDS].text);
                                        AddLog("to = " + strLine);

                                        AddInfocard(iIDS, false, strLine, false);

                                        imported_changed_infocards++;
                                    }
                                    // changing type
                                    else if (!infocards[iIDS].is_string_name)
                                    {
                                        AddLog("");
                                        AddLog("Changed TYPE TO INFOCARD " + iIDS);
                                        AddLog("from = " + infocards[iIDS].text);
                                        AddLog("to = " + strLine);

                                        AddInfocard(iIDS, false, strLine, false);

                                        imported_changed_infocards++;
                                    }
                                    else
                                    {
                                        imported_changed_whitespace_infocards++;
                                    }
                                }
                                else
                                {
                                    imported_unchanged_infocards++;
                                }

                                state = 0;
                            }
                            break;
                    }
                }

                AddLog("");
                AddLog("Imported unchanged infocards: " + imported_unchanged_infocards);
                AddLog("Imported new infocards: " + imported_new_infocards);
                AddLog("Imported changed infocards: " + imported_changed_infocards);
                AddLog("Imported changed whitespace infocards: " + imported_changed_whitespace_infocards);
                AddLog("Done");

                sr.Close();
            }

            
        }

        /// <summary>
        /// Save changes to DLLs based on imported text file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSaveChanges_Click(object sender, EventArgs e)
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += new DoWorkEventHandler(bg_SaveWork);
            bg.RunWorkerAsync();
        }

        void bg_SaveWork(object sender, DoWorkEventArgs e)
        {
            for (int iDLL = 0; iDLL < vDLLs.Count; iDLL++)
            {
                // skip resources.dll
                if (iDLL == 0)
                    continue;

                try
                {
                    AddLog("Making backup " + vDLLs[iDLL].path + ".orig");
                    File.Copy(vDLLs[iDLL].path, vDLLs[iDLL].path + ".orig", true);

                    AddLog("Writing " + vDLLs[iDLL].path);
                    IntPtr hUpdate = BeginUpdateResource(vDLLs[iDLL].path, true);
                    if (hUpdate == IntPtr.Zero)
                        throw new Exception("Unable to open file " + vDLLs[iDLL].path + " errcode = " + Marshal.GetLastWin32Error());

                    for (int resId = 0; resId < 0x10000; resId++)
                    {
                        int iIDS = (iDLL * 0x10000) + resId;
                        // If we have an infocard xml resource then write it out.
                        if (infocards.ContainsKey(iIDS)
                            && !infocards[iIDS].is_string_name)
                            //&& !infocards[iIDS].has_error)
                        {
                            UnicodeEncoding encoding = new System.Text.UnicodeEncoding();
                            byte[] textbuf = encoding.GetBytes(infocards[iIDS].text);

                            byte[] buf = new byte[textbuf.Length + 6];
                            Buffer.BlockCopy(textbuf, 0, buf, 2, textbuf.Length);
                            buf[0] = 0xFF;
                            buf[1] = 0xFE;
                            buf[buf.Length - 4] = 0x0A;
                            buf[buf.Length - 3] = 0x00;
                            buf[buf.Length - 2] = 0x0A;
                            buf[buf.Length - 1] = 0x00;

                            IntPtr ptr = Marshal.AllocHGlobal(buf.Length);
                            Marshal.Copy(buf, 0, ptr, buf.Length);
                            if (UpdateResource(hUpdate, 23, resId, (1 << 10 | 0x09), ptr, buf.Length) == 0)
                                throw new Exception("Update resource failed on file " + vDLLs[iDLL].path + " errcode = " + Marshal.GetLastWin32Error());
                            Marshal.FreeHGlobal(ptr);
                        }

                        // Write to the string table.
                        if ((resId % 16) == 0)
                        {
                            // Build the string buffer which might contain up to 16 unicode strings
                            // with the length in the first two bytes.
                            int pos = 0;
                            byte[] buf = new byte[0x10000];
                            for (int subId = 0; subId < 16; subId++)
                            {
                                if (infocards.ContainsKey(iIDS + subId)
                                    && infocards[iIDS + subId].is_string_name)
                                    //&& !infocards[iIDS + subId].has_error)
                                {
                                    UnicodeEncoding encoding = new System.Text.UnicodeEncoding(false, false, true);
                                    string text = infocards[iIDS + subId].text;

                                    byte[] textbuf = encoding.GetBytes(text);
                                    buf[pos++] = (byte)(text.Length & 0xFF);
                                    buf[pos++] = (byte)(text.Length >> 8);
                                    Buffer.BlockCopy(textbuf, 0, buf, pos, textbuf.Length);
                                    pos += textbuf.Length;
                                }
                                else
                                {
                                    buf[pos++] = 0x00;
                                    buf[pos++] = 0x00;
                                }
                            }
                            
                            if (pos > 32) // non empty resource
                            {
                                IntPtr ptr = Marshal.AllocHGlobal(pos);
                                Marshal.Copy(buf, 0, ptr, pos);
                                if (UpdateResource(hUpdate, 6, (resId / 16) + 1, (1 << 10 | 0x09), ptr, pos) == 0)
                                    throw new Exception("Update resource failed on file " + vDLLs[iDLL].path + " errcode = " + Marshal.GetLastWin32Error());
                                Marshal.FreeHGlobal(ptr);
                            }
                        }
                    }
                    if (EndUpdateResource(hUpdate, false) == 0)
                        throw new Exception("End update resource failed on file " + vDLLs[iDLL].path + " errcode = " + Marshal.GetLastWin32Error());
                }
                catch (Exception ex)
                {
                    AddLog("ERROR: " + ex.Message);
                    try { File.Copy(vDLLs[iDLL].path + ".orig", vDLLs[iDLL].path, true); }
                    catch { }
                }
            }
            AddLog("Done");

        }

        /// <summary>
        /// Export all infocards to text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonExportInfocards_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                BackgroundWorker bg = new BackgroundWorker();
                bg.DoWork += new DoWorkEventHandler(bg_ExportWork);
                bg.RunWorkerAsync(dialog.FileName);
            }
        }
        
        void bg_ExportWork(object sender, DoWorkEventArgs e)
        {
            using (StreamWriter fs = File.CreateText((string)e.Argument))
            {
                foreach (KeyValuePair<int, Infocard> item in infocards)
                {
                    if (item.Value.is_string_name)
                    {
                        fs.WriteLine();
                        fs.WriteLine("{0}", item.Key);
                        fs.WriteLine("NAME");
                        string text = item.Value.text.Replace("\n", "\\n");
                        fs.WriteLine("{0}", text);
                    }
                    else
                    {
                        fs.WriteLine();
                        fs.WriteLine("{0}", item.Key);
                        fs.WriteLine("INFOCARD");
                        string text = item.Value.text.Replace("\n", "\\n");
                        fs.WriteLine("{0}", text);
                    }
                }
                fs.Close();
            }
        }

        /// <summary>
        /// Escape a string for an expression.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EscapeLikeExpressionString(string value)
        {
            string escapedText = value;
            escapedText = escapedText.Replace("[", "[[]");
            //filter = filter.Replace("]", "[]]");
            escapedText = escapedText.Replace("%", "[%]");
            escapedText = escapedText.Replace("*", "[*]");
            escapedText = escapedText.Replace("'", "''");
            return escapedText;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            string filterText = FLUtility.EscapeLikeExpressionString(textBoxSearch.Text);
            if (filterText == "")
            {
                infocardsBindingSource.Filter = null;
                return;
            }
            string expr = "(desc LIKE '%" + filterText + "%')";
            expr += " OR (text LIKE '%" + filterText + "%')";
            infocardsBindingSource.Filter = expr;
        }

        /// <summary>
        /// Hack FL formatted xml into a RTF format.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string FLXmlToRtf(string xml)
        {
            int xmlEnd = xml.IndexOf("</RDP>");
            if (xmlEnd >= 0)
                xml = xml.Substring(0, xmlEnd);
            xml = xml.Replace("<JUST loc=\"center\"/>", "\\qc ");
            xml = xml.Replace("<JUST loc=\"left\"/>", "\\pard ");
            xml = xml.Replace("<TRA data=\"1\" mask=\"1\" def=\"-2\"/>", "\\b ");
            xml = xml.Replace("<TRA data=\"1\" mask=\"1\" def=\"0\"/>", "\\b0 ");
            xml = xml.Replace("<TRA data=\"0\" mask=\"1\" def=\"-1\"/>", "\\b0 ");
            xml = xml.Replace("<PARA/>", "\\par ");
            xml = System.Text.RegularExpressions.Regex.Replace(xml, "<[^<>]*>", "");
            xml = xml.Replace("&gt;", ">");
            xml = xml.Replace("&lt;", "<");
            xml = xml.Trim();
            return xml;
        }

        private void dataGridViewIDS_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataGridViewRow row = dataGridViewIDS.Rows[e.RowIndex];
                textBoxCardInfo.Text = row.Cells[0].Value.ToString();
                richTextBoxRawCard.Text = row.Cells[1].Value.ToString();

                richTextBoxFormattedCard.Clear();
                richTextBoxFormattedCard.AppendText("@@@INSERTED_RTF_CODE_HACK@@@");                
                string rtf = FLUtility.FLXmlToRtf(row.Cells[1].Value.ToString());
                richTextBoxFormattedCard.Rtf = richTextBoxFormattedCard.Rtf.Replace("@@@INSERTED_RTF_CODE_HACK@@@", rtf);
            }
            catch
            {
                textBoxCardInfo.Clear();
                richTextBoxRawCard.Clear();
                richTextBoxFormattedCard.Clear();
            }
        }

        private void buttonAbout_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = Properties.Resources.Readme;
        }
    }
}
