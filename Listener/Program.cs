using System;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Listener
{
    class Program
    {
        ///<summary>
        ///Модуль для получения данных от БД redis
        ///Включает в себя метод subscribe для redis
        ///и проверки успешности выполнения
        ///</summary>
        static void Main()
        {
            IDatabase db = default(IDatabase);
            var ch = Config(ref db, "myChanel");
            Listen(db, ch);
            Console.ReadKey(); 
        }

        ///<summary>
        ///Функция получения соединения с БД redis, возвращает значение по ссылке ref db
        ///</summary>
        public static ChannelMessageQueue Config(ref IDatabase db,  string chanelName, string url = "localhost", int port = 6379, int dbIndex = -1)
        {
            var chanel = default(ChannelMessageQueue);
            try
            {
                var config = new ConfigurationOptions();
                config.EndPoints.Add(url, port);
                var conMult = ConnectionMultiplexer.Connect(config);
                db = conMult.GetDatabase(dbIndex);
                chanel = conMult.GetSubscriber().Subscribe(chanelName);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return chanel;
        }

        ///<summary>
        ///Функция прослушивания указанного канал на наличие новых сообщений
        ///</summary>
        ///<remarks>
        ///При получении сообщения записывает его, затем когда кол - во сообщений
        /// = 2, перемножает полученные величины
        ///</remarks>
        public static void Listen(IDatabase db, ChannelMessageQueue chanel)
        {
            Console.WriteLine("Listening...");
            var numbers = new List<string>();

            chanel.OnMessage(message =>
            {
                numbers.Add(db.StringGet(message.Message.ToString()));
                if(numbers.Count >= 2)
                {
                    Console.WriteLine("NEW MULTIPLY RESULT: [" + (Int32.Parse(numbers[0]) * Int32.Parse(numbers[1])) + "]");
                    numbers.Clear();
                    Console.WriteLine("Listening...");
                }
            });
        }
    }
}
