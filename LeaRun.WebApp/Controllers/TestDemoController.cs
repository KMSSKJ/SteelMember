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

        //泛型接口 
        interface IFanXingable<T>
        {
            T SayNumber(T t);
        }
        //泛型类 
        public class FanXing<T>// where T:class,new() 
        {
            T[] arryStrings = new T[]
            {

            };
            //索引器 
            public T this[int index]
            {
                get
                {
                    return arryStrings[index];
                }
                set
                {
                    arryStrings[index] = value;
                }
            }
            //泛型方法 
            public T Add(T str)
            {
                return str;
            }
        }

        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }


            public void Add<T>(T t)
            {
                Console.WriteLine(t);
            }
        }
        static void Main(string[] args)
        { //实例化 
            FanXing<string> fanXing = new FanXing<string>();
            fanXing[0] = "aaa1"; fanXing[1] = "aaa2";
            fanXing[2] = "aaa3"; fanXing[3] = "aaa4";
            FanXing<int> fanXingInt = new FanXing<int>();
            fanXingInt[0] = 1; fanXingInt[2] = 2;
            fanXingInt[3] = 4; fanXingInt[4] = 3;
            FanXing<Person> fanXingClass = new FanXing<Person>();
            fanXingClass[0] = new Person() { Name = "guo", Age = 21 };
            fanXingClass[0] = new Person() { Name = "yang", Age = 20 };
            //方法泛型 泛型是数据类型的抽象 数据类型是对象的抽象 
            Console.ReadKey();
        }
        //实现接口 
        public class MyClass : IFanXingable<Int32>
        {
            public int SayNumber(int t)
            {
                return t;
            }
        }
    }
}
