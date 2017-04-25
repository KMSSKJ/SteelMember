using LeaRun.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace LeaRun.WebApp.Controllers
{
    public class TestDemoController : Controller
    {
        /// <summary>
        /// 消息提示
        /// </summary>
        /// <returns></returns>
        public ActionResult jBox_master()
        {
            return View();
        }
        /// <summary>
        /// 布局
        /// </summary>
        /// <returns></returns>
        public ActionResult layout()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 上传用户头像
        /// </summary>
        /// <param name="Filedata">用户图片对象</param>
        /// <returns></returns>
        public ActionResult SubmitUploadify(HttpPostedFileBase Filedata)
        {
            try
            {
                Thread.Sleep(1000);////延迟500毫秒
                //没有文件上传，直接返回
                if (Filedata == null || string.IsNullOrEmpty(Filedata.FileName) || Filedata.ContentLength == 0)
                {
                    return HttpNotFound();
                }
                //获取文件完整文件名(包含绝对路径)
                //文件存放路径格式：/Resource/Document/NetworkDisk/{日期}/{guid}.{后缀名}
                //例如：/Resource/Document/Email/20130913/43CA215D947F8C1F1DDFCED383C4D706.jpg
                string fileGuid = CommonHelper.GetGuid;
                long filesize = Filedata.ContentLength;
                string FileEextension = Path.GetExtension(Filedata.FileName);
                string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                string UserId = ManageProvider.Provider.Current().UserId;

                string virtualPath = string.Format("/Content/Images/Avatar/{0}/{1}/{2}{3}", UserId, uploadDate, fileGuid, FileEextension);
                string fullFileName = this.Server.MapPath(virtualPath);
                //创建文件夹，保存文件
                string path = Path.GetDirectoryName(fullFileName);
                Directory.CreateDirectory(path);
                if (!System.IO.File.Exists(fullFileName))
                {
                    Filedata.SaveAs(fullFileName);                    
                }
                return Content(virtualPath);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}
