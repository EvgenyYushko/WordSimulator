using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using Task_1.Enums;
using Task_1.Interfaces;
using Task_1.Logic;
using static Task_1.Helper.KeyBoardHelper;

namespace Task_1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isCapsLock;
        private bool _isRun;
        public event EventHandler CapsLockChanged;
        public event EventHandler AppRunChanged;
        Button _currentButton = null;
        IInputValidationRun _inputValidationRun;
        private int _fails;
        DifficultyMode _difficultyMode;
        bool _isCaseSensitive;
        DispatcherTimer timer;
        int _timeLeftDefault;
        int timeLeftCurrent;

        public MainWindow()
        {
            InitializeComponent();
            CapsLockChanged += Button_CapsLockChanged;
            AppRunChanged += Button_AppRunChanged;
            SetEventHandler(StackPanelKeyBoard);

            _inputValidationRun = new InputValidationRun(this);

            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lbTime.Content = timeLeftCurrent--.ToString();

            if (timeLeftCurrent < 0)
            {
                MessageBox.Show($"You are the lose! \n\nYou result: Fails - {_fails}", "Losing!", MessageBoxButton.OK);
                Run = false;
            }
        }

        /// <summary>
        /// Чувствительность к регистру
        /// </summary>
        public bool IsCaseSensitive
        {
            get { return _isCaseSensitive; }
            set { _isCaseSensitive = value; }
        }

        public TextBlock TbHeader
        {
            get { return tbHeader; }
            set { tbHeader = value; }
        }

        /// <summary>
        /// Запустить симулятор?
        /// </summary>
        public bool Run
        {
            get { return _isRun; }
            set
            {
                _isRun = value;
                OnButtonAppRunChanged();
            }
        }

        /// <summary>
        /// Заглвные буквы?
        /// </summary>
        public bool CapsLock
        {
            get { return _isCapsLock; }
            set
            {
                _isCapsLock = value;
                OnButtonCapsLockChanged();
            }
        }

        private void RefreshApp()
        {
            TbHeader.Text = string.Empty;
            _inputValidationRun.Refresh();
            lbFails.Content = _fails = 0;
            lbTime.Content = _timeLeftDefault.ToString();
        }

        public void SetFail()
        {
            lbFails.Content = ++_fails;
        }

        private void SetCapsLock(object obj)
        {
            if (obj is StackPanel)
            {
                var sp = obj as StackPanel;

                if (sp != null)
                {
                    foreach (UIElement child in sp.Children)
                    {
                        SetCapsLock(child);
                    }
                }
            }

            if (obj is Button)
            {
                var textBlock = GetTextBlockFromButton((Button)obj);
                var text = textBlock.Text;

                if (text.Length == 1 && char.IsLetter(text[0]))
                {
                    if (char.IsLower(text[0]))
                    {
                        textBlock.Text = text.ToUpper();
                    }
                    else
                    {
                        textBlock.Text = text.ToLower();
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик события запуска симулятроа
        /// </summary>
        private void Button_AppRunChanged(object sender, EventArgs e)
        {
            btStart.IsEnabled = slDifficulty.IsEnabled = cbCaseSensitive.IsEnabled = !Run;
            timeLeftCurrent = _timeLeftDefault;
            timer.Start();

            if (Run)
            {
                tbHeader.Inlines.AddRange(_inputValidationRun.InitString(_difficultyMode));
            }
            else
            {
                timer.Stop();
                RefreshApp();
            }
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            Run = true;
        }

        private void btStop_Click(object sender, RoutedEventArgs e)
        {
            Run = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!Run || sender == null)
                return;

            var textBlock = GetTextBlockFromButton((Button)sender);

            if(textBlock.Text == "Shift")
                return;

            if (!_inputValidationRun.MoveNextChar(tbHeader, textBlock.Text == "Space"  ? " " : textBlock.Text))
            {
                timer.Stop();
                MessageBox.Show($"You are the winner! \n\nYou result: Fails - {_fails} \n\nElapsed time - {_timeLeftDefault - timeLeftCurrent}", "Victory!", MessageBoxButton.OK);
                Run = false;
            }
        }

        /// <summary>
        /// Обработчик события подсветки кнопки
        /// </summary>
        /// <param name="button">Нажатая кнопка</param>
        /// <param name="isHighLight">Выделить кнопку?</param>
        private static void HighLightButton(Button button, bool isHighLight)
        {
            if (button == null)
                return;

            // тут нужно отрефакторить, вынести в GetTextBlockFromButton предварительно отрефакторитв и его :)
            var borderTemplate = button.Template.FindName("BorderTemplate", button);
            ((Border)borderTemplate).Effect = isHighLight ? new DropShadowEffect() : null;
        }

        /// <summary>
        /// Установить обрабочик события нажатия на кнопку для кнопок находящихся в StackPanel
        /// </summary>
        /// <param name="obj">Объект содержащий кнопки</param>
        private void SetEventHandler(object obj)
        {
            if (obj is StackPanel)
            {
                var sp = obj as StackPanel;

                if (sp != null)
                {
                    foreach (UIElement child in sp.Children)
                    {
                        SetEventHandler(child);
                    }
                }
            }

            if (obj is Button)
            {
                ((Button)obj).Click += Button_Click;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            PushButton(e, true);

            switch (e.Key)
            {
                case Key.LeftShift:
                case Key.RightShift:
                    CapsLock = true;
                    break;
                case Key.CapsLock:
                    CapsLock = !CapsLock;
                    HighLightButton(btCapsLock, CapsLock);
                    break;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            PushButton(e, false);

            switch (e.Key)
            {
                case Key.LeftShift:
                case Key.RightShift:
                    CapsLock = false;
                    break;
            }

            Button_Click(_currentButton, e);
        }

        /// <summary>
        /// Нажать на кнопку
        /// </summary>
        /// <param name="isPush">нажата/отпущена кнопка?</param>
        private void PushButton(KeyEventArgs e, bool isPush)
        {
            _currentButton = null;

            switch (e.SystemKey)
            {
                case Key.LeftAlt:
                    _currentButton = btAltLeft;
                    break;
                case Key.RightAlt:
                    _currentButton = btAltRight;
                    break;
            }

            switch (e.Key)
            {
                case Key.D0:
                    _currentButton = bt0;
                    break;
                case Key.D1:
                    _currentButton = bt1;
                    break;
                case Key.D2:
                    _currentButton = bt2;
                    break;
                case Key.D3:
                    _currentButton = bt3;
                    break;
                case Key.D4:
                    _currentButton = bt4;
                    break;
                case Key.D5:
                    _currentButton = bt5;
                    break;
                case Key.D6:
                    _currentButton = bt6;
                    break;
                case Key.D7:
                    _currentButton = bt7;
                    break;
                case Key.D8:
                    _currentButton = bt8;
                    break;
                case Key.D9:
                    _currentButton = bt9;
                    break;
                case Key.OemMinus:
                    _currentButton = btMinus;
                    break;
                case Key.OemPlus: // почему то "равно" определяет как "плюс"
                    _currentButton = btEqually;
                    break;
                case Key.Tab:
                    _currentButton = btTab;
                    break;
                case Key.A:
                    _currentButton = btA;
                    break;
                case Key.B:
                    _currentButton = btB;
                    break;
                case Key.C:
                    _currentButton = btC;
                    break;
                case Key.D:
                    _currentButton = btD;
                    break;
                case Key.E:
                    _currentButton = btE;
                    break;
                case Key.F:
                    _currentButton = btF;
                    break;
                case Key.G:
                    _currentButton = btG;
                    break;
                case Key.H:
                    _currentButton = btH;
                    break;
                case Key.I:
                    _currentButton = btI;
                    break;
                case Key.J:
                    _currentButton = btJ;
                    break;
                case Key.K:
                    _currentButton = btK;
                    break;
                case Key.L:
                    _currentButton = btL;
                    break;
                case Key.M:
                    _currentButton = btM;
                    break;
                case Key.N:
                    _currentButton = btN;
                    break;
                case Key.O:
                    _currentButton = btO;
                    break;
                case Key.P:
                    _currentButton = btP;
                    break;
                case Key.Q:
                    _currentButton = btQ;
                    break;
                case Key.R:
                    _currentButton = btR;
                    break;
                case Key.S:
                    _currentButton = btS;
                    break;
                case Key.T:
                    _currentButton = btT;
                    break;
                case Key.U:
                    _currentButton = btU;
                    break;
                case Key.V:
                    _currentButton = btV;
                    break;
                case Key.W:
                    _currentButton = btW;
                    break;
                case Key.X:
                    _currentButton = btX;
                    break;
                case Key.Y:
                    _currentButton = btY;
                    break;
                case Key.Z:
                    _currentButton = btZ;
                    break;
                case Key.Oem4:
                    _currentButton = btLeftBracket;
                    break;
                case Key.Oem5:
                    _currentButton = btSlashLeft;
                    break;
                case Key.Oem6:
                    _currentButton = btRightBracket;
                    break;
                case Key.Enter:
                    _currentButton = btEnter;
                    break;
                case Key.LeftShift:
                    _currentButton = btShiftLeft;
                    break;
                case Key.RightShift:
                    _currentButton = btShiftRight;
                    break;
                case Key.LeftCtrl:
                    _currentButton = btCtrlLeft;
                    break;
                case Key.RightCtrl:
                    _currentButton = btCtrlRight;
                    break;
                case Key.LWin:
                    _currentButton = btWinLeft;
                    break;
                case Key.RWin:
                    _currentButton = btWinRight;
                    break;
                case Key.OemComma:
                    _currentButton = btComma;
                    break;
                case Key.OemPeriod:
                    _currentButton = btDot;
                    break;
                case Key.Oem1:
                    _currentButton = btSemicolon;
                    break;
                case Key.Oem2:
                    _currentButton = btSlashRight;
                    break;
                case Key.Oem3:
                    _currentButton = btUpLeftComma;
                    break;
                case Key.Oem7:
                    _currentButton = btUpComma;
                    break;
                case Key.Space:
                    _currentButton = btSpace;
                    break;
                case Key.Back:
                    _currentButton = btBackSpace;
                    break;
            }

            HighLightButton(_currentButton, isPush);
        }

        protected virtual void OnButtonCapsLockChanged()
        {
            CapsLockChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnButtonAppRunChanged()
        {
            AppRunChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Button_CapsLockChanged(object sender, EventArgs e)
        {
            SetCapsLock(StackPanelKeyBoard);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ((Slider)sender).SelectionEnd = e.OldValue;

            _difficultyMode = (DifficultyMode)(int)e.NewValue;
            lbDifficulty.Content = ((int)_difficultyMode).ToString();

            switch (_difficultyMode)
            {
                case DifficultyMode.Easy:
                    this.Background = Brushes.White;
                    lbTime.Content = _timeLeftDefault = 90;
                    break;
                case DifficultyMode.Normal:
                    this.Background = new SolidColorBrush(Color.FromRgb(173, 218, 254));
                    lbTime.Content = _timeLeftDefault = 70;
                    break;
                case DifficultyMode.Hard:
                    this.Background = new SolidColorBrush(Color.FromRgb(255, 220, 172));
                    lbTime.Content = _timeLeftDefault = 50;
                    break;
                case DifficultyMode.Professional:
                    this.Background = new SolidColorBrush(Color.FromRgb(255, 181, 172));
                    lbTime.Content = _timeLeftDefault = 30;
                    break;
                case DifficultyMode.Expert:
                    this.Background = new SolidColorBrush(Color.FromRgb(254, 173, 242));
                    lbTime.Content = _timeLeftDefault = 20;
                    break;
            }
        }

        private void cbCaseSensitive_Click(object sender, RoutedEventArgs e)
        {
            _isCaseSensitive = cbCaseSensitive.IsChecked == true;
        }
    }
}
