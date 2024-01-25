using Employee_Shared_Service.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Employee_WebAPI.Filters
{
    public static class ProcessIP
    {
        private static LogHeaders log;
        static ProcessIP()
        {
            log = new LogHeaders(AppDomain.CurrentDomain.GetData("ContentRootPath") + "\\logs\\logHeader_Each_Request_" + DateTime.Now.ToString("dd_MM_yy") + ".txt");
        }

        public static string GetRequestIP(this IHttpContextAccessor _httpContextAccessor, bool tryUseXForwardHeader = true)
        {
            string ip = null;

            // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

            // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
            // for 99% of cases however it has been suggested that a better (although tedious)
            // approach might be to read each IP from right to left and use the first public IP.
            // http://stackoverflow.com/a/43554000/538763
            //
            if (tryUseXForwardHeader)
                ip = _httpContextAccessor.GetHeaderValueAs<string>("X-Forwarded-For").SplitCsv().FirstOrDefault();

            // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
            if (ip.IsNullOrWhitespace() && _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress != null)
                ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            if (ip.IsNullOrWhitespace())
                ip = _httpContextAccessor.GetHeaderValueAs<string>("REMOTE_ADDR");

            // _httpContextAccessor.HttpContext?.Request?.Host this is the local host.

            if (ip.IsNullOrWhitespace()) 
                    throw new AccessViolationException("Unable to determine caller's IP.");

            return ip;
        }

        internal static string GetUserLanguage()
        {
            throw new NotImplementedException();
        }

        public static string ClientLanguage(this IHttpContextAccessor _httpContextAccessor, bool tryUseXForwardHeader = true)
        {
            string lang = null;
            if (tryUseXForwardHeader)
                lang = _httpContextAccessor.GetHeaderValueAs<string>("Accept-Language").SplitCsv().FirstOrDefault();
            return lang;

        }

        public static string UserAgent(this IHttpContextAccessor _httpContextAccessor, bool tryUseXForwardHeader = true)
        {
            string useragent = null;
            if (tryUseXForwardHeader)
                useragent = _httpContextAccessor.GetHeaderValueAs<string>("User-Agent").SplitCsv().FirstOrDefault();
            return useragent;

        }


        public static void Unknown(this IHttpContextAccessor _httpContextAccessor, bool tryUseXForwardHeader = true)
        {
            foreach(var key in _httpContextAccessor.HttpContext?.Request?.Headers?.Keys)
            {
                log.WriteLog(DateTime.Now.ToString("dd_MM_yy_hh_mm_ss")+" " +key + " : " + _httpContextAccessor.HttpContext?.Request?.Headers[key].ToString());
            }

            log.WriteLogComplete();
            

        }
        public static T GetHeaderValueAs<T>(this IHttpContextAccessor _httpContextAccessor, string headerName)
        {
            StringValues values;
            //User Languages var languages = Request.Headers["Accept-Language"];
            //User Agent var userAgent = Request.Headers["User-Agent"];

            if (_httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
            {
                string rawValues = values.ToString();   // writes out as Csv when there are multiple.

                if (!rawValues.IsNullOrWhitespace())
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default(T);
        }

        public static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable<string>()
                .Select(s => s.Trim())
                .ToList();
        }

        public static bool IsNullOrWhitespace(this string s)
        {
            return String.IsNullOrWhiteSpace(s);
        }
    }
}
