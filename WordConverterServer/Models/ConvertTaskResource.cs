using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WordConverterServer.Models
{
    public class ConvertTaskResource
    {
        public string TaskId { get; set; }
        public string Docx { get; set; }//s3 word path
        public string Result { get; set; }//s3 path afeter convert 
        public string TaskType { get; set; }//docOrPdf       
        public bool ConvertSuccess { get; set; }
        public bool HookSuccess { get; set; }
        public string ExceptionLog { get; set; }
        public DateTime CreateOn { get; set; }
        public string Path { get; set; }
    }
}