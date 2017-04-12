using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WordConverterServer.Models
{
    public class ConvertTask
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string TaskId = Guid.NewGuid().ToString();
        public string Docx { get; set; }//s3 word path
        public string CallBack { get; set; }//CallBack path
        public string Result { get; set; }//s3 path afeter convert 
        public string TaskType { get; set; }//docOrPdf       
        public bool ConvertSuccess { get; set; } = false;
        public bool HookSuccess { get; set; } = false;
        public string ExceptionLog { get; set; }
        public DateTime CreateOn { get; set; } = DateTime.Now;
        public string Path { get; set; }

        public ConvertTask(string taskId, string docx, string callback, string type)
        {
            TaskId = taskId;
            Docx = docx;
            CallBack = callback;
            TaskType = type;
        }

        public ConvertTask(string docx,string callback,string type)
        {
            Docx = docx;
            CallBack = callback;
            TaskType = type;
        }

        public ConvertTask()
        {
        }

        public string GetCallBackUrl()
        {
            return  $"{CallBack}?id={TaskId}&success={ConvertSuccess}&result={Result}";
        }

        public Dictionary<string,string> ToDictionary()
        {
            var result = new Dictionary<string, string>
            {
                {"Id", Id.ToString()},
                {"TaskId", TaskId},
                {"Docx",Docx},
                {"CallBack",CallBack},
                {"Result",Result},
                {"TaskType",TaskType },
                {"ConvertSuccess",ConvertSuccess.ToString() },
                {"HookSuccess",HookSuccess.ToString()}
            };
            return result;
        }
    }
}