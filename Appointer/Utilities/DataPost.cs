using System.Collections.Specialized;
using System.Web;

namespace Appointer.Utility
{
    public class DataPost
    {
        private readonly NameValueCollection _inputs = new NameValueCollection();
        public string Url { get; set; } = "";
        public string Method { get; set; } = "POST"; //or Get
        public string FormName { get; set; } = "form1";

        public void AddKey(string name, string value)
        {
            _inputs.Add(name, value);
        }

        public void Post(HttpContextBase httpContext)
        {
            var context = httpContext; //HttpContext.Current;

            var inputs = "";
            for (int i = 0; i < _inputs.Keys.Count; i++)
            {
                inputs += $"<input name='{_inputs.Keys[i]}' type='hidden' value='{_inputs[_inputs.Keys[i]]}'>";
            }
            var html = $@"
                <html>
                <head>
                </head>
                <body>
	                <form name='{FormName}' method='{Method}' action='{Url}'>
		                {inputs}
                        <script>
                            document.{FormName}.submit();
                        </script>
	                </form>
                </body>
                </html>";

            context.Response.Clear();
            context.Response.Write(html);
            context.Response.Flush();
            context.Response.SuppressContent = true;
            context.ApplicationInstance.CompleteRequest();
        }
    }
}
