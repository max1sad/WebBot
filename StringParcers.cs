using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace FootballTelegramBot
{
   public class StringParcers
    {
        public  bool inputNumberLeague ( string applicationChek,int levelAplly)
        {
            bool check = false;
            //проверка на то что введено число, когда нужно только 1 параметр и это число
            if (levelAplly == 1 || levelAplly == 2)
            {
                string pattern = @"^[0-9]{1,2}$";
                if (Regex.IsMatch(applicationChek, pattern))
                {
                    check = true;
                }
            }
            //проверка ввода по шаблону на заявку в турнир
            if (levelAplly == 3)
            {
                string pattern = @"^[\wа-яёА-ЯЁ0-9][\wа-яёА-ЯЁ0-9\s\-]+\:[а-яёА-ЯЁ\s\;]+\;{1}$";
                if (Regex.IsMatch(applicationChek, pattern))
                {
                    check = true;
                }
            }
            Console.WriteLine(check);
            return check;
        }
        public bool checkAdminText(string adminStr, int levelClick)
        {
            bool check = false;
            if (levelClick == 1)
            {
                string pattern = @"^[0-9]{1,4}\:yes\;$|^[0-9]{1,3}\:no\;$";
                if (Regex.IsMatch(adminStr, pattern))
                {
                    check = true;
                }
            }
            if (levelClick == 12)
            {
                string pattern = @"^[0-9]{1,4}\:[0-9]{1,4}\;[0-9]{2}\-[0-9]{2}\-[0-9]{2}\s{1}[0-9]{2}\:[0-9]{2}$";
                if (Regex.IsMatch(adminStr,pattern))
                {
                    check = true;
                }
            }
            return check;
        }
        //разбивает строку с заявкой на элементы и записывает в массив 
        //первый элемент это id номер лиги
        //второй элемент id номер турнира в котором учавствовать будет.
        //дальше идет 3 параметров название команды, а все остальные элементы массива это Имена игроков команды.
        public List<string> sqlParcerApply (string messageString)
        {
            int startPosition = 0;
            List<string> stringArray = new List<string>();
            for (int i = 0;i < messageString.Length; i++)
            {
                if (messageString[i] == ':')
                {
                    stringArray.Add (messageString.Substring(startPosition, i - startPosition));
                    //strCount++;
                    startPosition = i + 1;
                }
                if (messageString[i] == ';')
                {
                    stringArray.Add(messageString.Substring(startPosition, i - startPosition));
                    startPosition = i + 1;
                }
                
            }
            /*for (int i = 0;i < stringArray.Count ; i++)
            {
                Console.WriteLine(stringArray[i]);
            }*/
            return stringArray;
        }
    }
}
