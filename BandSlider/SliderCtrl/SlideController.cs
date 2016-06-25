using System;
using System.Web.Http;

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
            try
            {
                return Globals.ThisAddIn.Application.ActivePresentation.SlideShowWindow.View.Slide.SlideIndex;
            }
            catch(Exception)
            {
                return 0;
            }
        }

        [HttpGet]
        public int State()
        {
            try
            {
                return Convert.ToInt32( Globals.ThisAddIn.Application.ActivePresentation.SlideShowWindow.View.State);
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        [HttpPost]
        public void Start()
        {
            try
            {
                Globals.ThisAddIn.Application.ActivePresentation.SlideShowSettings.Run();
            }
            catch(Exception)
            {

            }
        }

        [HttpPost]
        public void Stop()
        {
            try
            {
                Globals.ThisAddIn.Application.ActivePresentation.SlideShowWindow.View.Exit();
            }
            catch(Exception)
            {

            }
        }

        [HttpPost]
        public void Next()
        {
            try
            {
                Globals.ThisAddIn.Application.ActivePresentation.SlideShowWindow.View.Next();
            }
            catch(Exception)
            {

            }
        }

        [HttpPost]
        public void Prev()
        {
            try
            {
                Globals.ThisAddIn.Application.ActivePresentation.SlideShowWindow.View.Previous();
            }
            catch(Exception)
            {

            }
        }

    }
}
