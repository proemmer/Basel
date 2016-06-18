using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandSlider.Tile
{
    /// <summary>
    /// Constants related to the tile we install on the Band and its visual components.
    /// </summary>
    public static class TileConstants
    {
        // WARNING! This tile guid is only an example. Please do not copy it to your test application;
        // always create a unique guid for each application.
        // If one application installs its tile, a second application using the same guid will fail to install
        // its tile due to a guid conflict. In the event of such a failure, the text of the exception will not
        // report that the tile with the same guid already exists on the band.
        // There might be other unexpected behavior.
        private static Guid tileGuid = new Guid("59761A7C-5630-4844-9E66-ED2CDD570F05");
        private static Guid page1Guid = new Guid("00000000-0000-0000-0000-000000000001");

        public static Guid TileGuid { get { return tileGuid; } }
        public static Guid Page1Guid { get { return page1Guid; } }

    }
}
