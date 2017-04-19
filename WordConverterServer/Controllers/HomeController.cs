using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPaging;
using WordConverterServer.Models;

namespace WordConverterServer.Controllers
{
    public class HomeController : Controller
    {
        // GET: Index
        public ActionResult Index(int? page)
        {
            MongoRepository repository = new MongoRepository();
            var records = repository.GetAll();
            IEnumerable<ConvertTaskResource> result = null;
            if (records.Count > 0)
            {
                result = records.OrderByDescending(c=>c.CreateOn).Select(c => new ConvertTaskResource()
                {
                    TaskId = c.TaskId,
                    Docx = c.Docx,
                    Result = c.Result,
                    TaskType = c.TaskType,
                    ConvertSuccess = c.ConvertSuccess,
                    HookSuccess = c.HookSuccess,
                    ExceptionLog = c.ExceptionLog,
                    CreateOn = c.CreateOn
                }).ToList();

               
                ViewData["Count"] = result.Count();
                int defaultPageSize = 10;
                int currentPageIndex = page - 1 ?? 0;
                return View(result.ToPagedList(currentPageIndex, defaultPageSize, result.Count()));
            }
            return View(result);
        }
    }
}