using System;
using System.Text.RegularExpressions;

namespace NewAppChatSS.BLL.Infrastructure
{
    public static class TimeComputer
    {
        /// <summary>
        /// Метод вычисляет дату окончания блокировки/мута/изгнания
        /// </summary>
        public static DateTime CalculateUnlockDate(string command, string timeValueInString)
        {
            int blockTime;
            int.TryParse(Regex.Match(command, timeValueInString).Groups[1].Value, out blockTime);

            return DateTime.Now.AddMinutes(blockTime);
        }
    }
}
