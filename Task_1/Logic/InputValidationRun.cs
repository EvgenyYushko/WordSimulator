using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Task_1.Interfaces;
using Task_1.Services;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows;
using Task_1.Enums;

namespace Task_1.Logic
{
    public class InputValidationRun : IInputValidationRun
    {
        List<Run> _listRuns;
        int _index = 0;
        MainWindow _mainWindow;

        public InputValidationRun(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public List<Run> InitString(DifficultyMode difficultyMode)
        {
            _listRuns = new List<Run>();
            string str = DefineDifficultString(difficultyMode);

            if (str == null)
                throw new Exception("Ошибка инициализации строки!");

            foreach (char c in str)
            {
                _listRuns.Add(new Run(c.ToString()));
            }

            return _listRuns;
        }

        private string DefineDifficultString(DifficultyMode difficultyMode)
        {
            string str = null;

            switch (difficultyMode)
            {
                case DifficultyMode.Easy:
                    str = StringService.GetRandomString(3, _mainWindow.IsCaseSensitive);
                    break;
                case DifficultyMode.Normal:
                    str = StringService.GetRandomString(4, _mainWindow.IsCaseSensitive);
                    break;
                case DifficultyMode.Hard:
                    str = StringService.GetRandomString(5, _mainWindow.IsCaseSensitive);
                    break;
                case DifficultyMode.Professional:
                    str = StringService.GetRandomString(6, _mainWindow.IsCaseSensitive);
                    break;
                case DifficultyMode.Expert:
                    str = StringService.GetRandomString(7, _mainWindow.IsCaseSensitive);
                    break;
            }

            return str;
        }

        public bool MoveNextChar(TextBlock textBlock, string ch)
        {
            if (!CheckInputChar(textBlock, ch))
                _mainWindow.SetFail();

            return _index != textBlock.Text.Length;
        }
        
        private bool CheckInputChar(TextBlock textBlock, string ch)
        {
            bool isEquals = false;

            var sourceString =  textBlock.Text[_index].ToString();
            isEquals = sourceString.Equals(ch);

            if (isEquals)
            {
                _listRuns[_index].Foreground = Brushes.LawnGreen;
                _listRuns[_index++].FontSize = 30;
            }
            else
            {
                _listRuns[_index].Foreground = Brushes.Red;
                _listRuns[_index].FontSize = 30;
            }

            textBlock.Inlines.Clear();
            textBlock.Inlines.AddRange(_listRuns);

            return isEquals;
        }

        public void Refresh()
        {
            _index = 0;
        }
    }
}
