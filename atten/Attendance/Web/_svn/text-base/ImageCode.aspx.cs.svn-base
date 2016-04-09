using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Attendance.Base;

namespace Attendance
{
    public partial class ImageCode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string str = ImageCreator.RndCode(4);
            Session["CheckCode"] = str;
            Bitmap image = ImageCreator.CreateAddCodeImage(str, true, 70, 22, 30);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, ImageFormat.Gif);
            image.Dispose();

            this.Context.Response.ClearContent();
            this.Context.Response.ContentType = "image/gif";
            this.Context.Response.BinaryWrite(ms.ToArray());
        }
    }
}