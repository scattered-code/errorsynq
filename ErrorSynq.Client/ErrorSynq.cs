using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;

namespace ErrorSynq
{
    public class Client
    {
        public static bool SubmitError()
        {
            var success = false;

            try
            {
                TrackingInfo info = new TrackingInfo();

                var Request = System.Web.HttpContext.Current.Request;
                var Server = System.Web.HttpContext.Current.Server;

                info.ErrorPath = "http://" + Request.ServerVariables["HTTP_HOST"] + Request.Path;
                info.RawUrl = Request.RawUrl;

                try
                {
                    // Get the exception object for the last error message that occurred.
                    var lastError = Server.GetLastError();
                    if (lastError != null)
                    {
                        var ErrorInfo = lastError.GetBaseException();
                        if (ErrorInfo != null)
                        {
                            info.ErrorMessage = ErrorInfo.Message;
                            info.ErrorSource = ErrorInfo.Source;
                            info.TargetSite = ErrorInfo.TargetSite.ToString();
                        }

                        info.ErrorType = lastError.GetType().ToString();

                        if (lastError.GetType() == typeof(HttpUnhandledException))
                            info.HtmlErrorMessage = (lastError as HttpUnhandledException).GetHtmlErrorMessage();

                        info.StackTrace = lastError.StackTrace;
                    }

                    info.QueryStringData = new List<KeyValueModel>();
                    foreach (var item in Request.QueryString.AllKeys)
                        info.QueryStringData.Add(new KeyValueModel { Key = item, Value = Request.QueryString[item] });
                    
                    info.PostData = new List<KeyValueModel>();
                    foreach (var item in Request.Form.AllKeys)
                        info.PostData.Add(new KeyValueModel { Key = item, Value = Request.Form[item] });

                    info.SessionData = new List<KeyValueModel>();
                    for (int i = 0; i < System.Web.HttpContext.Current.Session.Count; i++)
                        info.SessionData.Add(new KeyValueModel { Key = System.Web.HttpContext.Current.Session.Keys[i].ToString(), Value = System.Web.HttpContext.Current.Session[i].ToString() });
                    
                    if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                        info.UserAccountName = System.Web.HttpContext.Current.User.Identity.Name;

                    info.ServerVariables = new List<KeyValueModel>();
                    foreach (var item in Request.ServerVariables.AllKeys)
                        info.ServerVariables.Add(new KeyValueModel { Key = item, Value = Request.ServerVariables[item] });
                }
                catch (Exception s) { }

                info.SiteKey = Guid.Parse(System.Configuration.ConfigurationManager.AppSettings["ErrorTrackingKey"]);
                info.IsRead = false;
                info.IsResolved = false;
                info.DateLogged = DateTime.Now;
                info.ErrorCause = ErrorCauses.Unknown;

                var binding = new BasicHttpBinding();
                binding.CloseTimeout = new TimeSpan(0, 10, 0);
                binding.OpenTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                binding.SendTimeout = new TimeSpan(0, 10, 0);
                binding.AllowCookies = false;
                binding.BypassProxyOnLocal = false;
                binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                binding.MaxBufferPoolSize = 2147483647;
                binding.MaxBufferSize = 2147483647;
                binding.MaxReceivedMessageSize = 2147483647;
                binding.TextEncoding = Encoding.UTF8;
                binding.TransferMode =  TransferMode.Buffered;
                binding.UseDefaultWebProxy = true;
                binding.MessageEncoding = WSMessageEncoding.Text;
                
                binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas();
                binding.ReaderQuotas.MaxDepth = 32;
                binding.ReaderQuotas.MaxStringContentLength = 2147483647;
                binding.ReaderQuotas.MaxArrayLength = 2147483647;
                binding.ReaderQuotas.MaxBytesPerRead = 2147483647;
                binding.ReaderQuotas.MaxNameTableCharCount = 2147483647;

                binding.Security = new BasicHttpSecurity();
                binding.Security.Mode = BasicHttpSecurityMode.None;

                var remoteAddress = new EndpointAddress("http://api.errorsynq.net/errortracking.svc");
                using (var client = new ErrorTrackingServiceClient(binding, remoteAddress))
                {
                    success = client.SubmitError(info);
                }

            }
            catch (Exception ex)
            {

            }
            return success;
        }
    }
}
