using System;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Learn
{
    class Program
    {
        ///<summary>
        ///Модуль для отправки данных к БД redis
        ///Включает в себя пользовательский ввод,
        ///метод publish для redis и 
        ///проверки успешности выполнения
        ///</summary>
        public static void Main()
        {
            var db = default(IDatabase);
            Config(ref db);
            Loop("userKey", db);
        }
        ///<summary>
        ///Основный цикл программы
        ///</summary>
        ///<remarks>
        ///Считывает пользовательский ввод, затем отправляет его в виде запроса к БД redis
        ///</remarks>
        public static void Loop(string generalKey, IDatabase db)
        {
            var numbers = new List<int>();
            var command = "";
            while(command != ":exit")
            {
                Console.Write("INPUT TWO NUMBERS SPLIT BY SPACE: ");
                command = Console.ReadLine();
                var nums = command.Split();

                if(CheckArrInt32(nums) && nums.Length == 2)
                {   
                    numbers.Clear();
                    for(int i = 0; i < nums.Length; i++)
                    {
                        numbers.Add(Int32.Parse(nums[i]));
                    }
                    Console.WriteLine("NUMBER[1]: {0} NUMBER[2]: {1}", numbers[0], numbers[1]);
                    SetRequest(db, generalKey, "1", numbers[0], "myChanel");
                    SetRequest(db, generalKey, "2", numbers[1], "myChanel");
                }
                else
                    Console.WriteLine("ERR: WRONG INPUT");

            }
        }

        ///<summary>
        ///Функция получения соединения с БД redis, возвращает значение по ссылке ref db
        ///</summary>
        public static void Config(ref IDatabase db, string url = "localhost", int port = 6379, int dbIndex = -1)
        {
            try
            {
                var config = new ConfigurationOptions();
                config.EndPoints.Add(url, port);
                var conMult = ConnectionMultiplexer.Connect(config);
                db = conMult.GetDatabase(dbIndex);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        ///<summary>
        ///Отправляет SET запрос на указанную БД redis по ключу с параметром и переданным сообщением
        ///</summary>
        ///<remarks>
        ///Если указать в аргументах название канала, то опубликует сообщение, 
        ///содержащее ключ передаваемого запроса
        ///</remarks>
        public static void SetRequest(IDatabase db, string key, string keyParam, int num, string chanelName = "")
        {
            try
            {
                db.StringSet(key + ":" + keyParam.ToString(), num);
                if(chanelName != "")
                    db.Publish(chanelName, key + ":" + keyParam.ToString());
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        ///<summary>
        ///Проверка на то, что все элементы строки можно привести с типу Int32
        ///</summary>
        public static bool CheckArrInt32(string[] lst)
        {
            var num = 0;
            for(int i = 0; i < lst.Length; i++)
            {
                if(Int32.TryParse(lst[i], out num));
                else return false;
            }
            return true;
        }
    }
}   