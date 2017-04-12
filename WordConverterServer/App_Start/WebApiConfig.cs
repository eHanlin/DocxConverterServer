using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WordConverterServer.EsaynetQ;

namespace WordConverterServer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 設定和服務

            // Web API 路由
            config.MapHttpAttributeRoutes();
            config.EnableCors(new EnableCorsAttribute("*","*","*"));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            WebInit();
        }

        public static void WebInit()
        {
            MqHelper.Subscribe();
            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data"), "words");
            Directory.CreateDirectory(path);
        }
    }
}
