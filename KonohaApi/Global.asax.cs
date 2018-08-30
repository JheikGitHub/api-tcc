﻿using KonohaApi.Mappers;

namespace KonohaApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AutoMapperConfig.RegisterMappings();

        }
    }
}
