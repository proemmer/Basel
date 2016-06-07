using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace SliderCtrl
{
    public class SlideController : ApiController
    {

        public SlideController()
        {

        }

        [HttpGet]
        public int Page()
        {
            return Globals.ThisAddIn.Application.ActivePresentation.SlideShowWindow.View.Slide.SlideIndex;
        }

        [HttpPost]
        public void Start()
        {
            Globals.ThisAddIn.Application.ActivePresentation.SlideShowSettings.Run();
        }

        [HttpPost]
        public void Stop()
        {
            Globals.ThisAddIn.Application.ActivePresentation.SlideShowWindow.View.Exit();
        }

        [HttpPost]
        public void Next()
        {
            Globals.ThisAddIn.Application.ActivePresentation.SlideShowWindow.View.Next();
        }

        [HttpPost]
        public void Prev()
        {
            Globals.ThisAddIn.Application.ActivePresentation.SlideShowWindow.View.Previous();
        }
    }
}
