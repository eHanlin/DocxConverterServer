using System;
using System.Configuration;
using System.Text;
using EasyNetQ;
using Newtonsoft.Json;
using WordConverterServer.Models;

namespace WordConverterServer.EsayNetQ
{
    public class BusBuilder
    {
        //註冊rabbitmq
        public static IBus CreateMessageBus()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["easyNetQ"].ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("messageserver connection string is missing or empty");
            }
            return RabbitHutch.CreateBus(connectionString,
                register => register.Register<ISerializer>(sp => new ConvertTaskSerializer()));
        }
    }

    public class ConvertTaskSerializer : ISerializer
    {
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        public byte[] MessageToBytes<T>(T message) where T : class
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, _serializerSettings));
        }

        public T BytesToMessage<T>(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes), _serializerSettings);
        }

        public object BytesToMessage(string typeName, byte[] bytes)
        {
            dynamic target = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(bytes));
            ConvertTask task = new ConvertTask(target.TaskId.ToString(),target.Docx.ToString(),target.CallBack.ToString(),target.TaskType.ToString());
            return task;
        }
    }
}