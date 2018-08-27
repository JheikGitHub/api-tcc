using KonohaApi.App_Start;
using KonohaApi.Mappers;
using System.Web.Http;

namespace KonohaApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AutoMapperConfig.RegisterMappings();

        }
    }
}
