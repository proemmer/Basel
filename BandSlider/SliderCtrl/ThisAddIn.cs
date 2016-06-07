using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;
using System.Web.Http.SelfHost;
using System.Web.Http;

namespace SliderCtrl
{
    public partial class ThisAddIn
    {
        private HttpSelfHostServer _server;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            var config = new HttpSelfHostConfiguration("http://localhost:5000");

            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{action}/{id}",
                new { id = RouteParameter.Optional });

            _server = new HttpSelfHostServer(config);
            _server.OpenAsync().Wait();
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            if (_server != null)
                _server.Dispose();
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
