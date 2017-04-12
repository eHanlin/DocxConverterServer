using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using WordConverterServer.Models;
using Word = Microsoft.Office.Interop.Word;

namespace WordConverterServer
{
    public class TaskHandler
    {
        readonly S3Repository _s3Repository = new S3Repository();
        readonly MongoRepository _mongoRepository = new MongoRepository();
        readonly string _path = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "words");


        public void Convert(ConvertTask task)
        {
            string fileName = HttpUtility.UrlDecode(task.Docx.Split('/').Last());
            string localPath = $@"{_path}\{fileName}";
            task.Path = localPath;
            _mongoRepository.Create(task);

            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(task.Docx, localPath);
            }
            var targetPath = WordProcess(task, localPath, fileName);

            if (File.Exists(targetPath) && !string.IsNullOrEmpty(targetPath))
            {
                _mongoRepository.ConvertSuccese(task);
                DeleteFile(localPath);
                task.Result = RetryHelper.Do(()=>_s3Repository.UploadFile(targetPath,task.TaskId),TimeSpan.FromSeconds(5));
                DeleteFile(targetPath);
                UploadSuccess(task);
                SendResponse(task);
            }
        }

        private string WordProcess(ConvertTask task,string localPath,string fileName)
        {

            string targetPath = "";
            var wordApp = new Word.Application
            {
                Visible = true,
                DisplayAlerts = Word.WdAlertLevel.wdAlertsNone
            };
            try
            {
                var docx = wordApp.Documents.Open(localPath);
                if (task.TaskType == "Doc")
                {
                    targetPath = $"{_path}\\{fileName.Split('.').First()}.doc";
                    docx.SaveAs(targetPath, Word.WdSaveFormat.wdFormatDocument);
                }
                else if (task.TaskType == "Pdf")
                {
                    targetPath = $"{_path}\\{fileName.Split('.').First()}.pdf";
                    docx.SaveAs(targetPath, Word.WdSaveFormat.wdFormatPDF);
                }
            }
            catch (Exception e)
            {
                task.ExceptionLog = e.ToString();
                SendResponse(task);
            }
            finally
            {
                wordApp.Documents.Close();
                wordApp.Quit();
            }
            return targetPath;
        }


        private void SendResponse(ConvertTask task)
        {
            WebRequest request = WebRequest.Create(task.GetCallBackUrl());
            request.ContentType = "application/json";
            request.Method = "GET";
            
            WebResponse response = RetryHelper.Do(()=>request.GetResponse(),TimeSpan.FromMinutes(3));
            task.HookSuccess = true;
            _mongoRepository.HookSuccess(task);
            response.Dispose();
        }

        private void DeleteFile(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            fi.Delete();
        }

        private void UploadSuccess(ConvertTask task)
        {
            if (!string.IsNullOrEmpty(task.Result))
            {
                task.ConvertSuccess = true;
                _mongoRepository.Update(task);
            }
        }
    }
}