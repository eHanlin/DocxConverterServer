using System;
using System.Diagnostics;
using System.Security.AccessControl;
using EasyNetQ;
using EasyNetQ.FluentConfiguration;
using WordConverterServer.EsayNetQ;
using WordConverterServer.Models;

namespace WordConverterServer.EsaynetQ
{
    public class MqHelper
    {
        //發送訊息
        public static void Publish(ConvertTask task)
        {
            IBus bus = BusBuilder.CreateMessageBus();
            try
            {
               bus.Publish(task);
            }
            catch (EasyNetQException ex)
            {
                throw ex;
            }
            bus.Dispose();
        }

        //接收訊息
        public static void Subscribe()
        {
            IBus bus = BusBuilder.CreateMessageBus();
            try
            {
                TaskHandler taskHandler = new TaskHandler();
                bus.Subscribe<ConvertTask>("task", c => taskHandler.Convert(c));
            }
            catch (EasyNetQException ex)
            {
                throw ex;
            }
        }
    }
}