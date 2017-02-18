using System.Web.Http;
using WordConverterServer.EsaynetQ;
using WordConverterServer.Models;

namespace WordConverterServer.Controllers
{
    [RoutePrefix("api/Converter")]
    public class ConverterController : ApiController
    {
        [HttpPost]
        [Route("ConvertToPdf")]
        public string ConvertToPdf([FromBody]ConvertTask task)
        {
            task.TaskType = "Pdf";
            MqHelper.Publish(task);

            return task.TaskId;
        }

        [HttpPost]
        [Route("ConvertToDoc")]
        public string ConvertToDoc([FromBody]ConvertTask task)
        {
            task.TaskType = "Doc";
            MqHelper.Publish(task);

            return task.TaskId;
        }
    }
}
