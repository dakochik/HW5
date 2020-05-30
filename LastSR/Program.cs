using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace LastSR
{
    /// <summary>
    /// Кочик Дмитрий
    /// БПИ1999
    /// </summary>
 
    class Program
    {
        /// <summary>
        /// Словарь для перевода символов.
        /// </summary>
        static Dictionary<char, string> myTranslator = new Dictionary<char, string> // Экспериментальным путем было выяснено, что
        {                                                                           // использование большого словаря работает быстрее,
            { 'A', "А"},                                                            // чем использование маленького + проверка.
            { 'B', "Б"},
            { 'V', "В"},
            { 'G', "Г"},
            { 'D', "Д"},
            { 'E', "Е"},
            { 'J', "Ж"},
            { 'Z', "З"},
            { 'I', "И"},
            { 'K', "К"},
            { 'L', "Л"},
            { 'M', "М"},
            { 'N', "Н"},
            { 'O', "О"},
            { 'P', "П"},
            { 'R', "Р"},
            { 'S', "С"},
            { 'T', "Т"},
            { 'U', "У"},
            { 'F', "Ф"},
            { 'H', "Х"},
            { 'C', "Ц"},
            { 'Q', "КУ"},
            { 'W', "У"},
            { 'X', "КС"},
            { 'Y', "Й"},
            { 'a', "а"},
            { 'b', "б"},
            { 'v', "в"},
            { 'g', "г"},
            { 'd', "д"},
            { 'e', "е"},
            { 'j', "ж"},
            { 'z', "з"},
            { 'i', "и"},
            { 'k', "к"},
            { 'l', "л"},
            { 'm', "м"},
            { 'n', "н"},
            { 'o', "о"},
            { 'p', "п"},
            { 'r', "р"},
            { 's', "с"},
            { 't', "т"},
            { 'u', "у"},
            { 'f', "ф"},
            { 'h', "х"},
            { 'c', "ц"},
            { 'q', "ку"},
            { 'w', "у"},
            { 'x', "кс"},
            { 'y', "й"}
        };

        /// <summary>
        /// Путь к файлу с книгами.
        /// </summary>
        static string path = "../../Книги/";

        /// <summary>
        /// Название перевода книги из интернета.
        /// </summary>
        static string secondTaskBookPath = "../../Книги/new_book_from_web.txt";

        /// <summary>
        /// Ссылка на книгу из интернета.
        /// </summary>
        static string webPath = "https://www.gutenberg.org/files/1342/1342-0.txt";

        /// <summary>
        /// Метод для медленного чтения, перевода и записи книги.
        /// Одновременно читает и записывает книгу.
        /// </summary>
        /// <param name="booksName"></param>
        static void TranslateTheBook(string booksName)
        {
            StringBuilder bookBuilder = new StringBuilder();
            int sign = 0;
            int totalEnS = 0;
            int totalRuS = 0;
            string appendThis;
            Stopwatch timer = new Stopwatch();
            try
            {
                using (StreamReader streamR = new StreamReader(path + booksName))
                {
                    using (StreamWriter streamW = new StreamWriter(path + "new_" + booksName))
                    {
                        timer.Start();
                        sign = streamR.Read();
                        while (sign != -1)
                        {
                            ++totalEnS; // Входные данные - всегда 1 символ
                            if (char.IsLetter((char)sign) && myTranslator.ContainsKey((char)sign))
                            {
                                appendThis = myTranslator[(char)sign];
                                streamW.Write(appendThis);
                                totalRuS += appendThis.Length; // Выходные данные могут быть двумя или одним символом
                            }
                            else
                            {
                                streamW.Write((char)sign);
                                ++totalRuS;
                            }
                            streamW.Flush();
                            sign = streamR.Read();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UsualExceptionMeth(e.Message);
                return;
            }
            timer.Stop();
            PrintInfoAboutBook(booksName, totalEnS, totalRuS, timer.Elapsed);
        }

        /// <summary>
        /// Метод для быстрого чтения, перевода и записи книги.
        /// </summary>
        /// <param name="booksName"> название книги</param>
        static void TranslateTheBookFaster(string booksName)
        {
            StringBuilder buildMyBook = new StringBuilder();
            int totalEnS = 0;
            int totalRuS = 0;
            string appendThis;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            try
            {
                using (StreamReader reader = new StreamReader(path + booksName))
                {
                    int sign = reader.Read();
                    do
                    {
                        ++totalEnS;
                        if (char.IsLetter((char)sign) && myTranslator.ContainsKey((char)sign))
                        {
                            appendThis = myTranslator[(char)sign];
                            buildMyBook.Append(appendThis);
                            totalRuS += appendThis.Length;
                        }
                        else
                        {
                            buildMyBook.Append((char)sign);
                            ++totalRuS;
                        }
                        sign = reader.Read();
                    }
                    while (sign != -1);
                }
                File.WriteAllText(path + "new_" + booksName, buildMyBook.ToString());
            }
            catch (Exception e)
            {
                UsualExceptionMeth(e.Message);
                return;
            }
            timer.Stop();
            PrintInfoAboutBook(booksName, totalEnS, totalRuS, timer.Elapsed);
        }

        /// <summary>
        /// Метод, для активации выполнения синхронного и асинхронного перевода.
        /// </summary>
        static void FirstTaskMeth()
        {
            string[] allPaths;
            try
            {
                allPaths = Directory.GetFiles(path);
            }
            catch (Exception e)
            {
                UsualExceptionMeth(e.Message);
                return;
            }
            int countBooks = 0;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            foreach (string str in allPaths)
            {
                if (!str.Contains("new_"))
                {
                    ++countBooks;
                    TranslateTheBook(str.Replace(path, "")); // Поочереди выполняем перевод.
                }
            }
            timer.Stop();
            Console.WriteLine($"TOTAL \"TRANSLATION\" TOOK: {timer.Elapsed.TotalSeconds:f3} SECONDS" + Environment.NewLine);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("NOW WE ARE GOING TO TEST ASYNC METHODS..." + Environment.NewLine);
            Console.ResetColor();
            Stopwatch secTimer = new Stopwatch();
            secTimer.Start();
            Task[] tasks = new Task[countBooks];
            int lastTaskNumber = 0;
            for (int i = 0; i < allPaths.Length; ++i)
            {
                if (!allPaths[i].Contains("new_"))
                {
                    string bookspath = allPaths[i].Replace(path, "");
                    Task task = new Task(() => TranslateTheBook(bookspath)); // Запускаем асинхронные методы перевода.
                    tasks[lastTaskNumber] = task;
                    task.Start();
                    ++lastTaskNumber;
                }
            }
            Task.WaitAll(tasks);
            secTimer.Stop();
            Console.WriteLine($"TOTAL \"TRANSLATION\" TOOK: {secTimer.Elapsed.TotalSeconds:f3} SECONDS" + Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("RESULTS" + Environment.NewLine +
                $"WITHOUT ASYNC METHODS: {timer.Elapsed.TotalSeconds:f3} SECONDS" + Environment.NewLine +
                $"WITH ASYNC METHODS: {secTimer.Elapsed.TotalSeconds:f3} SECONDS" + Environment.NewLine);
            Console.ResetColor();
        }

        /// <summary>
        /// Ускоренный метод, для активации выполнения синхронного и асинхронного перевода.
        /// </summary>
        static void FirstTaskMethS()
        {
            string[] allPaths;
            try
            {
                allPaths = Directory.GetFiles(path);
            }
            catch (Exception e)
            {
                UsualExceptionMeth(e.Message);
                return;
            }
            int countBooks = 0;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            foreach (string str in allPaths)
            {
                if (!str.Contains("new_"))
                {
                    ++countBooks;
                    TranslateTheBookFaster(str.Replace(path, "")); // Поочереди выполняем перевод.
                }
            }
            timer.Stop();
            Console.WriteLine($"TOTAL \"TRANSLATION\" TOOK: {timer.Elapsed.TotalSeconds:f3} SECONDS" + Environment.NewLine);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("NOW WE ARE GOING TO TEST ASYNC METHODS..." + Environment.NewLine);
            Console.ResetColor();
            Stopwatch secTimer = new Stopwatch();
            secTimer.Start();
            Task[] tasks = new Task[countBooks];
            int lastTaskNumber = 0;
            for (int i = 0; i < allPaths.Length; ++i)
            {
                if (!allPaths[i].Contains("new_"))
                {
                    string bookspath = allPaths[i].Replace(path, "");
                    Task task = new Task(() => TranslateTheBookFaster(bookspath)); // Запускаем асинхронные методы перевода.
                    tasks[lastTaskNumber] = task;
                    task.Start();
                    ++lastTaskNumber;
                }
            }
            Task.WaitAll(tasks);
            secTimer.Stop();
            Console.WriteLine($"TOTAL \"TRANSLATION\" TOOK: {secTimer.Elapsed.TotalSeconds:f3} SECONDS" + Environment.NewLine);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("RESULTS" + Environment.NewLine +
                $"WITHOUT ASYNC METHODS: {timer.Elapsed.TotalSeconds:f3} SECONDS" + Environment.NewLine +
                $"WITH ASYNC METHODS: {secTimer.Elapsed.TotalSeconds:f3} SECONDS" + Environment.NewLine);
            Console.ResetColor();
        }

        /// <summary>
        /// Метод, для чтения текста с сайта, перевода и записи нового текста в книгу.
        /// </summary>
        static void SecondTaskMeth()
        {
            Task<string> sth;
            try
            {
                sth = GetSth(webPath);
                sth.Wait();
            }
            catch (HttpRequestException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("We got some problems with connection: ");
                Console.ResetColor();
                Console.WriteLine(e.Message + Environment.NewLine);
                return;
            }
            catch (Exception e)
            {
                UsualExceptionMeth(e.Message);
                return;
            }

            Console.WriteLine("Ok, we got a text. Now we're going to \"TRANSLATE\" it . . ." + Environment.NewLine);
            string appendThis;
            Stopwatch timer = new Stopwatch();
            int totalEnS = 0;
            int totalRuS = 0;
            timer.Start();
            try
            {
                using (StreamWriter streamW = new StreamWriter(secondTaskBookPath))
                {
                    foreach (char sign in sth.Result)
                    {
                        ++totalEnS;
                        if (char.IsLetter((char)sign) && myTranslator.ContainsKey((char)sign))
                        {
                            appendThis = myTranslator[(char)sign];
                            streamW.Write(appendThis);
                            totalRuS += appendThis.Length;
                        }
                        else
                        {
                            streamW.Write(sign);
                            ++totalRuS;
                        }
                        streamW.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                UsualExceptionMeth(e.Message);
                return;
            }
            timer.Stop();
            Console.ForegroundColor = ConsoleColor.Magenta;
            PrintInfoAboutBook(secondTaskBookPath.Replace(path, ""), totalEnS, totalRuS, timer.Elapsed);
            Console.ResetColor();
        }

        /// <summary>
        /// Асинхронное обращение к сайту.
        /// </summary>
        /// <param name="wPath"> ссылка на сайт </param>
        /// <returns> задача по чтению сайта </returns>
        static async Task<string> GetSth(string wPath)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(wPath);
            return response;

        }

        /// <summary>
        /// Метод, проверяющий, хочет ли пользователь повторить выполнение программы.
        /// </summary>
        /// <returns> результат проверки </returns>
        static bool DoYouWantRepeat()
        {
            Console.WriteLine("Do you want to repeat?");
            Console.Write("Press Enter button if ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("YES");
            Console.ResetColor();
            Console.Write("Press any other button if ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("NO" + Environment.NewLine);
            Console.ResetColor();
            if (Console.ReadKey().Key == ConsoleKey.Enter)
                return true;
            return false;
        }

        /// <summary>
        /// Метод, выводящий основную информацию о переводе.
        /// </summary>
        /// <param name="name"> название книги, которую переводили </param>
        /// <param name="totalEnS"> символов до перевода </param>
        /// <param name="totalRuS"> символов после перевода </param>
        /// <param name="time"> время перевода </param>
        static void PrintInfoAboutBook(string name, int totalEnS, int totalRuS, TimeSpan time)
        {
            Console.WriteLine($"BOOK'S NAME: {name}");
            Console.WriteLine($"SYMBOLS BEFORE \"TRANSLATION\": {totalEnS}");
            Console.WriteLine($"SYMBOLS AFTER \"TRANSLATION\": {totalRuS}");
            Console.WriteLine($"THE \"TRANSLATION\" HAS TAKEN: {time.TotalSeconds:f3} SECONDS" + Environment.NewLine);
            Console.ResetColor();
        }

        /// <summary>
        /// Метод, выводящий информацию об обычном исключение.
        /// </summary>
        /// <param name="msg"> сообщение исключения </param>
        static void UsualExceptionMeth(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("We got some problems: ");
            Console.ResetColor();
            Console.WriteLine(msg + Environment.NewLine);
        }

        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("Which task do you want to check?");
                Console.Write("Press 1 button if ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Task №1");
                Console.ResetColor();
                Console.Write("Press 2 button if ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("Task №1");
                Console.ResetColor();
                Console.WriteLine(", but much faster");
                Console.Write("Press 3 button if ");
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine("Task №2");
                Console.ResetColor();
                Console.Write("Press any other button if you want to finish :(" + Environment.NewLine);
                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.D1)
                {
                    Console.WriteLine(" running . . ." + Environment.NewLine);
                    FirstTaskMeth(); // Оказалось, что читать и одновременно записывать не самая лучшая идея. Очень жаль((((
                                     // А еще, я думал, что это не безопасно всю книгу загонять в оперативную память (закончится может)
                                     // Похоже, что не может...
                }
                else if (key == ConsoleKey.D2)
                {
                    Console.WriteLine(" running . . ." + Environment.NewLine);
                    FirstTaskMethS();
                }
                else if (key == ConsoleKey.D3)
                {
                    Console.WriteLine(" running . . ." + Environment.NewLine);
                    SecondTaskMeth();
                }
                else
                {
                    break;
                }
            }
            while (DoYouWantRepeat());
        }
    }
}
