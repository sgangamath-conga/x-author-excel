/*
 * Apttus X-Author for Excel
 * © 2014 Apttus Inc. All rights reserved.
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

namespace Apttus.XAuthor.Core
{
    public class ExceptionLogHelper
    {
        private static string LOG_FORMAT = "---------------------- {0:yyyy-MM-dd     HH:mm:ss:fff} ----------------------" + Environment.NewLine + "Message: {1}" + Environment.NewLine + Environment.NewLine + "Additional Info" + Environment.NewLine + "Message Type: {2}, Process ID: {3}, Thread#: {4}, Caller URL: {5}, User:{6}." + Environment.NewLine + Environment.NewLine;
        private static string SAVEMESSAGE_FORMAT = "---------------------- {0:yyyy-MM-dd     HH:mm:ss:fff} ----------------------" + Environment.NewLine + "App Name: {1}" + Environment.NewLine + Environment.NewLine + "Message: {2}";
        private const string LEVEL_ERROR = "ERROR";
        private const string LEVEL_WARN = "WARN";
        private const string LEVEL_INFO = "INFO";
        private const string LEVEL_DEBUG = "DEBUG";

        static Mutex mut = new Mutex();
        private static string[] m_sTraceLevelNames = { "Off", "Error", "Warning", "Info", "Debug" };
        private static int m_iTraceLevel = 0;

        private static string m_sLogFilePath = null;
        private static bool m_bUseProcessId = false;
        private static ExceptionLogHelper m_logInst = null;
        private static string m_sCallerURL = "Direct Call";
        private static string m_sUserInfo = "";
        private static Logger ExternalLogger;

        //private ExceptionLogHelper()
        //{

        //}
        public static void AssignExternalLog(Logger log)
        {
            ExternalLogger = log;
        }

        // for batch mode, merge server will pass the logger
        // and use the MS logger
        public static string LogFileFullPath
        {
            get { return Path.Combine(m_sLogFilePath, Constants.APTTUSAPPBUILDERCLIENTLOG); }
        }

        public static string SaveMessageLogFullPath
        {
            get { return Path.Combine(m_sLogFilePath, Constants.APTTUSAPPBUILDERSAVEMESSAGELOG); }
        }

        public static ExceptionLogHelper GetInstance(string sPath, string AppAddInsType)
        {
            if (ObjectManager.RuntimeMode != RuntimeMode.Batch)
            {
                m_sLogFilePath = Path.Combine(sPath, AppAddInsType);

                DirectoryInfo di = new DirectoryInfo(m_sLogFilePath);
                if (!di.Exists)
                {
                    di.Create();
                }
            }

            if (m_logInst == null)
            {
                m_logInst = new ExceptionLogHelper();
            }
            return m_logInst;
        }

        public static ExceptionLogHelper GetInstance()
        {
            if (m_logInst == null)
            {
                m_logInst = new ExceptionLogHelper();
            }
            return m_logInst;
        }

        public bool useProcessId
        {
            get
            {
                return m_bUseProcessId;
            }
            set
            {
                m_bUseProcessId = value;
            }
        }

        public static string TraceLevel
        {
            get
            {
                return m_sTraceLevelNames[m_iTraceLevel];
            }

            set
            {
                m_iTraceLevel = 0;
                for (int i = 0; i < m_sTraceLevelNames.Length; i++)
                {
                    if (m_sTraceLevelNames[i].ToUpper() == value.ToUpper())
                    {
                        m_iTraceLevel = i;
                        break;
                    }
                }
            }
        }

        public string userInfo
        {
            get
            {
                return m_sUserInfo;
            }
            set
            {
                m_sUserInfo = value;
            }
        }

        public static void ErrorLog(Exception ex, bool ShowMessage, string Message = "", string MessageWindowCaption = "")
        {

            ErrorLog(ex);
            if (ShowMessage)
                ApttusMessageUtil.ShowError((string.IsNullOrEmpty(Message) ? ex.Message : Message) +
                    Environment.NewLine + Environment.NewLine + "Error details have been logged in the X-Author log file.",
                    string.IsNullOrEmpty(MessageWindowCaption) ? Constants.ERRORMESSAGE_WINDOWTITLE : MessageWindowCaption);
        }

        public static void ErrorLog(Exception ex)
        {
            try
            {
                string sMessage = string.Format("{0}\n{1}", ex.Message, ex.StackTrace);
                if (ExternalLogger != null)
                {
                    ExternalLogger.Error(sMessage);
                    return;
                }

                if (m_iTraceLevel > 0)
                {
                    writeTrace(makeFullLogStr(LEVEL_ERROR, sMessage));
                }
            }
            catch (Exception)
            {
                
            }
        }

        public static void ErrorLog(string format, params object[] arg)
        {
            try
            {
                string sMessage = string.Format(format, arg);
                if (ExternalLogger != null)
                {
                    ExternalLogger.Error(sMessage);
                    return;
                }
                if (m_iTraceLevel > 0)
                {

                    writeTrace(makeFullLogStr(LEVEL_ERROR, sMessage));
                }
            }
            catch (Exception)
            {
                
            }
        }

        public static void WarnLog(string format, params object[] arg)
        {
            string sMessage = string.Format(format, arg);
            if (ExternalLogger != null)
            {
                ExternalLogger.Warn(sMessage);
                return;
            }
            if (m_iTraceLevel > 1)
            {
                writeTrace(makeFullLogStr(LEVEL_WARN, sMessage));
            }
        }

        public static void InfoLog(string format, params object[] arg)
        {
            string sMessage = string.Format(format, arg);
            if (ExternalLogger != null)
            {
                ExternalLogger.Warn(sMessage);
                return;
            }
            if (m_iTraceLevel > 2)
            {
                writeTrace(makeFullLogStr(LEVEL_INFO, sMessage));
            }
        }

        public static void DebugLog(string format, params object[] arg)
        {
            string sMessage = string.Format(format, arg);
            if (ExternalLogger != null)
            {
                ExternalLogger.Debug(sMessage);
                return;
            }
            if (m_iTraceLevel > 3)
            {
                writeTrace(makeFullLogStr(LEVEL_DEBUG, sMessage));
            }
        }

        public static void DebugLog(string sMessage)
        {
            if (ExternalLogger != null)
            {
                ExternalLogger.Debug(sMessage);
                return;
            }
            if (m_iTraceLevel > 3)
            {
                writeTrace(makeFullLogStr(LEVEL_DEBUG, sMessage));
            }
        }

        public static void SaveMessageLog(string saveMessage)
        {
            writeSaveMessage(String.Format(SAVEMESSAGE_FORMAT, DateTime.Now, ConfigurationManager.GetInstance.Application.Definition.Name, saveMessage));
        }

        private static string makeFullLogStr(string sLevel, string sMessage)
        {
            Thread th = Thread.CurrentThread;
            Process currentProcess = Process.GetCurrentProcess();
            string sProcId = currentProcess.Id.ToString();
            string thName = th.Name != null ? th.Name : th.GetHashCode().ToString();
            return String.Format(LOG_FORMAT, DateTime.Now, sMessage, sLevel, sProcId, thName, m_sCallerURL, m_sUserInfo);
        }

        public bool debugEnabled
        {
            get
            {
                return (m_iTraceLevel > 3);
            }
        }

        public void snapshotFile(string sSourceFileName, string sTargetName)
        {
            if (!debugEnabled)
            {
                return;
            }
            if (m_bUseProcessId)
            {
                Process currentProcess = Process.GetCurrentProcess();
                string sProcId = currentProcess.Id.ToString();
                sTargetName = string.Format("[{0}]_{1}", sProcId, sTargetName);

                //DateTime dt = DateTime.Now;
                //sTargetName = dt.ToString("[MM-dd-yyyy_HH.mm.ss]_") + sTargetName;
            }
            string sFullPath = Path.Combine(m_sLogFilePath, sTargetName);
            File.Copy(sSourceFileName, sFullPath, true);
        }

        public void dump(object obj, string sFileName)
        {
            if (obj == null || sFileName == null || sFileName.Length == 0)
            {
                return;
            }
            if (debugEnabled)
            {
                mut.WaitOne();
                try
                {
                    if (m_bUseProcessId)
                    {
                        Process currentProcess = Process.GetCurrentProcess();
                        string sProcId = currentProcess.Id.ToString();
                        sFileName = string.Format("[{0}]_{1}", sProcId, sFileName);

                        //DateTime dt = DateTime.Now;
                        //sFileName = dt.ToString("[MM-dd-yyyy_HH.mm.ss]_") + sFileName;
                    }
                    string sType = obj.GetType().ToString();
                    FileStream fs = new FileStream(Path.Combine(m_sLogFilePath, sFileName), FileMode.Create);
                    byte[] baResult = null;
                    BinaryWriter w = new BinaryWriter(fs);

                    if (sType == "System.String")
                    {
                        baResult = Encoding.UTF8.GetBytes((string)obj);
                    }
                    else if (sType == "System.Byte[]")
                    {
                        baResult = (byte[])obj;
                    }
                    else if (sType == "System.Xml.XmlElement")
                    {
                        XmlElement xmlResult = (XmlElement)obj;
                        StringWriter sw = new StringWriter();
                        XmlTextWriter xw = new XmlTextWriter(sw);
                        xw.Formatting = Formatting.Indented;
                        xmlResult.WriteTo(xw);
                        string sResult = sw.ToString();
                        baResult = Encoding.UTF8.GetBytes(sResult);
                    }
                    else if (sType == "System.Xml.XmlDocument")
                    {
                        XmlDocument xmlResult = (XmlDocument)obj;
                        StringWriter sw = new StringWriter();
                        XmlTextWriter xw = new XmlTextWriter(sw);
                        xw.Formatting = Formatting.Indented;
                        xmlResult.WriteTo(xw);
                        string sResult = sw.ToString();
                        baResult = Encoding.UTF8.GetBytes(sResult);
                    }
                    if (baResult != null)
                    {
                        w.Write(baResult);
                    }
                    w.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    ErrorLog("{0}\n{1}", ex.Message, ex.StackTrace);
                }
                finally
                {
                    mut.ReleaseMutex();
                }
            }
        }

        private static void writeTrace(string sMessage)
        {
            try
            {

                mut.WaitOne();
                string sFileName = Path.Combine(m_sLogFilePath, Constants.APTTUSAPPBUILDERLOG);
                FileInfo fi = new FileInfo(sFileName);

                if (fi.Exists)
                {

                    fi.CopyTo(Path.Combine(m_sLogFilePath, Constants.APTTUSAPPBUILDERCLIENTLOG), true);
                    fi.Delete();

                    sFileName = Path.Combine(m_sLogFilePath, Constants.APTTUSAPPBUILDERCLIENTLOG);
                    fi = new FileInfo(sFileName);

                    // the max size for the log file is 5M.
                    if (fi.Length > 5000000)
                    {

                        fi.CopyTo(sFileName + ".archive", true);
                        fi.Delete();
                    }
                }
                else
                {

                    sFileName = Path.Combine(m_sLogFilePath, Constants.APTTUSAPPBUILDERCLIENTLOG);
                    fi = new FileInfo(sFileName);

                    // the max size for the log file is 5M.
                    if (fi.Exists && fi.Length > 5000000)
                    {

                        fi.CopyTo(sFileName + ".archive", true);
                        fi.Delete();
                    }
                }

                using (StreamWriter sw = File.AppendText(sFileName))
                {

                    sw.WriteLine(sMessage);
                    sw.Close();
                }
            }
            catch (Exception)
            {

                //string sMsg = string.Format("Apttus XmlAuthorServer Error\n\n{0}", ex.Message);
                //m_ntEventLog.WriteEntry(sMsg, EventLogEntryType.Error);
            }
            finally
            {

                mut.ReleaseMutex();
            }
        }

        private static void writeSaveMessage(string sMessage)
        {
            try
            {
                mut.WaitOne();
                using (StreamWriter writer = File.AppendText(SaveMessageLogFullPath))
                {
                    writer.Write(sMessage);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                mut.ReleaseMutex();
            }
        }

        public void setCallerURL(string sURL)
        {
            m_sCallerURL = sURL;
        }


    }

    public class Logger
    {
        private object m_oLogger = null;

        public Logger(object oLogger)
        {
            m_oLogger = oLogger;
        }

        private void CallMethod(string sMethodName, string sMsg)
        {
            MethodInfo mi = m_oLogger.GetType().GetMethod(sMethodName);
            object[] parameters = new object[1];
            parameters[0] = sMsg;
            if (mi != null)
                mi.Invoke(m_oLogger, parameters);
        }

        public void Info(string sMsg)
        {
            CallMethod("info", sMsg);
        }

        public void Warn(string sMsg)
        {
            CallMethod("warn", sMsg);
        }

        public void Error(string sMsg)
        {
            CallMethod("error", sMsg);
        }

        public void Debug(string sMsg)
        {
            CallMethod("debug", sMsg);
        }
    }


}

