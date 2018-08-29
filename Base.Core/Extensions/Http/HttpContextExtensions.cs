using Base.Core.Entities.Session;
using Microsoft.AspNetCore.Http;


namespace Base.Core.Extensions
{
    public static class HttpContextExtensions
    {

        public static SessionUserModel GetMiyaUser(this HttpContext context)
        {
            if(context.Items.ContainsKey("MiyaUser"))
                            return (SessionUserModel)context.Items["MiyaUser"];
            return new SessionUserModel();
        }

        public static string GetPublicKey(this HttpContext context)
        {
            
            if(context.Items.ContainsKey("PublicKey") && context.Items["PublicKey"] !=null)
                            return context.Items["PublicKey"].ToString();
            return "";
        }

        public static string GetUserName(this HttpContext context)
        {
            if(context.Items.ContainsKey("MiyaUserName") && context.Items["MiyaUserName"] != null)
                            return context.Items["MiyaUserName"].ToString();
            return "";
        }

        public static string GetUserPassword(this HttpContext context)
        {
            if(context.Items.ContainsKey("MiyaUserPassword") && context.Items["MiyaUserPassword"]!=null)
                            return context.Items["MiyaUserPassword"].ToString();
            return "";
        }

        public static string GetPrivateKey(this HttpContext context)
        {
            if(context.Items.ContainsKey("PrivateKey") && context.Items["PrivateKey"]!=null)
                            return context.Items["PrivateKey"].ToString();
            return "";
        }

        public static string GetHmacToken(this HttpContext context)
        {
            if(context.Items.ContainsKey("HmacToken") && context.Items["HmacToken"] !=null)
                            return context.Items["HmacToken"].ToString();
            return "";
        }

        public static SessionUserModel GetUser(this HttpContext context)
        {
            if(context.Items.ContainsKey("MiyaUser") && context.Items["MiyaUser"] !=null)
                            return (SessionUserModel)context.Items["MiyaUser"];
            return default(SessionUserModel);
        }

        public static string GetUserAgent(this HttpContext context)
        {
            if (context.Items.ContainsKey("UserAgent") && context.Items["UserAgent"] != null)
                return context.Items["UserAgent"].ToString();
            return "";
        }
    }
}
