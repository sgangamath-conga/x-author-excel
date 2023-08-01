using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// Interaction logic for Logs.xaml
    /// </summary>
    public partial class Logs : UserControl
    {
        private static Logs _instance;
        private ApplicationConfigManager applicationConfigManager = ApplicationConfigManager.GetInstance();
        private ApttusResourceManager resourceManager = ApttusResourceManager.GetInstance;
        private CentralUserControls LogType;
        private string CurrentLogName;
        public static Logs Instance {
            get {
                if (_instance == null)
                    _instance = new Logs();

                return _instance;
            }
        }
        private Logs()
        {
            InitializeComponent();
            if (applicationConfigManager.ApplicationSettings == null)
                applicationConfigManager.LoadApplicationSettings();
            SetCultureInfo();
        }

        private void SetCultureInfo()
        {
            gbLastLog.Header = resourceManager.GetResource("CAD_ProductLogs_LastLog");
            gbCompleteLog.Header = resourceManager.GetResource("CAD_ProductLogs_CompleteLog");
            btnOpenFile.Content = resourceManager.GetResource("CAD_ProductLogs_OpenFile");
            btnClearLog.Content = resourceManager.GetResource("CAD_ProductLogs_ClearLog");
        }

        private void UCLogs_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            string lastLogZoomLevel = GetProperty(ApttusGlobals.LastLogZoomLevel);
            string completeLogZoomLevel = GetProperty(ApttusGlobals.CompleteLogZoomLevel);

            if (!string.IsNullOrEmpty(lastLogZoomLevel))
                LastLogViewer.Zoom = double.Parse(lastLogZoomLevel);
            if (!string.IsNullOrEmpty(completeLogZoomLevel))
                CompleteLogViewer.Zoom = double.Parse(completeLogZoomLevel);
        }
        public void LoadControl(CentralUserControls logType)
        {
            try
            {
                LogType = logType;
                if (CentralAdmin.Instance.CurrentAddIn.Equals(ApplicationMode.Designer))
                {
                    if (LogType == CentralUserControls.Logs)
                        SwitchControl(Constants.DESIGNER_LOG_NAME);
                    else if (LogType == CentralUserControls.ServiceLogs)
                        SwitchControl(Constants.SERVICE_LOG_NAME);
                    else
                        SwitchControl(string.Empty);

                }
                else
                {
                    if (LogType == CentralUserControls.Logs)
                        SwitchControl(Constants.RUNTIME_LOG_NAME);
                    else if (LogType == CentralUserControls.ServiceLogs)
                        SwitchControl(Constants.SERVICE_LOG_NAME);
                    else
                        SwitchControl(string.Empty);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
            }
        }
        private void SwitchControl(string controlName)
        {
            if (LogType.Equals(CentralUserControls.Logs))
            {
                if (controlName.Equals(Constants.RUNTIME_LOG_NAME))
                    btnSwitchLogs.Content = resourceManager.GetResource("CAD_ProductLogs_SwitchToDesigner");
                else
                    btnSwitchLogs.Content = resourceManager.GetResource("CAD_ProductLogs_SwitchToRuntime");
                btnSwitchLogs.Visibility = System.Windows.Visibility.Visible;
                LoadCompleteLogs(controlName);
            }
            else
            {
                btnSwitchLogs.Visibility = System.Windows.Visibility.Collapsed;
                if (LogType.Equals(CentralUserControls.ServiceLogs))
                {
                    lblHeader.Content = resourceManager.GetResource("CAD_ServiceLogs");
                    LoadCompleteLogs(controlName);
                }
                else
                {
                    lblHeader.Content = resourceManager.GetResource("CAD_SaveMessages",true);
                    LoadCompleteLogs(string.Empty);
                }
            }
        }
        private void LoadCompleteLogs(string currentLogName)
        {

            string FullLog = string.Empty;
            string FilePath = string.Empty;
            CurrentLogName = currentLogName;
            if (LogType.Equals(CentralUserControls.Logs))
            {
                if (currentLogName.Equals(Constants.DESIGNER_LOG_NAME))
                    lblHeader.Content = resourceManager.GetResource("CAD_ProductLogs_DesignerLogs");
                else
                    lblHeader.Content = resourceManager.GetResource("CAD_ProductLogs_RuntimeLogs");

                FilePath = Path.Combine(Utils.GetApplicationTempDirectory(), currentLogName, Constants.APTTUSAPPBUILDERCLIENTLOG);
            }
            else
            {
                if (LogType.Equals(CentralUserControls.ServiceLogs))
                    FilePath = Path.Combine(Utils.GetApplicationTempDirectory(), currentLogName, Constants.APTTUSAPPBUILDERSERVICELOG);
                else
                    FilePath = ExceptionLogHelper.SaveMessageLogFullPath;
            }
            if (File.Exists(FilePath))
            {
                FullLog = File.ReadAllText(FilePath);
            }

            string lastLog = string.Empty;
            if (string.IsNullOrEmpty(FullLog))
            {
                FullLog = resourceManager.GetResource("CAD_ProductLogs_NoLogsFound");
            }
            else
            {
                lastLog = FullLog.Substring(FullLog.Substring(0, FullLog.LastIndexOf("----------------------")).LastIndexOf("----------------------") + 1);
            }
            LoadLastLog(lastLog);

            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(FullLog);
            FlowDocument document = new FlowDocument(paragraph)
            {
                FontFamily = new System.Windows.Media.FontFamily("Consolas")
            };
            CompleteLogViewer.Document = document;
        }

        private void LoadLastLog(string lastLog)
        {
            if (string.IsNullOrEmpty(lastLog))
            {
                lastLog = resourceManager.GetResource("CAD_ProductLogs_NoLogsFound");
            }
            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(lastLog);
            FlowDocument document = new FlowDocument(paragraph)
            {
                FontFamily = new System.Windows.Media.FontFamily("Consolas")
            };
            LastLogViewer.Document = document;
        }
        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            double lastLogZoomLevel = LastLogViewer.Zoom;
            double completeLogZoomLevel = CompleteLogViewer.Zoom;
            SaveProperty(ApttusGlobals.LastLogZoomLevel, lastLogZoomLevel.ToString());
            SaveProperty(ApttusGlobals.CompleteLogZoomLevel, completeLogZoomLevel.ToString());
            applicationConfigManager.ApplicationSettings.Save();
            _instance = (null);
        }

        private void btnOpenFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string FilePath = string.Empty;
                if (LogType.Equals(CentralUserControls.Logs))
                    FilePath = Path.Combine(Utils.GetApplicationTempDirectory(), CurrentLogName, Constants.APTTUSAPPBUILDERCLIENTLOG);
                else
                {
                    if (LogType.Equals(CentralUserControls.ServiceLogs))
                        FilePath = Path.Combine(Utils.GetApplicationTempDirectory(), CurrentLogName, Constants.APTTUSAPPBUILDERSERVICELOG);
                    else
                        FilePath = ExceptionLogHelper.SaveMessageLogFullPath;
                }


                if (!File.Exists(FilePath))
                    File.Create(FilePath).Close();

                System.Diagnostics.Process.Start(@FilePath);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
            }
        }

        private void btnSwitchLogs_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (CurrentLogName.Equals(Constants.DESIGNER_LOG_NAME))
                {
                    btnSwitchLogs.Content = resourceManager.GetResource("CAD_ProductLogs_SwitchToDesigner");
                    LoadCompleteLogs(Constants.RUNTIME_LOG_NAME);
                }
                else
                {
                    btnSwitchLogs.Content = resourceManager.GetResource("CAD_ProductLogs_SwitchToRuntime");
                    LoadCompleteLogs(Constants.DESIGNER_LOG_NAME);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
            }
        }

        private void btnClearLog_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (CurrentLogName.Equals(Constants.DESIGNER_LOG_NAME) || CurrentLogName.Equals(Constants.RUNTIME_LOG_NAME) || CurrentLogName.Equals(Constants.SERVICE_LOG_NAME))
                {
                    string logPath = Path.Combine(Utils.GetApplicationTempDirectory(), CurrentLogName, CurrentLogName.Equals(Constants.SERVICE_LOG_NAME) ? Constants.APTTUSAPPBUILDERSERVICELOG : Constants.APTTUSAPPBUILDERCLIENTLOG);
                    if (File.Exists(logPath))
                        File.WriteAllText(logPath, "");
                }
                else
                {
                    if (File.Exists(ExceptionLogHelper.SaveMessageLogFullPath))
                        File.WriteAllText(ExceptionLogHelper.SaveMessageLogFullPath, "");
                }
                LoadCompleteLogs(CurrentLogName);
            }
            catch (Exception ex)
            {
                ExceptionLogHelper.ErrorLog(ex, true);
            }
        }
        private void SaveProperty(string property, string value)
        {
            if (!applicationConfigManager.ApplicationSettings.AppExtendedProperties.ContainsKey(property))
                applicationConfigManager.ApplicationSettings.AppExtendedProperties.Add(property, value);
            else applicationConfigManager.ApplicationSettings.AppExtendedProperties[property] = value;
            applicationConfigManager.ApplicationSettings.Save();
        }
        private string GetProperty(string property)
        {
            string result;
            if (applicationConfigManager.ApplicationSettings.AppExtendedProperties.TryGetValue(property, out result))
            {
                return result;
            }
            result = string.Empty;
            return result;
        }
    }
}
