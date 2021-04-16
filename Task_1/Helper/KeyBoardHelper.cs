using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Task_1.Helper
{
    public static class KeyBoardHelper
    {
        /// <summary>
        /// Получить контент кнопки находящийся в шаблоне
        /// </summary>
        /// <returns>TextBlock</returns>
        public static TextBlock GetTextBlockFromButton(Button button)
        {
            var borderTemplate = button.Template.FindName("BorderTemplate", button);
            var border = ((Border)borderTemplate);

            return border.Child as TextBlock;
        }
    }
}
