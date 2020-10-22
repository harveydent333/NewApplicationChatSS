using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AppChatSS.Infrastucture
{
    public class TimeComputer
    {
        /// <summary>
        /// Метод вычисляет дату окончания блокировки/мута/изгнания
        /// </summary>
        public static DateTime CalculateUnlockDate(String command, String timeValueInString)
        {
            Int32 blockTime;
            Int32.TryParse(Regex.Match(command, timeValueInString).Groups[1].Value, out blockTime);

            return DateTime.Now.AddMinutes(blockTime);
        }
    }
}
