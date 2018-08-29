using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Base.Core.Culture.RequestCulture
{
    public class RequestUriCultureFinder
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _context;
        public RequestUriCultureFinder(IConfiguration configuration,
                                    IHttpContextAccessor context)
        {
            _configuration = configuration;
            _context = context;

            /*var builder = new UriBuilder();
            builder.Scheme = request.Scheme;
            builder.Host = request.Host.Value;
            builder.Path = request.Path;
            builder.Query = request.QueryString.ToUriComponent();
            return builder.Uri;*/
        }

        public string ReplaceCultureCode()
        {
            try {
                //var queryStr = _context.HttpContext.Request.QueryString;
                var uripath = _context.HttpContext.Request.Path;
                //var host = _context.HttpContext.Request.Host.Value;
                if (!string.IsNullOrEmpty(uripath.ToString()))
                {
                    return ReplaceCultureCode(uripath.ToString());
                }
                else
                {
                    return "";
                }
            } catch(Exception ex)
            {
                return "";
            } 
            
           
        }

        private string ReplaceCultureCode(string queryStr)
        {
            StringBuilder queryStrBuilder = new StringBuilder(queryStr);
            queryStrBuilder.Replace("  ", string.Empty);
            queryStrBuilder.Replace(Environment.NewLine, string.Empty);
            queryStrBuilder.Replace("/" + CultureInfoNames.AF + "/", "/--dil--/");
            queryStrBuilder.Replace("/" + CultureInfoNames.US + "/", "/--dil--/");
            queryStrBuilder.Replace("/" + CultureInfoNames.TR + "/", "/--dil--/");
            queryStrBuilder.Replace("/" + CultureInfoNames.FA + "/", "/--dil--/");
            queryStrBuilder.Replace("/" + CultureInfoNames.AR + "/", "/--dil--/");
            queryStrBuilder.Replace("/" + CultureInfoNames.ZH + "/", "/--dil--/");
            queryStrBuilder.Replace("/" + CultureInfoNames.MX + "/", "/--dil--/");
            queryStrBuilder.Replace("/" + CultureInfoNames.FR + "/", "/--dil--/");
            queryStrBuilder.Replace("/" + CultureInfoNames.RU + "/", "/--dil--/");
            return queryStrBuilder.ToString();
        }
       
    }
}
