using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using Task_1.Enums;

namespace Task_1.Interfaces
{
    /// <summary>
    /// Интерфейс для работы с обработкой введённых символов пользака с клавиатуры
    /// </summary>
    public interface IInputValidationRun
    {
        /// <summary>
        /// Сгенерировать рандомну строку
        /// </summary>
        List<Run> InitString(DifficultyMode difficultyMode);

        /// <summary>
        /// Проверить введённый символ. Возварт true если конец строки.
        /// </summary>
        /// <param name="textBlock">Гланвый тестбок содержащий всё строку</param>
        /// <param name="str">Введённая пользователем буква</param>
        /// <returns></returns>
        bool MoveNextChar(TextBlock textBlock, string str);

        /// <summary>
        /// Сбросить указатель в строке в 0
        /// </summary>
        void Refresh();
    }
}
