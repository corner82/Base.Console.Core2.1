using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Newtonsoft.Json;
using Base.Core.Entities.Log;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Base.Core.Culture.RequestCulture
{
    public class RequestCultureFinder
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _context;
        public RequestCultureFinder(IConfiguration configuration,
                                    IHttpContextAccessor context
                                      )
        {
            _configuration = configuration;
            _context = context;
        }

        public  string GetRequestCultureInfo()
        {
            var rqf = _context.HttpContext.Request.HttpContext.Features.Get<IRequestCultureFeature>();
            var culture = rqf.RequestCulture.Culture;
            var cultureShortName = GetShortCultureInfo(culture.ToString());
            return cultureShortName;

        }

        private string GetShortCultureInfo(string cultureInfo)
        {
            if(!string.IsNullOrEmpty(cultureInfo))
            {
               switch (cultureInfo)
                {
                    case CultureInfoNames.TR:
                        return "tr";
                    case CultureInfoNames.MX:
                        return "es";
                    case CultureInfoNames.FR:
                        return "fr";
                    case CultureInfoNames.DE:
                        return "de";
                    case CultureInfoNames.FA:
                        return "fa";
                    case CultureInfoNames.AR:
                        return "ar";
                    case CultureInfoNames.RU:
                        return "ru";
                    case CultureInfoNames.AF:
                        return "af";
                    case CultureInfoNames.ZH:
                        return "zh";
                    default:
                        return "en";
                        
                }
                
            } else
            {
                return "en";
            }
            
        }
    }
}
