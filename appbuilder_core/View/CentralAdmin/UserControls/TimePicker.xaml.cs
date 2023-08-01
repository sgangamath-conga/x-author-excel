using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// Interaction logic for ucDateTimeUpDown.xaml
    /// </summary>
    public partial class TimePicker : UserControl, INotifyPropertyChanged
    {
        #region Private Member variable
        private DateTime _currentTime = DateTime.UtcNow;
        private bool _adHours;
        private bool _addMinutes;
        private ObservableCollection<string> _amPmTypes = new ObservableCollection<string>();
        private string _displayAmPm;

        #endregion

        #region Constructors

        public TimePicker()
        {
            InitializeComponent();
            DataContext = this;
            AmPmTypes.Add("AM");
            AmPmTypes.Add("PM");
            CurrentTime = DateTime.Now;
            SelectedTime = CurrentTime.ToString("t");
        }

        #endregion

        #region Public Properties

        public ObservableCollection<string> AmPmTypes {
            get { return _amPmTypes; }
            set { _amPmTypes = value; }
        }

        public string DisplayTime {
            get { return _currentTime.ToString("t"); }
        }


        public string DisplayAmPm {
            get {
                if (_currentTime.ToLocalTime().Hour >= 0
        && _currentTime.ToLocalTime().Hour < 12)
                    _displayAmPm = AmPmTypes.FirstOrDefault(s => s.Equals("AM"));
                else
                {
                    if (_currentTime.ToLocalTime().Hour >= 12)
                    {
                        _displayAmPm = AmPmTypes.FirstOrDefault(s => s.Equals("PM"));
                    }
                }

                return _displayAmPm;
            }
            set {
                if (!value.Equals(_displayAmPm))
                {
                    //if (value.Equals("PM"))
                    //    CurrentTime = CurrentTime.ToLocalTime().AddHours(12);
                    //else
                    //{
                    //    CurrentTime = CurrentTime.ToLocalTime().AddHours(-12);
                    //}
                }
                _displayAmPm = value;
            }
        }

        public string DisplayTimeHours {
            get {
                var hours = _currentTime.Hour;
                //return hours > 12 ? (hours - 12).ToString("00") : hours.ToString("00");
                return hours.ToString("00");
            }
            set {
                var hour = 0;
                Int32.TryParse(value, out hour);
                CurrentTime = new DateTime(CurrentTime.Year, CurrentTime.Month, CurrentTime.Day, hour, Int32.Parse(DisplayTimeMinutes), 0);
                OnPropertyChanged("DisplayTime");
                OnPropertyChanged("DisplayTimeHours");
                OnPropertyChanged("DisplayTimeMinutes");
            }
        }

        public string DisplayTimeMinutes {
            get { return _currentTime.Minute.ToString("00"); }
            set {
                var minutes = 0;
                Int32.TryParse(value, out minutes);
                CurrentTime = new DateTime(CurrentTime.Year, CurrentTime.Month, CurrentTime.Day, Int32.Parse(DisplayTimeHours), minutes, 0);
                OnPropertyChanged("DisplayTime");
                OnPropertyChanged("DisplayTimeHours");
                OnPropertyChanged("DisplayTimeMinutes");
            }
        }

        public DateTime CurrentTime {
            get { return _currentTime; }
            set {
                _currentTime = value;

                OnPropertyChanged("CurrentTime");
                OnPropertyChanged("DisplayTime");
                OnPropertyChanged("DisplayTimeHours");
                OnPropertyChanged("DisplayTimeMinutes");
                OnPropertyChanged("DisplayAmPm");
                SelectedTime = value.ToString("t");
            }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty SelectedTimeProperty = DependencyProperty.Register("SelectedTime", typeof(string), typeof(TimePicker), new PropertyMetadata(default(string)));

        public string SelectedTime {
            get {
                return ((DateTime)GetValue(SelectedTimeProperty)).ToString("t");
            }
            set { SetValue(SelectedTimeProperty, value); }
        }

        #endregion

        #region Methods

        private void MinutesUpButton_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentTime = CurrentTime.AddMinutes(1);
            SelectedTime = CurrentTime.ToString("t");
        }

        private void MinutesDownButton_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentTime = CurrentTime.AddMinutes(-1);
            SelectedTime = CurrentTime.ToString("t");
        }

        private void HourUpButton_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentTime = CurrentTime.AddHours(1);
            SelectedTime = CurrentTime.ToString("t");
        }

        private void HourDownButton_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentTime = CurrentTime.AddHours(-1);
            SelectedTime = CurrentTime.ToString("t");
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void HourUpButton_MouseEnter(object sender, MouseEventArgs e)
        {
            HourUpButton.Background = null;
        }
    }
}
