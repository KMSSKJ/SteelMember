﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeaRun.WebApp.Areas.SteelMember.Controllers
{
    using System.Text;
    using System.IO;
    using System.Threading;
    using System.Linq.Expressions;
    using System.Collections.Specialized;
    using System.Data;
    using System.Reflection;
    using System.Diagnostics;
    using System.Collections;
    using SteelMember.Controllers;
    using System.Data.OleDb;
    using Ninject;
    using Business;
    using Repository.SteelMember.IBLL;
    using Entity.SteelMember;
    using Utilities;
    using Models;
    using WebApp.Controllers;
    using LeaRun.Repository;
    using LeaRun.Entity;

    public class FileController : BaseController
    {
        public Base_ModuleBll Sys_modulebll = new Base_ModuleBll();
        public Base_ButtonBll Sys_buttonbll = new Base_ButtonBll();

        [Inject]
        public TreeIBLL TreeCurrent { get; set; }
        [Inject]
        public FileIBLL MemberLibraryCurrent { get; set; }
        [Inject]
        public ProjectInfoIBLL ProjectInfoCurrent { get; set; }
        [Inject]
        public MemberMaterialIBLL MemberMaterialCurrent { get; set; }
        [Inject]
        public RawMaterialIBLL RawMaterialCurrent { get; set; }
        [Inject]
        public MemberUnitIBLL MemberUnitCurrent { get; set; }
        [Inject]
        public MemberProcessIBLL MemberProcessCurrent { get; set; }

        [Inject]
        public ProcessManagementIBLL ProcessManagementCurrent { get; set; }

        #region 已注销代码
        ///// <summary>
        ///// 文件在线查看
        ///// </summary>
        ///// <param name="KeyValue"></param>
        ///// <returns></returns>
        //#region 文件在线查看
        //public ActionResult OnlineLook(string KeyValue)
        //{
        //    ViewData["aa"] = RequestFile(KeyValue);
        //    return View();
        //}

        //protected string RequestFile(string KeyValue)
        //{
        //    string file = "";
        //    RMC_MemberLibrary ht = new RMC_MemberLibrary();
        //    if (!string.IsNullOrEmpty(KeyValue))
        //    {
        //        int fileid = Convert.ToInt32(KeyValue);
        //        ht = MemberLibraryCurrent.Find(f => f.MemberID == fileid).SingleOrDefault();
        //        if (ht != null)
        //        {
        //            if (ht.MemberName != null)
        //            {

        //                //file = /*System.Web.HttpContext.Current.Server.MapPath("~")+ "Plugins/SteelMember" +*/ht.FullPath.Replace("~","");
        //                // file = ht.FileGuid + ht.FileExtensions;
        //            }

        //        }
        //    }

        //    string extension = file.Substring(file.LastIndexOf('.'));//获取文件后缀名
        //    string time = DateTime.Now.ToString("yyyyMMdd"); //file.Substring(0, file.LastIndexOf('\\'));//获取日期格式(yyyy-MM-dd)的文件名
        //    #region 创建pdf、swf保存的路径

        //    string pdf_path = "LoadFile\\file_pdf\\" + time + "\\";//设置存放路径
        //    string pdfUrl = System.Web.HttpContext.Current.Server.MapPath("~") + "Plugins/SteelMember/" + pdf_path;//保存文件路径

        //    string swf_path = "LoadFile\\file_swf\\" + time + "\\";//设置swf存放路径
        //    string swfUrl = System.Web.HttpContext.Current.Server.MapPath("~") + "Plugins/SteelMember/" + swf_path;//保存文件路径
        //    if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~") + "Plugins/SteelMember/LoadFile/file_pdf") || Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~") + "Plugins/SteelMember/LoadFile/file_swf"))
        //    {

        //        Directory.Delete(System.Web.HttpContext.Current.Server.MapPath("~") + "Plugins/SteelMember/LoadFile/file_pdf", true);//pdf路径
        //        Directory.Delete(System.Web.HttpContext.Current.Server.MapPath("~") + "Plugins/SteelMember/LoadFile/file_swf", true);//创建swf路径
        //    }
        //    //如果文件不存在，则从新创建，文件夹以(yyyy-MM-d)的格式创建
        //    if (!Directory.Exists(pdf_path) || !Directory.Exists(swf_path))
        //    {
        //        Directory.CreateDirectory(pdfUrl);//创建pdf路径
        //        Directory.CreateDirectory(swfUrl);//创建swf路径
        //    }
        //    #endregion

        //    #region 文件重新命名为pdf、swf格式
        //    string fname = file.Substring(file.LastIndexOf('\\') + 1);//获取文件名称，去除后缀名
        //    string filename = fname.Substring(0, fname.LastIndexOf('.'));//获取文件名称，去除后缀名

        //    pdfUrl += filename + ".pdf";//把文件更改成pdf格式
        //    swfUrl += filename + ".swf";//把文件更改成swf格式
        //    string strfile = filename + ".swf";//把文件更改成swf格式
        //    #endregion

        //    bool blpdf = System.IO.File.Exists(pdfUrl);//判断pdf是否已经存在
        //    bool blswf = System.IO.File.Exists(swfUrl);//判断swf是否已经存在
        //    if (extension == ".doc" || extension == ".docx")
        //    {
        //        Document doc = new Document(System.Web.HttpContext.Current.Server.MapPath("~") + ht.FullPath.Replace("~", ""));
        //        doc.Save(pdfUrl, Aspose.Words.SaveFormat.Pdf);
        //        PDFConvertToSWF(pdfUrl, swfUrl);
        //    }
        //    else if (extension == ".ppt" || extension == ".pptx")
        //    {
        //        Presentation pres = new Presentation(System.Web.HttpContext.Current.Server.MapPath("~") + "Plugins/SteelMember/" + ht.FullPath.Replace("~", ""));
        //        pres.Save(pdfUrl, Aspose.Slides.Export.SaveFormat.Pdf);
        //        PDFConvertToSWF(pdfUrl, swfUrl);
        //        // pdf2swf.PDFConvertToSWF("G:/doc.pdf", " G:/1.swf");
        //    }
        //    else if (extension == ".xls" || extension == ".xlsx")
        //    {
        //        Workbook book = new Workbook(System.Web.HttpContext.Current.Server.MapPath("~") + "Plugins/SteelMember/" + ht.FullPath.Replace("~", ""));
        //        book.Save(pdfUrl, Aspose.Cells.SaveFormat.Pdf);
        //        //office2pdf.XLSConvertToPDF(ArticlePath, pdfpath);
        //        PDFConvertToSWF(pdfUrl, swfUrl);
        //    }
        //    else if (extension == ".pdf")
        //    {
        //        PDFConvertToSWF(file, swfUrl);
        //    }
        //    #region 文件转换---暂不用
        //    //if (!blpdf && !blswf)//pdf和swf都不存在
        //    //{
        //    //    //FileUpload ffile = new FileUpload();
        //    //   // ffile.SaveAs(pdfUrl); //讲pdf文件保存到服务器
        //    //    if (extension == ".doc" || extension == ".docx")
        //    //    {
        //    //        DOCConvertToPDF(file, pdfUrl);
        //    //        PDFConvertToSWF(pdfUrl, swfUrl);//pdf转换swf保存在服务器
        //    //    }
        //    //    else if (extension == ".xls" || extension == ".xlsx")
        //    //    {
        //    //        XLSConvertToPDF(file, pdfUrl);
        //    //        PDFConvertToSWF(pdfUrl, swfUrl);//pdf转换swf保存在服务器
        //    //    }
        //    //    else if (extension == ".ppt" || extension == ".pptx")
        //    //    {
        //    //        PPTConvertToPDF(file, pdfUrl);
        //    //        PDFConvertToSWF(pdfUrl, swfUrl);//pdf转换swf保存在服务器
        //    //    }
        //    //    else
        //    //    {
        //    //        PDFConvertToSWF(file, swfUrl);//pdf转换swf保存在服务器
        //    //    }

        //    //}
        //    //if (blpdf && !blswf)
        //    //{
        //    //    PDFConvertToSWF(pdfUrl, swfUrl);//pdf转换swf保存在服务器
        //    //}
        //    #endregion
        //    RMC_MemberLibrary model = new RMC_MemberLibrary();
        //    return model.MemberName = "Plugins/SteelMember/LoadFile/file_swf/" + time + "/" + strfile;
        //}
        //public string PDFConvertToSWF(string pdfPath, string swfPath)
        //{
        //    //string cmdStr = HttpContext.Current .Server.MapPath("~/SWFTools/pdf2swf.exe");
        //    string cmdStr = System.Web.HttpContext.Current.Server.MapPath("~~/SWFTools/pdf2swf.exe");
        //    string sourcePath = @"""" + pdfPath + @"""";
        //    string targetPath = @"""" + swfPath + @"""";
        //    string argsStr = "  -t " + sourcePath + " -s flashversion=9 -o " + targetPath;
        //    return ExcutedCmd(cmdStr, argsStr);
        //}
        //public static void ExecuteCmd(string cmd, string args)
        //{
        //    using (Process p = new Process())
        //    {
        //        p.StartInfo.FileName = cmd;
        //        p.StartInfo.Arguments = args;
        //        p.StartInfo.UseShellExecute = false;
        //        p.StartInfo.RedirectStandardOutput = false;
        //        p.StartInfo.CreateNoWindow = true;
        //        p.Start();
        //        p.PriorityClass = ProcessPriorityClass.Normal;
        //        p.WaitForExit();
        //    }
        //}
        //private static string ExcutedCmd(string cmd, string args)
        //{
        //    string b = "1";
        //    using (Process p = new Process())
        //    {
        //        ProcessStartInfo psi = new ProcessStartInfo(cmd, args);
        //        p.StartInfo = psi;
        //        psi.UseShellExecute = false;
        //        p.Start();
        //        p.WaitForExit();
        //    }
        //    return b;
        //}
        //#endregion

        ///// <summary>
        ///// 返回上一级
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[ValidateInput(false)]
        //public virtual ActionResult ReturnSuperior(string KeyValue)
        //{
        //    try
        //    {
        //        int FolderId = Convert.ToInt32(KeyValue);
        //        //string UserId = ManageProvider.Provider.Current().UserId;
        //        RMC_MemberLibrary ListData = null;
        //        RMC_Tree ListData1 = null;
        //        List<RMC_Tree> listtree = TreeCurrent.Find(t => t.TreeID == FolderId).ToList();
        //        List<RMC_MemberLibrary> listfile = MemberLibraryCurrent.Find(t => t.TreeID == FolderId).ToList();
        //        if (listfile.Count() > 0 && listtree.Count() > 0)
        //        {
        //            ListData = MemberLibraryCurrent.Find(t => t.TreeID == FolderId).First();
        //        }
        //        else if (listfile.Count() > 0)
        //        {
        //            ListData = MemberLibraryCurrent.Find(t => t.TreeID == FolderId).FirstOrDefault();
        //        }
        //        else if (listtree.Count() > 0)
        //        {
        //            ListData1 = TreeCurrent.Find(t => t.TreeID == FolderId).FirstOrDefault();
        //        }
        //        else
        //        {
        //            ListData = null;
        //        }
        //        if (ListData == null)
        //        {
        //            return Content(ListData1.ToJson());
        //        }
        //        else
        //        {
        //            return Content(ListData.ToJson());
        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// 审核文件（夹）
        ///// </summary>
        ///// <param name="FolderId"></param>
        ///// <returns></returns>
        //public ActionResult ReviewFolder(string FolderId)
        //{
        //    try
        //    {
        //        int folderid = Convert.ToInt32(FolderId);
        //        var tree = TreeCurrent.Find(u => u.TreeID == folderid).First();
        //        tree.ModifiedTime = DateTime.Now;
        //        tree.IsReview = 1;
        //        TreeCurrent.Modified(tree);
        //        List<RMC_MemberLibrary> files = MemberLibraryCurrent.Find(u => u.TreeID == folderid).ToList();
        //        if (files.Count() > 0)
        //        {
        //            foreach (var item in files)
        //            {
        //                var file = MemberLibraryCurrent.Find(u => u.TreeID == folderid).First();
        //                tree.ModifiedTime = DateTime.Now;
        //                tree.IsReview = 1;
        //                MemberLibraryCurrent.Modified(file);
        //            }
        //        }
        //        List<RMC_Tree> trees = TreeCurrent.Find(u => u.TreeID > 0).ToList();
        //        List<RMC_Tree> trees1 = trees.Where(t => t.ParentID == folderid).ToList();
        //        for (int i = 0; i < trees.Count(); i++)
        //        {
        //            foreach (var item in trees1)
        //            {
        //                tree = trees.Where(u => u.TreeID == item.TreeID).First();
        //                tree.ModifiedTime = DateTime.Now;
        //                tree.IsReview = 1;
        //                TreeCurrent.Modified(tree);
        //                files = MemberLibraryCurrent.Find(u => u.TreeID == item.TreeID).ToList();
        //                if (files.Count() > 0)
        //                {
        //                    foreach (var itemfile in files)
        //                    {
        //                        var file1 = MemberLibraryCurrent.Find(u => u.MemberID == itemfile.MemberID).First();
        //                        file1.ModifiedTime = DateTime.Now;
        //                        file1.IsReview = 1;
        //                        MemberLibraryCurrent.Modified(file1);
        //                    }
        //                }
        //            }
        //        }
        //        return Content(new JsonMessage { Success = true, Code = "1", Message = "审核成功。" }.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
        //    }
        //}

        ///// <summary>
        ///// 审核文件
        ///// </summary>
        ///// <param name="FolderId"></param>
        ///// <returns></returns>
        //public ActionResult ReviewFile(string MemberID)
        //{
        //    try
        //    {
        //        int fileid = Convert.ToInt32(MemberID);
        //        var file = MemberLibraryCurrent.Find(u => u.MemberID == fileid).First();
        //        file.ModifiedTime = DateTime.Now;
        //        file.IsReview = 1;
        //        MemberLibraryCurrent.Modified(file);
        //        return Content(new JsonMessage { Success = true, Code = "1", Message = "删除成功。" }.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(new JsonMessage
        //        {
        //            Success = false,
        //            Code = "-1",
        //            Message = "操作失败：" + ex.Message
        //        }.ToString());
        //    }
        //}

        ///// <summary>
        ///// 文档申请（文件或文件夹）
        ///// </summary>
        ///// <param name="MemberID"></param>
        ///// <returns></returns>      
        //public ActionResult WikipediFile(string MemberID, string FolderId, string Sort)
        //{
        //    try
        //    {
        //        if (Sort != "1")
        //        {
        //            int folderid = Convert.ToInt32(FolderId);
        //            var tree = TreeCurrent.Find(u => u.TreeID == folderid).First();
        //            tree.ModifiedTime = DateTime.Now;
        //            tree.IsMenu = 0;
        //            TreeCurrent.Modified(tree);
        //            List<RMC_MemberLibrary> files = MemberLibraryCurrent.Find(u => u.TreeID == folderid).ToList();
        //            if (files.Count() > 0)
        //            {
        //                foreach (var item in files)
        //                {
        //                    var file = MemberLibraryCurrent.Find(u => u.TreeID == folderid).First();
        //                    tree.ModifiedTime = DateTime.Now;
        //                    tree.IsMenu = 0;
        //                    MemberLibraryCurrent.Modified(file);
        //                }
        //            }
        //            List<RMC_Tree> trees = TreeCurrent.Find(u => u.TreeID > 0).ToList();
        //            List<RMC_Tree> trees1 = trees.Where(t => t.ParentID == folderid).ToList();
        //            for (int i = 0; i < trees.Count(); i++)
        //            {
        //                foreach (var item in trees1)
        //                {
        //                    tree = trees.Where(u => u.TreeID == item.TreeID).First();
        //                    tree.ModifiedTime = DateTime.Now;
        //                    //tree.IsAuthorize = 1;
        //                    TreeCurrent.Modified(tree);
        //                    files = MemberLibraryCurrent.Find(u => u.TreeID == item.TreeID).ToList();
        //                    if (files.Count() > 0)
        //                    {
        //                        foreach (var itemfile in files)
        //                        {
        //                            var file1 = MemberLibraryCurrent.Find(u => u.MemberID == itemfile.MemberID).First();
        //                            file1.ModifiedTime = DateTime.Now;
        //                            //file1.IsAuthorize = 1;
        //                            MemberLibraryCurrent.Modified(file1);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            int fileid = Convert.ToInt32(MemberID);
        //            var file = MemberLibraryCurrent.Find(u => u.MemberID == fileid).First();
        //            file.ModifiedTime = DateTime.Now;
        //            //file.IsAuthorize = 1;
        //            MemberLibraryCurrent.Modified(file);
        //        }
        //        return Content(new JsonMessage { Success = true, Code = "1", Message = "申请成功。" }.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
        //    }
        //}

        ///// <summary>
        ///// 申请表单视图
        ///// </summary>
        ///// <returns></returns>
        ////[ManagerPermission(PermissionMode.Enforce)]
        //public ActionResult ApplicationForm()
        //{
        //    return View();
        //}

        ///// <summary>
        ///// 文件表单视图
        ///// </summary>
        ///// <returns></returns>
        ////[ManagerPermission(PermissionMode.Enforce)]
        //public ActionResult AuthorizeFileForm()
        //{
        //    return View();
        //}

        //public ActionResult SubmitAuthorizeFileForm(string MemberID, string FolderId, string Sort)
        //{
        //    try
        //    {
        //        if (Sort != "1")
        //        {
        //            int folderid = Convert.ToInt32(FolderId);
        //            var tree = TreeCurrent.Find(u => u.TreeID == folderid).First();
        //            tree.ModifiedTime = DateTime.Now;
        //            tree.IsMenu = 0;
        //            //tree.ItemID = Convert.ToInt32(Folderid);
        //            TreeCurrent.Modified(tree);
        //            List<RMC_MemberLibrary> files = MemberLibraryCurrent.Find(u => u.TreeID == folderid).ToList();
        //            if (files.Count() > 0)
        //            {
        //                foreach (var item in files)
        //                {
        //                    var file = MemberLibraryCurrent.Find(u => u.TreeID == folderid).First();
        //                    tree.ModifiedTime = DateTime.Now;
        //                    tree.IsMenu = 0;
        //                    MemberLibraryCurrent.Modified(file);
        //                }
        //            }
        //            List<RMC_Tree> trees = TreeCurrent.Find(u => u.TreeID > 0).ToList();
        //            List<RMC_Tree> trees1 = trees.Where(t => t.ParentID == folderid).ToList();
        //            for (int i = 0; i < trees.Count(); i++)
        //            {
        //                foreach (var item in trees1)
        //                {
        //                    tree = trees.Where(u => u.TreeID == item.TreeID).First();
        //                    tree.ModifiedTime = DateTime.Now;
        //                    tree.IsMenu = 0;
        //                    TreeCurrent.Modified(tree);
        //                    files = MemberLibraryCurrent.Find(u => u.TreeID == item.TreeID).ToList();
        //                    if (files.Count() > 0)
        //                    {
        //                        foreach (var itemfile in files)
        //                        {
        //                            var file1 = MemberLibraryCurrent.Find(u => u.MemberID == itemfile.MemberID).First();
        //                            file1.ModifiedTime = DateTime.Now;
        //                            //file1.IsAuthorize = 1;
        //                            MemberLibraryCurrent.Modified(file1);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            int fileid = Convert.ToInt32(MemberID);
        //            var file = MemberLibraryCurrent.Find(u => u.MemberID == fileid).First();
        //            file.ModifiedTime = DateTime.Now;
        //            //file.IsAuthorize = 1;
        //            MemberLibraryCurrent.Modified(file);
        //        }
        //        return Content(new JsonMessage { Success = true, Code = "1", Message = "申请成功。" }.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
        //    }
        //}

        ///// <summary>
        ///// 标记删除文件（夹）
        ///// </summary>
        ///// <param name="FolderId"></param>
        ///// <returns></returns>
        //public ActionResult MarkFolder(string FolderId)
        //{
        //    try
        //    {
        //        int folderid = Convert.ToInt32(FolderId);
        //        var tree = TreeCurrent.Find(u => u.TreeID == folderid).First();
        //        tree.ModifiedTime = DateTime.Now;
        //        tree.DeleteFlag = 1;
        //        TreeCurrent.Modified(tree);
        //        List<RMC_MemberLibrary> files = MemberLibraryCurrent.Find(u => u.TreeID == folderid).ToList();
        //        if (files.Count() > 0)
        //        {
        //            foreach (var item in files)
        //            {
        //                var file = MemberLibraryCurrent.Find(u => u.TreeID == folderid).First();
        //                tree.ModifiedTime = DateTime.Now;
        //                tree.DeleteFlag = 1;
        //                MemberLibraryCurrent.Modified(file);
        //            }
        //        }
        //        List<RMC_Tree> trees = TreeCurrent.Find(u => u.TreeID > 0).ToList();
        //        List<RMC_Tree> trees1 = trees.Where(t => t.ParentID == folderid).ToList();
        //        for (int i = 0; i < trees.Count(); i++)
        //        {
        //            foreach (var item in trees1)
        //            {
        //                tree = trees.Where(u => u.TreeID == item.TreeID).First();
        //                tree.ModifiedTime = DateTime.Now;
        //                tree.DeleteFlag = 1;
        //                TreeCurrent.Modified(tree);
        //                files = MemberLibraryCurrent.Find(u => u.TreeID == item.TreeID).ToList();
        //                if (files.Count() > 0)
        //                {
        //                    foreach (var itemfile in files)
        //                    {
        //                        var file1 = MemberLibraryCurrent.Find(u => u.MemberID == itemfile.MemberID).First();
        //                        file1.ModifiedTime = DateTime.Now;
        //                        file1.DeleteFlag = 1;
        //                        MemberLibraryCurrent.Modified(file1);
        //                    }
        //                }
        //            }
        //        }
        //        return Content(new JsonMessage { Success = true, Code = "1", Message = "删除成功。" }.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
        //    }
        //}

        ///// <summary>
        ///// 标记(删除)文件
        ///// </summary>
        ///// <param name="FolderId"></param>
        ///// <returns></returns>
        //public ActionResult MarkFile(string MemberID)
        //{
        //    try
        //    {
        //        int fileid = Convert.ToInt32(MemberID);
        //        var file = MemberLibraryCurrent.Find(u => u.MemberID == fileid).First();
        //        file.ModifiedTime = DateTime.Now;
        //        file.DeleteFlag = 1;
        //        MemberLibraryCurrent.Modified(file);
        //        return Content(new JsonMessage { Success = true, Code = "1", Message = "删除成功。" }.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(new JsonMessage
        //        {
        //            Success = false,
        //            Code = "-1",
        //            Message = "操作失败：" + ex.Message
        //        }.ToString());
        //    }
        //}

        ///// <summary>
        ///// 文件重命名
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult FileReName()
        //{
        //    return View();
        //}

        ///// <summary>
        ///// 【控制测量文档管理】返回文件对象JSON
        ///// </summary>
        ///// <param name="KeyValue">主键值</param>
        ///// <returns></returns>
        //[HttpPost]
        //[ValidateInput(false)]
        //// [LoginAuthorize]
        //public virtual ActionResult SetFileForm(string MemberID)
        //{
        //    int fileid = Convert.ToInt32(MemberID);
        //    RMC_MemberLibrary entity = MemberLibraryCurrent.Find(f => f.MemberID == fileid).SingleOrDefault();
        //    //string JsonData = entity.ToJson();
        //    ////自定义
        //    //JsonData = JsonData.Insert(1, Sys_FormAttributeBll.Instance.GetBuildForm(KeyValue));
        //    return Content(entity.ToJson());
        //}

        ///// <summary>
        ///// 文件 重命名 提交保存
        ///// </summary>
        ///// <param name="KeyValue">主键</param>
        ///// <param name="MemberName">重命名</param>
        ///// <returns></returns>
        //public ActionResult SubmitFileReName(/*string MemberID, string MemberName*/RMC_MemberLibrary file)
        //{
        //    try
        //    {
        //        //int fileid = Convert.ToInt32(MemberID);
        //        RMC_MemberLibrary entity = MemberLibraryCurrent.Find(f => f.MemberID == file.MemberID).SingleOrDefault();
        //        entity.MemberName = file.MemberName;
        //        entity.ModifiedTime = DateTime.Now;
        //        MemberLibraryCurrent.Modified(entity);
        //        int IsOk = 1;
        //        return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "编辑成功。" }.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
        //    }

        //}

        ///// <summary>
        ///// 移动文件（夹）
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult MoveLocation()
        //{
        //    return View();
        //}

        ///// <summary>
        ///// 移动文件（夹）提交保存
        ///// </summary>
        ///// <param name="KeyValue">主键</param>
        ///// <param name="MoveFolderId">选择移动文件夹Id</param>
        ///// <param name="sort">分类0-文件、1-文件夹</param>
        ///// <returns></returns>
        //public ActionResult SubmitMoveLocation(string KeyValue, string MoveFolderId, string Sort)
        //{
        //    try
        //    {
        //        int keyvalue = Convert.ToInt32(KeyValue);
        //        int IsOk = 0;
        //        if (Sort == "1")
        //        {
        //            RMC_MemberLibrary networkfile = MemberLibraryCurrent.Find(t => t.TreeID == keyvalue).SingleOrDefault(); ;
        //            //networkfile.MemberID = keyvalue;
        //            networkfile.TreeID = Convert.ToInt32(MoveFolderId);
        //            MemberLibraryCurrent.Modified(networkfile);
        //            IsOk = 1;
        //        }
        //        else
        //        {
        //            RMC_Tree networkfolder = TreeCurrent.Find(t => t.TreeID == keyvalue).SingleOrDefault();
        //            networkfolder.ParentID = Convert.ToInt32(MoveFolderId);
        //            TreeCurrent.Modified(networkfolder);
        //            IsOk = 1;

        //        }
        //        return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "移动成功。" }.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
        //    }

        //}
        #endregion

        public ActionResult Partial1()
        {
            return View();
        }

        /// <summary>
        /// 【控制测量文档管理】返回树JONS
        /// </summary>
        /// <returns></returns>      
        public ActionResult TreeJson(string TreeId)
        {
            //var userid = 1;
            //List<DOC_R_Tree_Role> TRR = new List<DOC_R_Tree_Role>();
            //var userrole = UserRoleRepository.Find(ur => ur.UserID == userid).SingleOrDefault();
            //TRR = TreeRoleRepository.Find(tr => tr.RoleID == userrole.RoleID).ToList();
            int ItemId = Convert.ToInt32(TreeId);
            List<RMC_Tree> list = TreeCurrent.Find(t => t.ItemID == ItemId).ToList();
            List<TreeJsonEntity> TreeList = new List<TreeJsonEntity>();
            foreach (RMC_Tree item in list)
            {
                TreeJsonEntity tree = new TreeJsonEntity();
                bool hasChildren = false;
                List<RMC_Tree> childnode = list.FindAll(t => t.ParentID == item.TreeID);
                if (childnode.Count > 0)
                {
                    hasChildren = true;
                }
                tree.id = item.TreeID.ToString();
                tree.text = item.TreeName;
                tree.value = item.TreeID.ToString();
                tree.isexpand = item.State == 1 ? true : false;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.parentId = item.ParentID.ToString();
                //tree.iconCls = item.Icon != null ? "/Content/Images/Icon16/" + item.Icon : item.Icon;
                TreeList.Add(tree);
            }
            return Content(TreeList.TreeToJson());
        }

        /// <summary>
        /// 文件表单视图
        /// </summary>
        /// <returns></returns>
        //[ManagerPermission(PermissionMode.Enforce)]
        public virtual ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 【控制测量文档管理】返回对象JSON
        /// </summary>
        /// <param name="KeyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult SetForm(string KeyValue)
        {
            string JsonData = "";
            if (KeyValue != "")
            {
                int fileid = Convert.ToInt32(KeyValue);
                RMC_MemberLibrary entity = MemberLibraryCurrent.Find(f => f.MemberID == fileid).SingleOrDefault();
                JsonData = entity.ToJson();
            }
            //自定义
            //JsonData = JsonData.Insert(1, Sys_FormAttributeBll.Instance.GetBuildForm(KeyValue));
            return Content(JsonData);
        }
        /// <summary>
        /// 提交文件表单
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="KeyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult SubmitData(RMC_MemberLibrary entity, string KeyValue)
        {
            int fileid = Convert.ToInt32(KeyValue);
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    RMC_MemberLibrary Oldentity = MemberLibraryCurrent.Find(f => f.MemberID == fileid).SingleOrDefault();//获取没更新之前实体对象
                    Oldentity.MemberName = entity.MemberName;//给旧实体重新赋值
                    Oldentity.ModifiedTime = DateTime.Now;
                    MemberLibraryCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                    //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    entity.IsProcess = 0;
                    entity.IsRawMaterial = 0;
                    MemberLibraryCurrent.Add(entity);
                    IsOk = 1;
                    //this.WriteLog(IsOk, entity, null, KeyValue, Message);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                //this.WriteLog(-1, entity, null, KeyValue, "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        /// <summary>
        /// 删除（销毁）文件
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public ActionResult DeleteFile(string MemberID)
        {
            try
            {
                int _MemberId = Convert.ToInt32(MemberID);
                //删除构件
                List<int> ids = new List<int>();
                ids.Add(_MemberId);
                MemberLibraryCurrent.Remove(ids);
                //

                //删除构件原材料

                var MemberMaterial = MemberMaterialCurrent.Find(f => f.MemberId == _MemberId).ToList();
                if (MemberMaterial.Count() > 0)
                {
                    List<int> ids1 = new List<int>();
                    for (int i = 0; i < MemberMaterial.Count(); i++)
                    {
                        ids1.Add(Convert.ToInt32(MemberMaterial[i].MemberId));
                    }
                    MemberMaterialCurrent.Remove(ids1);
                }
                //

                //删除构件制程
                var MemberProcess = MemberProcessCurrent.Find(f => f.MemberId == _MemberId).ToList();
                if (MemberProcess.Count() > 0)
                {
                    List<int> ids2 = new List<int>();
                    for (int i = 0; i < MemberProcess.Count(); i++)
                    {
                        ids2.Add(Convert.ToInt32(MemberProcess[i].MemberId));
                    }
                    MemberProcessCurrent.Remove(ids2);
                }
                //

                return Content(new JsonMessage { Success = true, Code = "1", Message = "删除成功。" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage
                {
                    Success = false,
                    Code = "-1",
                    Message = "操作失败：" + ex.Message
                }.ToString());
            }
        }

        #region 数据查询与呈现
        public ActionResult Index()
        {
            //Session.Add("focusUrl", this.Request.Url.ToString());
            //return View(GetCurrentUserPermission());
            return View();
        }

        public ActionResult QueryPage()
        {
            return View();
        }

        /// <summary>
        /// 【控制测量文档管理】返回文件（夹）列表JSON
        /// </summary>
        /// <param name="keywords">文件名搜索条件</param>
        /// <param name="FolderId">文件夹ID</param>
        /// <param name="IsPublic">是否公共 1-公共、0-我的</param>
        /// <returns></returns>         
        public ActionResult GridListJson(FileViewModel model, string TreeId, JqGridParam jqgridparam, string IsPublic, string ParameterJson)
        {
            try
            {
                #region 查询条件拼接
                if (ParameterJson != null)
                {
                    if (ParameterJson != "[{\"MemberModel\":\"\",\"InBeginTime\":\"\",\"InEndTime\":\"\"}]")
                    {
                        List<FileViewModel> query_member = JsonHelper.JonsToList<FileViewModel>(ParameterJson);
                        for (int i = 0; i < query_member.Count(); i++)
                        {
                            model.MemberModel = query_member[i].MemberModel;
                            model.InBeginTime = query_member[i].InBeginTime;
                            model.InEndTime = query_member[i].InEndTime;
                        }
                    }
                }

                Expression<Func<RMC_MemberLibrary, bool>> func = ExpressionExtensions.True<RMC_MemberLibrary>();
                Func<RMC_MemberLibrary, bool> func1 = f => f.TreeID != 0;

                var _a = model.MemberModel != null && model.MemberModel.ToString() != "";
                var _b = model.InBeginTime != null && model.InBeginTime.ToString() != "0001/1/1 0:00:00";
                var _c = model.InEndTime != null && model.InEndTime.ToString() != "0001/1/1 0:00:00";

                if (_a && _b && _c)
                {
                    func = func.And(f => f.MemberModel.Contains(model.MemberModel) && f.UploadTime >= model.InBeginTime && f.UploadTime <= model.InEndTime);
                    func1 = f => f.MemberModel.Contains(model.MemberModel) && f.UploadTime >= model.InBeginTime && f.UploadTime <= model.InEndTime;
                }
                else if (_a && !_b && !_c)
                {
                    func = func.And(f => f.MemberModel.Contains(model.MemberModel));
                    func1 = f => f.MemberModel.Contains(model.MemberModel);
                }
                else if (_b && !_a && !_c)
                {
                    func = func.And(f => f.UploadTime >= model.InBeginTime);
                    func1 = f => f.UploadTime >= model.InBeginTime;
                }
                else if (_c && !_b && !_a)
                {
                    func = func.And(f => f.UploadTime <= model.InEndTime);
                    func1 = f => f.UploadTime <= model.InEndTime;
                }
                else if (_a && _b && !_c)
                {
                    func = func.And(f => f.MemberModel.Contains(model.MemberModel) && f.UploadTime >= model.InBeginTime);
                    func1 = f => f.MemberModel.Contains(model.MemberModel) && f.UploadTime >= model.InBeginTime;
                }
                else if (_a && _c && !_b)
                {
                    func = func.And(f => f.MemberModel.Contains(model.MemberModel) && f.UploadTime <= model.InEndTime);
                    func1 = f => f.MemberModel.Contains(model.MemberModel) && f.UploadTime <= model.InEndTime;
                }
                else if (_b && _c && !_a)
                {
                    func = func.And(f => f.UploadTime >= model.InBeginTime && f.UploadTime <= model.InEndTime);
                    func1 = f => f.UploadTime >= model.InBeginTime && f.UploadTime <= model.InEndTime;
                }
                #endregion

                var MemberList_ = new List<RMC_MemberLibrary>();
                Stopwatch watch = CommonHelper.TimerStart();
                int total = 0;
                List<RMC_MemberLibrary> MemberList = new List<RMC_MemberLibrary>();
                if (TreeId == "")
                {
                    func.And(f => f.DeleteFlag != 1 & f.MemberID > 0);
                    MemberList = MemberList_ = MemberLibraryCurrent.FindPage<string>(jqgridparam.page
                                             , jqgridparam.rows
                                             , func
                                             , false
                                             , f => f.UploadTime.ToString()
                                             , out total
                                             ).ToList();
                }
                else
                {
                    int _id = Convert.ToInt32(TreeId);
                    var list = GetSonId(_id).ToList();

                    list.Add(TreeCurrent.Find(p => p.TreeID == _id).Single());

                    foreach (var item in list)
                    {
                        var _MemberList = MemberLibraryCurrent.Find(m => m.TreeID == item.TreeID).ToList();
                        if (_MemberList.Count() > 0)
                        {
                            MemberList = MemberList.Concat(_MemberList).ToList();
                        }
                    }

                    MemberList = MemberList.Where(func1).OrderByDescending(f => f.UploadTime).ToList();
                    MemberList_ = MemberList.Take(jqgridparam.rows * jqgridparam.page).Skip(jqgridparam.rows * (jqgridparam.page - 1)).ToList();
                    total = MemberList.Count();
                }
                var JsonData = new
                {
                    total = total / jqgridparam.rows + 1,
                    page = jqgridparam.page,
                    records = total,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = MemberList_,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }

        }

        //获取树字节子节点(自循环)
        public IEnumerable<RMC_Tree> GetSonId(int p_id)
        {
            List<RMC_Tree> list = TreeCurrent.Find(p => p.ParentID == p_id).ToList();
            return list.Concat(list.SelectMany(t => GetSonId(t.TreeID)));
        }
        #endregion

        #region Excel 数据导入
        // 将Excel导入数据库  

        public ActionResult ImportFile()
        {
            return View();
        }

        //public ActionResult SubmitImportFile()
        //{
        //    List<string> list = new List<string>();

        //    for (int i = 0; i < 5; i++)
        //    {
        //        list.Add(i.ToString());
        //    }

        //    return View(list);
        //}

        public ContentResult SubmitImportFile(string KeyValue, RMC_MemberLibrary MemberLibrary)
        {
            try
            {
                HttpFileCollectionBase Filedatas = Request.Files;

                //HttpPostedFileBase file = Request.Files["FileUpload"];//获取上传的文件  
                string FileName;
                string savePath;
                string Photo = "";
                //int IsOk = 1;
                if (Filedatas.Count == 0)
                {
                    return Content("文件不能为空");
                }
                else
                {
                    var Filedata = Request.Files[0];
                    string filename = Path.GetFileName(Filedata.FileName);
                    int filesize = Filedata.ContentLength;//获取上传文件的大小单位为字节byte  
                    string fileEx = System.IO.Path.GetExtension(filename);//获取上传文件的扩展名  
                    string NoFileName = System.IO.Path.GetFileNameWithoutExtension(filename);//获取无扩展名的文件名  
                    int Maxsize = 10000 * 1024;//定义上传文件的最大空间大小为10M  
                    string FileType = ".xls,.xlsx";//定义上传文件的类型字符串  

                    FileName = NoFileName + DateTime.Now.ToString("yyyyMMddhhmmssffff") + fileEx;
                    if (!FileType.Contains(fileEx))
                    {
                        return Content("文件类型不对，只能导入xls和xlsx格式的文件");
                    }
                    if (filesize >= Maxsize)
                    {
                        return Content("上传的文件不能超过10M");
                    }
                    //string path = AppDomain.CurrentDomain.BaseDirectory + "uploads\\excel\\";
                    string virtualPath = string.Format("~/Resource/Document/NetworkDisk/Excel");
                    string fullFileName = this.Server.MapPath(virtualPath);
                    //如果文件存在，则删除
                    if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~") + "~/Resource/Document/NetworkDisk/Excel"))
                    {

                        Directory.Delete(System.Web.HttpContext.Current.Server.MapPath("~") + "~/Resource/Document/NetworkDisk/Excel", true);//pdf路径
                    }
                    //如果文件不存在，则从新创建，文件夹以(yyyy-MM-d)的格式创建
                    if (!Directory.Exists(fullFileName))
                    {
                        Directory.CreateDirectory(fullFileName);//创建swf路径
                    }
                    savePath = Path.Combine(fullFileName, FileName);
                    Filedata.SaveAs(savePath);
                }
                string result = string.Empty;
                string strConn;
                //strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + savePath + "; " + "Extended Properties=Excel 8.0;";  
                strConn = "Provider=Microsoft.Ace.OleDb.12.0;" + "data source=" + savePath + ";Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'"; //此连接可以操作.xls与.xlsx文件 (支持Excel2003 和 Excel2007 的连接字符串)  

                //OleDbDataAdapter myCommand = new OleDbDataAdapter("select * from [Sheet1$]", strConn);  
                //连接串  
                OleDbConnection conn = new OleDbConnection(strConn);
                conn.Open();
                //返回Excel的架构，包括各个sheet表的名称,类型，创建时间和修改时间等　  
                DataTable dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" });
                //包含excel中表名的字符串数组  
                string[] strTableNames = new string[dtSheetName.Rows.Count];
                for (int k = 0; k < dtSheetName.Rows.Count; k++)
                {
                    strTableNames[k] = dtSheetName.Rows[k]["TABLE_NAME"].ToString();
                }
                OleDbDataAdapter myCommand = null;
                DataTable dt = new DataTable();
                //从指定的表明查询数据,可先把所有表明列出来供用户选择  
                string strExcel = "select*from[" + strTableNames[0] + "]";
                myCommand = new OleDbDataAdapter(strExcel, strConn);
                //myCommand.Fill(dt);  
                DataSet myDataSet = new DataSet();
                myCommand.Fill(myDataSet, "ExcelInfo");
                // Data.Deleted();
                DataTable table = myDataSet.Tables["ExcelInfo"].DefaultView.ToTable();
                if (table.Columns.Count != 37)
                {
                    return Content("文件数据格式不正确");
                }
                int count = 0;
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    if (table.Rows[i].IsNull(0))
                    {
                        count++;
                    }
                }
                if (table.Rows.Count == count)
                {
                    return Content("文件不包含任何数据");
                }
                else
                {
                    int _count = 0;
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        if (!table.Rows[i].IsNull(0))
                        {
                            string MemberModel = table.Rows[i][0].ToString();
                            var _MemberLibrary = MemberLibraryCurrent.Find(f => f.MemberModel == MemberModel).SingleOrDefault();
                            if (_MemberLibrary == null)
                            {
                                int key_value = Convert.ToInt32(KeyValue);
                                MemberLibrary.TreeID = key_value;
                                var tree = TreeCurrent.Find(f => f.TreeID == key_value).SingleOrDefault();
                                MemberLibrary.MemberName = tree.TreeName;
                                MemberLibrary.UploadTime = DateTime.Now;
                                MemberLibrary.Sort = 1;
                                MemberLibrary.MemberModel = table.Rows[i][0].ToString();
                                MemberModel = MemberLibrary.MemberModel;
                                char[] Number = MemberModel.ToArray();
                                string MemberNumbering = "";
                                for (int I = 0; I < Number.Length; I++)
                                {
                                    if (("0123456789").IndexOf(Number[I] + "") != -1)
                                    {
                                        MemberNumbering += Number[I];
                                    }
                                }

                                MemberLibrary.MemberNumbering = (MemberNumbering + "-" + DateTime.Now.ToString("yyyyMMddhhmmssffff")).ToString();
                                MemberLibrary.SectionalArea = Convert.ToDecimal(table.Rows[i][1]);
                                MemberLibrary.SurfaceArea = Convert.ToDecimal(table.Rows[i][2]);
                                MemberLibrary.TheoreticalWeight = table.Rows[i][3].ToString();
                                MemberLibrary.SectionalSize_h = Convert.ToInt32(table.Rows[i][4]);
                                MemberLibrary.SectionalSizeB = Convert.ToInt32(table.Rows[i][5]);
                                MemberLibrary.SectionalSize_b = Convert.ToInt32(table.Rows[i][6]);
                                MemberLibrary.SectionalSizeD = Convert.ToDecimal(table.Rows[i][7]);
                                MemberLibrary.SectionalSize_d = Convert.ToInt32(table.Rows[i][8]);
                                MemberLibrary.SectionalSize_t = Convert.ToDecimal(table.Rows[i][9]);
                                MemberLibrary.SectionalSize_r = Convert.ToDecimal(table.Rows[i][10]);
                                MemberLibrary.SectionalSize_r1 = Convert.ToDecimal(table.Rows[i][11]);
                                MemberLibrary.InertiaDistance_x = Convert.ToDecimal(table.Rows[i][12]);
                                MemberLibrary.InertiaDistance_y = Convert.ToDecimal(table.Rows[i][13]);
                                MemberLibrary.InertiaDistance_x0 = Convert.ToDecimal(table.Rows[i][14]);
                                MemberLibrary.InertiaDistance_y0 = Convert.ToDecimal(table.Rows[i][15]);
                                MemberLibrary.InertiaDistance_x1 = Convert.ToDecimal(table.Rows[i][16]);
                                MemberLibrary.InertiaDistance_y1 = Convert.ToDecimal(table.Rows[i][17]);
                                MemberLibrary.InertiaDistance_u = Convert.ToDecimal(table.Rows[i][18]);
                                MemberLibrary.InertiaRadius_x = Convert.ToDecimal(table.Rows[i][19]);
                                MemberLibrary.InertiaRadius_x0 = Convert.ToDecimal(table.Rows[i][20]);
                                MemberLibrary.InertiaRadius_y = Convert.ToDecimal(table.Rows[i][21]);
                                MemberLibrary.InertiaRadius_y0 = Convert.ToDecimal(table.Rows[i][22]);
                                MemberLibrary.InertiaRadius_u = Convert.ToDecimal(table.Rows[i][23]);
                                MemberLibrary.SectionalModulus_x = Convert.ToDecimal(table.Rows[i][24]);
                                MemberLibrary.SectionalModulus_y = Convert.ToDecimal(table.Rows[i][25]);
                                MemberLibrary.SectionalModulus_x0 = Convert.ToDecimal(table.Rows[i][26]);
                                MemberLibrary.SectionalModulus_y0 = Convert.ToDecimal(table.Rows[i][27]);
                                MemberLibrary.SectionalModulus_u = Convert.ToDecimal(table.Rows[i][28]);
                                MemberLibrary.GravityCenterDistance_0 = Convert.ToDecimal(table.Rows[i][29]);
                                MemberLibrary.GravityCenterDistance_x0 = Convert.ToDecimal(table.Rows[i][30]);
                                MemberLibrary.GravityCenterDistance_y0 = Convert.ToDecimal(table.Rows[i][31]);
                                MemberLibrary.MemberUnit = table.Rows[i][32].ToString();
                                MemberLibrary.UnitPrice = table.Rows[i][33].ToString();
                                string CAD_Drawing = "1.png";
                                string Model_Drawing = "1.png";
                                string Icon = "1.png";
                                if (table.Rows[i][34].ToString() != "")
                                {
                                    CAD_Drawing = table.Rows[i][34].ToString().Trim();
                                    if (CAD_Drawing != "1.png")
                                    {
                                        Photo += CAD_Drawing + "、";

                                    }
                                }
                                if (table.Rows[i][35].ToString() != "")
                                {
                                    Model_Drawing = table.Rows[i][35].ToString().Trim();
                                    if (Model_Drawing!="1.png")
                                    {
                                        Photo += Model_Drawing + "、";
                                    }
                                }
                                if (table.Rows[i][36].ToString() != "")
                                {
                                    Icon = table.Rows[i][36].ToString().Trim();
                                    if (Icon!="1.png")
                                    {
                                        Photo += Icon + "、";
                                    }
                                }

                                MemberLibrary.CAD_Drawing = CAD_Drawing;
                                MemberLibrary.Model_Drawing = Model_Drawing;
                                MemberLibrary.Icon = Icon;
                                MemberLibrary.IsProcess = 0;
                                MemberLibrary.IsRawMaterial = 0;
                                MemberLibraryCurrent.Add(MemberLibrary);
                                // Data.AddData(data);
                            }
                            else
                            {
                                _count++;
                            }
                        }
                    }
                    if (table.Rows.Count - count == _count || (_count == 1 && table.Rows.Count - count == _count))
                    {
                        return Content("操作失败：要导入的数据在该构件类型或其他构件类型下已存在");
                    }
                }
                Session["photo"] = Photo;
                if (Photo=="")
                {
                    return Content("1");
                }
                else
                {
                    return Content("2");
                }
                
                //return View(Photo);
            }
            catch (Exception ex)
            {
                return Content("操作失败：" + ex.Message);
            }
        }
        #endregion

        #region JqGrid导出Excel
        /// <summary>
        /// 导出Excell模板
        /// </summary>
        /// <returns></returns>
        public void GetExcellTemperature(string ImportId)
        {

            DataTable data = new DataTable();
            string fileName = "导入构件模板.xlsx";
            //string TableHeader = "构件模板";
            string DataColumn = "型号 | 截面面积/cm²| 外表面积/(m²/m)|理论重量/(㎏/m)|";
            DataColumn += "h|B|b|D|d| t|r|r1|Ix | Ix0 | Ix1 | Iy | Iy0 | Iy1 | Iu | ix |";
            DataColumn += " iy | ix0 | iy0 | iu | Wx | Wy | Wx0 | Wy0 | Wu | Z0 | X0 | Yu|单位|单价|图纸|模型|图标";
            DeriveExcel.DataTableToExcel(data, DataColumn.Split('|'), fileName);
        }

        ///// <summary>                                                                                            
        ///// 获取要导出表头字段                                                                                   
        ///// </summary>                                                                                          
        ///// <returns></returns>                                                                                 
        //public ActionResult GetDeriveExcelColumn()
        //{
        //    string JsonColumn = GZipHelper.Uncompress(CookieHelper.GetCookie("JsonColumn_DeriveExcel"));
        //    return Content(JsonColumn);
        //}
        ///// <summary>                                                                                            
        ///// 导出Excel                                                                                            
        ///// </summary>                                                                                           
        ///// <param name="ExportField">要导出字段</param>                                                         
        //public void GetDeriveExcel(string ExportField)
        //{
        //    string JsonColumn = GZipHelper.Uncompress(CookieHelper.GetCookie("JsonColumn_DeriveExcel"));
        //    string JsonData = GZipHelper.Uncompress(CookieHelper.GetCookie("JsonData_DeriveExcel"));
        //    string JsonFooter = GZipHelper.Uncompress(CookieHelper.GetCookie("JsonFooter_DeriveExcel"));
        //    string fileName = GZipHelper.Uncompress(CookieHelper.GetCookie("FileName_DeriveExcel"));
        //    DeriveExcel.JqGridToExcel(JsonColumn, JsonData, ExportField, fileName);
        //}
        ///// <summary>
        ///// 写入数据到Cookie
        ///// </summary>
        ///// <param name="JsonColumn">表头</param>
        ///// <param name="JsonData">数据</param>
        ///// <param name="JsonFooter">底部合计</param>
        //[ValidateInput(false)]
        //public void SetDeriveExcel(string JsonColumn, string JsonData, string JsonFooter, string FileName,string TableHeader)
        //{
        //    CookieHelper.WriteCookie("JsonColumn_DeriveExcel", GZipHelper.Compress(JsonColumn));
        //    CookieHelper.WriteCookie("JsonData_DeriveExcel", GZipHelper.Compress(JsonData));
        //    CookieHelper.WriteCookie("JsonFooter_DeriveExcel", GZipHelper.Compress(JsonFooter));
        //    CookieHelper.WriteCookie("FileName_DeriveExcel", GZipHelper.Compress(FileName));
        //    CookieHelper.WriteCookie("TabelHeader_DeriveExcel", GZipHelper.Compress(TableHeader));
        //}

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <returns></returns>
        public ActionResult DeriveFile()
        {
            return View();
        }

        #endregion

        #region 文件上传、下载
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadFile()
        {
            ViewData["_photo"] = Session["photo"].ToString();
            return View();
        }



        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public ActionResult SubmitUpLoadFile(string KeyValue, string Img, RMC_MemberLibrary file, HttpPostedFileBase Filedata)/*RMC_MemberLibrary File,  */
        {
            try
            {
                int key_value = Convert.ToInt32(KeyValue);
                RMC_MemberLibrary oldentity = new RMC_MemberLibrary();
                RMC_ProjectInfo oldentity1 = new RMC_ProjectInfo();
                int IsOk = 0;
                //没有文件上传，直接返回
                if (Filedata == null || string.IsNullOrEmpty(Filedata.FileName) || Filedata.ContentLength == 0)
                {
                    return HttpNotFound();
                }
                //获取文件完整文件名(包含绝对路径)
                //文件存放路径格式：/Resource/Document/NetworkDisk/{日期}/{guid}.{后缀名}
                //例如：/Resource/Document/Email/20130913/43CA215D947F8C1F1DDFCED383C4D706.jpg
                string fileGuid = CommonHelper.GetGuid;
                string FileEextension = Path.GetExtension(Filedata.FileName);
                string uploadDate = DateTime.Now.ToString("yyyyMMdd");
                //string UserId = ManageProvider.Provider.Current().UserId;
                string NewPath = "";
                string virtualPath = "";
                string virtualPath1 = "";
                string fullFileName = "";
                string UserId = "System";
                string filename = Filedata.FileName.Substring(0, Filedata.FileName.LastIndexOf('.'));//获取文件名称，去除后缀名
                oldentity = MemberLibraryCurrent.Find(f => f.MemberID == key_value).SingleOrDefault();
                oldentity1 = ProjectInfoCurrent.Find(f => f.ProjectId == key_value).SingleOrDefault();
                if (Img == "Logo")
                {
                    NewPath = string.Format("~/Resource/Document/NetworkDisk/{0}/{1}/{2}", UserId, "Project", filename);
                    virtualPath = this.Server.MapPath(NewPath);// UserId,
                    fullFileName = virtualPath + "/" + Filedata.FileName;

                    if (oldentity1.ProjectLogo == null || oldentity1.ProjectLogo == "")
                    {
                        oldentity1.ProjectLogo = "1.png";
                    }
                    else
                    {
                        string filename1 = oldentity1.ProjectLogo.Substring(0, oldentity1.ProjectLogo.LastIndexOf('.'));//获取文件名称，去除后缀名
                        virtualPath1 = "~/Resource/Document/NetworkDisk/System/Project/" + filename1;
                        if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~") + virtualPath1))
                        {
                            Directory.Delete(System.Web.HttpContext.Current.Server.MapPath("~") + virtualPath1, true);//pdf路径
                        }
                    }

                    //创建文件夹，保存文件
                    Directory.CreateDirectory(virtualPath);
                    Filedata.SaveAs(fullFileName);
                    oldentity1.ProjectLogo = Filedata.FileName;
                    oldentity1.ModifiedTime = DateTime.Now;
                    ProjectInfoCurrent.Modified(oldentity1);
                }
                else if (Img == "Background")
                {
                    NewPath = string.Format("~/Resource/Document/NetworkDisk/{0}/{1}/{2}", UserId, "Project", filename);
                    virtualPath = this.Server.MapPath(NewPath);// UserId,
                    fullFileName = virtualPath + "/" + Filedata.FileName;

                    if (oldentity1.ProjectBackground == null || oldentity1.ProjectBackground == "")
                    {
                        oldentity1.ProjectBackground = "1.png";
                    }
                    else
                    {
                        string filename1 = oldentity1.ProjectBackground.Substring(0, oldentity1.ProjectBackground.LastIndexOf('.'));//获取文件名称，去除后缀名
                        virtualPath1 = "~/Resource/Document/NetworkDisk/System/Project/" + filename1;
                        if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~") + virtualPath1))
                        {
                            Directory.Delete(System.Web.HttpContext.Current.Server.MapPath("~") + virtualPath1, true);//pdf路径
                        }
                    }

                    //创建文件夹，保存文件
                    Directory.CreateDirectory(virtualPath);
                    Filedata.SaveAs(fullFileName);
                    oldentity1.ProjectBackground = Filedata.FileName;
                    oldentity1.ModifiedTime = DateTime.Now;
                    ProjectInfoCurrent.Modified(oldentity1);
                }
                IsOk = 1;
                //IsOk = DataFactory.Database().Insert<Base_NetworkFile>(entity).ToString();
                var JsonData = new
                {
                    Status = IsOk,
                    NetworkFile = oldentity,
                };
                NewPath = NewPath.Replace("~", "../..");
                return Content(NewPath + "/" + Filedata.FileName);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        [HttpPost]
        public ContentResult UploadPhoto()
        {
            HttpFileCollectionBase fileToUpload = Request.Files;
            List<string> list = new List<string>();
            int count = 0;
            foreach (string file in fileToUpload)
            {
                var fileName = fileToUpload[file].FileName.Trim();
                string suffix = fileName.Substring(fileName.LastIndexOf('.') + 1);

                if (suffix != "jpg" && suffix != "gif" && suffix != "jpeg" && suffix != "png")
                {
                    count++;
                }
                list.Add(fileName);

            }

            string photo = Session["photo"].ToString();

            string _photo = photo.Substring(0, photo.Length - 1);
            string[] fileArray = _photo.Split('、');
            var _list = fileArray.ToList();

            if (_list.Count() != list.Count())
            {
                return Content("必须同时选择要上传的所有图片！");
            }
            else
            {
                if (count > 0)
                {
                    return Content("所选的图片格式有误，请重选！");
                }
                else
                {
                    var diffArr = list.Where(c => !_list.Contains(c)).ToList();
                    if (diffArr.Count == 0)
                    {
                        foreach (string file in fileToUpload)
                        {
                            var fileName = fileToUpload[file].FileName.Trim();
                            string filename1 = fileName.Substring(0, fileName.LastIndexOf('.'));//获取文件名称，去除后缀名
                            string virtualPath = "/Resource/Document/NetworkDisk/System/Member/" + filename1;

                            string path = Server.MapPath(virtualPath);
                            if (Directory.Exists(path))
                            {
                                Directory.Delete(path, true);//pdf路径
                            }
                            //创建文件夹，保存文件
                            Directory.CreateDirectory(path);

                            fileToUpload[file].SaveAs(path + "/" + fileName);
                        }
                        return Content("操作成功！");
                    }
                    else
                    {
                        return Content("所选的图片名称有误，请重选！");
                    }
                }
            }
        }

        public bool exists(string[] _list)
        {
            string _photo = Session["photo"].ToString();

            _photo = _photo.Substring(0, _photo.Length - 1);
            string[] FieldInfo = _photo.Split('、');

            var sameArr = FieldInfo.Intersect(_list).ToArray();

            //找出不同的元素(即交集的补集)
            var diffArr = FieldInfo.Where(c => !_list.Contains(c)).ToArray();
            //if (_list. == FieldInfo)
            //{
            //    return true;
            //}
            return true;
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

                string filename = Filedata.FileName;
                string filename1 = filename.Substring(0, filename.LastIndexOf('.'));//获取文件名称，去除后缀名
                string NewPath = string.Format("/Resource/Document/NetworkDisk/System/{0}/{1}", "Member", filename1);
                long filesize = Filedata.ContentLength;
                string FileEextension = Path.GetExtension(Filedata.FileName);

                // virtualPath = string.Format("/Content/Images/Avatar/{0}/{1}/{2}{3}", UserId, uploadDate, fileGuid, FileEextension);
                string fullFileName = this.Server.MapPath(NewPath + "/");
                //创建文件夹，保存文件
                string path = Path.GetDirectoryName(fullFileName);
                Directory.CreateDirectory(path);
                if (!System.IO.File.Exists(fullFileName))
                {
                    Filedata.SaveAs(fullFileName + filename);
                }
                return Content("../.." + NewPath + "/" + filename);
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }


        /// <summary>
        /// 获取文件类型、文件图标
        /// </summary>
        /// <param name="Eextension">后缀名</param>
        /// <param name="FileType">要返回文件类型</param>
        /// <param name="Icon">要返回文件图标</param>
        public void DocumentType(string Eextension, ref string FileType, ref string Icon)
        {
            string _FileType = "";
            string _Icon = "";
            switch (Eextension)
            {
                case ".exe":
                    _FileType = "PC软件";
                    _Icon = "exe";
                    break;
                case ".docx":
                    _FileType = "word文件";
                    _Icon = "doc";
                    break;
                case ".doc":
                    _FileType = "word文件";
                    _Icon = "doc";
                    break;
                case ".xlsx":
                    _FileType = "excel文件";
                    _Icon = "xls";
                    break;
                case ".xls":
                    _FileType = "excel文件";
                    _Icon = "xls";
                    break;
                case ".pptx":
                    _FileType = "ppt文件";
                    _Icon = "ppt";
                    break;
                case ".ppt":
                    _FileType = "ppt文件";
                    _Icon = "ppt";
                    break;
                case ".txt":
                    _FileType = "记事本文件";
                    _Icon = "txt";
                    break;
                case ".pdf":
                    _FileType = "pdf文件";
                    _Icon = "pdf";
                    break;
                case ".zip":
                    _FileType = "压缩文件";
                    _Icon = "zip";
                    break;
                case ".rar":
                    _FileType = "压缩文件";
                    _Icon = "rar";
                    break;
                case ".png":
                    _FileType = "png图片";
                    _Icon = "png";
                    break;
                case ".gif":
                    _FileType = "gif图片";
                    _Icon = "gif";
                    break;
                case ".jpg":
                    _FileType = "jpg图片";
                    _Icon = "jpeg";
                    break;
                case ".mp3":
                    _FileType = "mp3文件";
                    _Icon = "mp3";
                    break;
                case ".html":
                    _FileType = "html文件";
                    _Icon = "html";
                    break;
                case ".css":
                    _FileType = "css文件";
                    _Icon = "css";
                    break;
                case ".mpeg":
                    _FileType = "mpeg文件";
                    _Icon = "mpeg";
                    break;
                case ".pds":
                    _FileType = "pds文件";
                    _Icon = "pds";
                    break;
                case ".ttf":
                    _FileType = "ttf文件";
                    _Icon = "ttf";
                    break;
                case ".swf":
                    _FileType = "swf文件";
                    _Icon = "swf";
                    break;
                case ".apk":
                    _FileType = "手机";
                    _Icon = "apk";
                    break;
                default:
                    _FileType = "其他文件";
                    _Icon = "new";
                    //return "else.png";
                    break;
            }
            FileType = _FileType;
            Icon = _Icon;
        }

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <returns></returns>
        public void Download(string KeyValue)
        {
            int fileid = Convert.ToInt32(KeyValue);
            RMC_MemberLibrary entity = MemberLibraryCurrent.Find(f => f.MemberID == fileid).SingleOrDefault();
            string filename = Server.UrlDecode(entity.MemberName);//返回客户端文件名称
            string filepath = Server.UrlDecode(entity.FullPath);//文件虚拟路径
            if (FileDownHelper.FileExists(filepath))
            {
                FileDownHelper.DownLoadold(filepath, filename);
            }
        }
        #endregion

        #region 编辑构件信息
        [ValidateInput(false)]
        public virtual ActionResult MemberForm()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        //[LoginAuthorize]
        public virtual ActionResult SetMemberForm(string KeyValue)
        {
            int memberid = Convert.ToInt32(KeyValue);
            RMC_MemberLibrary entity = MemberLibraryCurrent.Find(f => f.MemberID == memberid).SingleOrDefault();
            //string JsonData = entity.ToJson();
            ////自定义
            //JsonData = JsonData.Insert(1, Sys_FormAttributeBll.Instance.GetBuildForm(KeyValue));
            return Content(entity.ToJson());
        }
        /// <summary>
        /// 提交文件夹表单
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="KeyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        //[LoginAuthorize]
        public virtual ActionResult SubmitMember(RMC_MemberLibrary entity, string KeyValue, string TreeId, string ItemID)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int keyvalue = Convert.ToInt32(KeyValue);
                    RMC_MemberLibrary Oldentity = MemberLibraryCurrent.Find(t => t.MemberID == keyvalue).SingleOrDefault();//获取没更新之前实体对象
                    Oldentity.MemberModel = entity.MemberModel;
                    Oldentity.SectionalArea = entity.SectionalArea;
                    Oldentity.SurfaceArea = entity.SurfaceArea;
                    Oldentity.TheoreticalWeight = entity.TheoreticalWeight;
                    Oldentity.SectionalSize_h = entity.SectionalSize_h;
                    Oldentity.SectionalSizeB = entity.SectionalSizeB;
                    Oldentity.SectionalSize_b = entity.SectionalSize_b;
                    Oldentity.SectionalSizeD = entity.SectionalSizeD;
                    Oldentity.SectionalSize_d = entity.SectionalSize_d;
                    Oldentity.SectionalSize_t = entity.SectionalSize_t;
                    Oldentity.SectionalSize_r = entity.SectionalSize_r;
                    Oldentity.SectionalSize_r1 = entity.SectionalSize_r1;
                    Oldentity.InertiaDistance_x = entity.InertiaDistance_x;
                    Oldentity.InertiaDistance_y = entity.InertiaDistance_y;
                    Oldentity.InertiaDistance_x0 = entity.InertiaDistance_x0;
                    Oldentity.InertiaDistance_y0 = entity.InertiaDistance_y0;
                    Oldentity.InertiaDistance_x1 = entity.InertiaDistance_x1;
                    Oldentity.InertiaDistance_y1 = entity.InertiaDistance_y1;
                    Oldentity.InertiaDistance_u = entity.InertiaDistance_u;
                    Oldentity.InertiaRadius_x = entity.InertiaRadius_x;
                    Oldentity.InertiaRadius_x0 = entity.InertiaRadius_x0;
                    Oldentity.InertiaRadius_y = entity.InertiaRadius_y;
                    Oldentity.InertiaRadius_y0 = entity.InertiaRadius_y0;
                    Oldentity.InertiaRadius_u = entity.InertiaRadius_u;
                    Oldentity.SectionalModulus_x = entity.SectionalModulus_x;
                    Oldentity.SectionalModulus_y = entity.SectionalModulus_y;
                    Oldentity.SectionalModulus_x0 = entity.SectionalModulus_x0;
                    Oldentity.SectionalModulus_y0 = entity.SectionalModulus_y0;
                    Oldentity.SectionalModulus_u = entity.SectionalModulus_u;
                    Oldentity.GravityCenterDistance_0 = entity.GravityCenterDistance_0;
                    Oldentity.GravityCenterDistance_x0 = entity.GravityCenterDistance_x0;
                    Oldentity.GravityCenterDistance_y0 = entity.GravityCenterDistance_y0;
                    if (entity.CAD_Drawing == null)
                    {
                        entity.CAD_Drawing = "1.png";
                    }
                    string filename = System.IO.Path.GetFileName(entity.CAD_Drawing);
                    Oldentity.CAD_Drawing = filename;

                    if (entity.Model_Drawing == null)
                    {
                        entity.Model_Drawing = "1.png";
                    }
                    string filename1 = System.IO.Path.GetFileName(entity.Model_Drawing);
                    Oldentity.Model_Drawing = filename1;

                    if (entity.Icon == null)
                    {
                        entity.Icon = "1.png";
                    }
                    string filename2 = System.IO.Path.GetFileName(entity.Icon);
                    Oldentity.Icon = filename2;

                    Oldentity.MemberUnit = entity.MemberUnit;
                    Oldentity.UnitPrice = entity.UnitPrice;
                    MemberLibraryCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                             //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    int tree_id = Convert.ToInt32(TreeId);
                    RMC_MemberLibrary entitys = new RMC_MemberLibrary();
                    var tree = TreeCurrent.Find(f => f.TreeID == tree_id).SingleOrDefault();
                    entitys.MemberName = tree.TreeName;
                    entitys.ParentID = tree_id;
                    entitys.TreeID = tree_id;
                    entitys.Sort = 1;

                    entitys.MemberModel = entity.MemberModel;
                    string MemberModel = entity.MemberModel;
                    char[] Number = MemberModel.ToArray();
                    string MemberNumbering = "";
                    for (int I = 0; I < Number.Length; I++)
                    {
                        if (("0123456789").IndexOf(Number[I] + "") != -1)
                        {
                            MemberNumbering += Number[I];
                        }
                    }
                    entitys.UploadTime = DateTime.Now;
                    entitys.MemberNumbering = (MemberNumbering + "-" + DateTime.Now.ToString("yyyyMMddhhmmssffff")).ToString();
                    entitys.SectionalArea = entity.SectionalArea;
                    entitys.SurfaceArea = entity.SurfaceArea;
                    entitys.TheoreticalWeight = entity.TheoreticalWeight;
                    entitys.SectionalSize_h = entity.SectionalSize_h;
                    entitys.SectionalSizeB = entity.SectionalSizeB;
                    entitys.SectionalSize_b = entity.SectionalSize_b;
                    entitys.SectionalSizeD = entity.SectionalSizeD;
                    entitys.SectionalSize_d = entity.SectionalSize_d;
                    entitys.SectionalSize_t = entity.SectionalSize_t;
                    entitys.SectionalSize_r = entity.SectionalSize_r;
                    entitys.SectionalSize_r1 = entity.SectionalSize_r1;
                    entitys.InertiaDistance_x = entity.InertiaDistance_x;
                    entitys.InertiaDistance_y = entity.InertiaDistance_y;
                    entitys.InertiaDistance_x0 = entity.InertiaDistance_x0;
                    entitys.InertiaDistance_y0 = entity.InertiaDistance_y0;
                    entitys.InertiaDistance_y1 = entity.InertiaDistance_y1;
                    entitys.InertiaDistance_u = entity.InertiaDistance_u;
                    entitys.InertiaRadius_x = entity.InertiaRadius_x;
                    entitys.InertiaRadius_x0 = entity.InertiaRadius_x0;
                    entitys.InertiaRadius_y = entity.InertiaRadius_y;
                    entitys.InertiaRadius_y0 = entity.InertiaRadius_y0;
                    entitys.InertiaRadius_u = entity.InertiaRadius_u;
                    entitys.SectionalModulus_x = entity.SectionalModulus_x;
                    entitys.SectionalModulus_y = entity.SectionalModulus_y;
                    entitys.SectionalModulus_x0 = entity.SectionalModulus_x0;
                    entitys.SectionalModulus_y0 = entity.SectionalModulus_y0;
                    entitys.SectionalModulus_u = entity.SectionalModulus_u;
                    entitys.GravityCenterDistance_0 = entity.GravityCenterDistance_0;
                    entitys.GravityCenterDistance_x0 = entity.GravityCenterDistance_x0;
                    entitys.GravityCenterDistance_y0 = entity.GravityCenterDistance_y0;
                    entitys.MemberUnit = entity.MemberUnit;
                    entitys.UnitPrice = entity.UnitPrice;

                    if (entity.CAD_Drawing == null)
                    {
                        entity.CAD_Drawing = "1.png";
                    }
                    string filename = System.IO.Path.GetFileName(entity.CAD_Drawing);
                    entitys.CAD_Drawing = filename;

                    if (entity.Model_Drawing == null)
                    {
                        entity.Model_Drawing = "1.png";
                    }
                    string filename1 = System.IO.Path.GetFileName(entity.Model_Drawing);
                    entitys.Model_Drawing = filename1;

                    if (entity.Icon == null)
                    {
                        entity.Icon = "1.png";
                    }
                    string filename2 = System.IO.Path.GetFileName(entity.Icon);
                    entitys.Icon = filename2;

                    entitys.IsRawMaterial = 0;
                    entitys.IsProcess = 0;
                    MemberLibraryCurrent.Add(entitys);
                    IsOk = 1;
                    //this.WSectionalSize_r = entity.SectionalSize_r;riteLog(IsOk, entity, null, KeyValue, Message);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                //this.WriteInertiaDistance_x1 = entity.InertiaDistance_x1;Log(-1, entity, null, KeyValue, "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        public virtual ActionResult DeleteMember(RMC_MemberLibrary entity, string KeyValue)
        {

            int IsOk = -1;
            int key_value = Convert.ToInt32(KeyValue);
            //删除构件
            List<int> ids = new List<int>();
            ids.Add(key_value);
            IsOk = MemberLibraryCurrent.Remove(ids);
            //

            //删除构件材料
            List<int> ids1 = new List<int>();
            var MemberMaterial = MemberMaterialCurrent.Find(f => f.MemberId == key_value).ToList();
            if (MemberMaterial.Count() > 0)
            {
                foreach (var item in MemberMaterial)
                {
                    ids1.Add(Convert.ToInt32(item.MemberId));
                }
                IsOk = MemberMaterialCurrent.Remove(ids1);
            }
            //

            //删除构件制程
            List<int> ids2 = new List<int>();
            var MemberProcess = MemberProcessCurrent.Find(f => f.MemberId == key_value).ToList();
            if (MemberProcess.Count() > 0)
            {
                foreach (var item in MemberProcess)
                {
                    ids1.Add(Convert.ToInt32(item.MemberId));
                }
                IsOk = MemberProcessCurrent.Remove(ids1);
            }

            return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "删除成功。" }.ToString());
        }

        #endregion

        #region 构件数控参数
        [ValidateInput(false)]
        public virtual ActionResult CNCParameterForm()
        {
            return View();
        }
        #endregion

        #region 构件图纸模型管理 
        public ActionResult DrawingManagement(string KeyValue, string FileNamePath)
        {
            if (KeyValue == "")
            {
                ViewData["CADDrawing"] = FileNamePath;
                ViewData["ModelDrawing"] = FileNamePath;
            }
            else
            {
                int memberid = Convert.ToInt32(KeyValue);
                var ht = MemberLibraryCurrent.Find(f => f.MemberID == memberid).SingleOrDefault();
                if (ht.CAD_Drawing == null || ht.CAD_Drawing == "")
                {
                    ht.CAD_Drawing = "1.png";
                }
                if (ht.Model_Drawing == null || ht.Model_Drawing == "")
                {
                    ht.Model_Drawing = "1.png";
                }
                var filename = ht.CAD_Drawing.Substring(0, ht.CAD_Drawing.LastIndexOf('.'));//获取文件名称，去除后缀名
                string virtualPath = "../../Resource/Document/NetworkDisk/System/Member/" + filename + "/";

                var filename1 = ht.Model_Drawing.Substring(0, ht.Model_Drawing.LastIndexOf('.'));//获取文件名称，去除后缀名
                string virtualPath1 = "../../Resource/Document/NetworkDisk/System/Member/" + filename1 + "/";
                //string fullFileName = this.Server.MapPath(virtualPath);
                var file = virtualPath + ht.CAD_Drawing;
                var file1 = virtualPath1 + ht.Model_Drawing;

                ViewData["CADDrawing"] = file;
                ViewData["ModelDrawing"] = file1;
                ViewData["MemberId"] = memberid;
            }
            return View();
        }
        public ActionResult CADDrawingManagement(string KeyValue, string FileNamePath)
        {
            if (KeyValue == "")
            {
                ViewData["CADDrawing"] = FileNamePath;
                // ViewData["ModelDrawing"] = FileNamePath;
            }
            else
            {
                int memberid = Convert.ToInt32(KeyValue);
                var ht = MemberLibraryCurrent.Find(f => f.MemberID == memberid).SingleOrDefault();
                if (ht.CAD_Drawing == null || ht.CAD_Drawing == "")
                {
                    ht.CAD_Drawing = "1.png";
                }
                if (ht.Model_Drawing == null || ht.Model_Drawing == "")
                {
                    ht.Model_Drawing = "1.png";
                }
                var filename = ht.CAD_Drawing.Substring(0, ht.CAD_Drawing.LastIndexOf('.'));//获取文件名称，去除后缀名
                string virtualPath = "../../Resource/Document/NetworkDisk/System/Member/" + filename + "/";

                var filename1 = ht.Model_Drawing.Substring(0, ht.Model_Drawing.LastIndexOf('.'));//获取文件名称，去除后缀名
                string virtualPath1 = "../../Resource/Document/NetworkDisk/System/Member/" + filename1 + "/";
                //string fullFileName = this.Server.MapPath(virtualPath);
                var file = virtualPath + ht.CAD_Drawing;
                var file1 = virtualPath1 + ht.Model_Drawing;

                ViewData["CADDrawing"] = file;
                //ViewData["ModelDrawing"] = file1;
                ViewData["MemberId"] = memberid;
            }
            return View();
        }
        public ActionResult ModelDrawingManagement(string KeyValue, string FileNamePath)
        {
            if (KeyValue == "")
            {
                // ViewData["CADDrawing"] = FileNamePath;
                ViewData["ModelDrawing"] = FileNamePath;
            }
            else
            {
                int memberid = Convert.ToInt32(KeyValue);
                var ht = MemberLibraryCurrent.Find(f => f.MemberID == memberid).SingleOrDefault();
                if (ht.CAD_Drawing == null || ht.CAD_Drawing == "")
                {
                    ht.CAD_Drawing = "1.png";
                }
                if (ht.Model_Drawing == null || ht.Model_Drawing == "")
                {
                    ht.Model_Drawing = "1.png";
                }
                var filename = ht.CAD_Drawing.Substring(0, ht.CAD_Drawing.LastIndexOf('.'));//获取文件名称，去除后缀名
                string virtualPath = "../../Resource/Document/NetworkDisk/System/Member/" + filename + "/";

                var filename1 = ht.Model_Drawing.Substring(0, ht.Model_Drawing.LastIndexOf('.'));//获取文件名称，去除后缀名
                string virtualPath1 = "../../Resource/Document/NetworkDisk/System/Member/" + filename1 + "/";
                //string fullFileName = this.Server.MapPath(virtualPath);
                var file = virtualPath + ht.CAD_Drawing;
                var file1 = virtualPath1 + ht.Model_Drawing;

                //ViewData["CADDrawing"] = file;
                ViewData["ModelDrawing"] = file1;
                ViewData["MemberId"] = memberid;
            }
            return View();
        }

        /// <summary>
        /// 删除图纸
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult DelDrawing(string KeyValue, string CAD, string Model)
        {
            int memberid = Convert.ToInt32(KeyValue);
            string filename = "";
            var file = MemberLibraryCurrent.Find(u => u.MemberID == memberid).First();
            if (CAD != "" || CAD != null && Model == "")
            {
                filename = file.CAD_Drawing.Substring(0, file.CAD_Drawing.LastIndexOf('.'));//获取文件名称，去除后缀名
            }
            else
            {
                filename = file.Model_Drawing.Substring(0, file.Model_Drawing.LastIndexOf('.'));//获取文件名称，去除后缀名
            }
            string virtualPath = "~/Resource/Document/NetworkDisk/System/Member/" + filename;
            //string fullFileName = this.Server.MapPath(virtualPath);
            if (Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~") + virtualPath))
            {
                Directory.Delete(System.Web.HttpContext.Current.Server.MapPath("~") + virtualPath, true);//pdf路径
            }
            file.ModifiedTime = DateTime.Now;
            file.CAD_Drawing = "1.png";
            MemberLibraryCurrent.Modified(file);

            return Content(new JsonMessage { Success = true, Code = "1", Message = "删除成功。" }.ToString());
        }
        #endregion

        #region 打印报表
        public ActionResult PrintPage()
        {
            return View();
        }

        #endregion

        #region 构件原材料用量
        public ActionResult RawMaterialsDosageIndex()
        {
            return View();
        }
        public ActionResult GridListJsonMemberMaterial(string KeyValue, JqGridParam jqgridparam)
        {
            int _KeyValue = Convert.ToInt32(KeyValue);
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                int total = 0;
                Expression<Func<RMC_MemberMaterial, bool>> func = ExpressionExtensions.True<RMC_MemberMaterial>();
                func = f => f.MemberId == _KeyValue;
                #region 查询条件拼接
                //if (keywords != null && keywords != "&nbsp;")
                //{
                //    func = func.And(f => f.MemberNumbering.Contains(MemberNumbering));
                //}
                //if (!string.IsNullOrEmpty(MemberModel))
                //{
                //    func = func.And(f => f.MemberModel.Contains(MemberModel)); /*func = func.And(f => f.FullPath.Contains(model.FilePath))*/
                //}
                #endregion

                List<RMC_MemberMaterial> listfile = MemberMaterialCurrent.FindPage<string>(jqgridparam.page
                                         , jqgridparam.rows
                                         , func
                                         , false
                                         , f => f.MemberMaterialId.ToString()
                                         , out total
                                         ).ToList();
                List<MemberMaterialModel> EntityModelList = new List<MemberMaterialModel>();
                foreach (var item in listfile)
                {
                    MemberMaterialModel EntityModel = new MemberMaterialModel();
                    EntityModel.MemberMaterialId = item.MemberMaterialId;
                    EntityModel.MemberId = item.MemberId;
                    EntityModel.RawMaterialId = item.RawMaterialId;
                    EntityModel.MaterialNumber = item.MaterialNumber;
                    EntityModel.Description = item.Description;
                    var RawMaterial = RawMaterialCurrent.Find(f => f.RawMaterialId == item.RawMaterialId).SingleOrDefault();
                    var Unit = MemberUnitCurrent.Find(f => f.UnitId == RawMaterial.UnitId).SingleOrDefault();
                    EntityModel.RawMaterialName = RawMaterial.RawMaterialName;
                    EntityModel.RawMaterialStandard = RawMaterial.RawMaterialStandard;
                    EntityModel.UnitName = Unit.UnitName;
                    EntityModelList.Add(EntityModel);
                }
                var JsonData = new
                {
                    total = total / jqgridparam.rows + 1,
                    page = jqgridparam.page,
                    records = total,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = EntityModelList,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }

        }


        //表单
        public ActionResult RawMaterialsDosageForm()
        {
            return View();
        }

        public ActionResult SetDataForm(string KeyValue)
        {
            int _KeyValue = Convert.ToInt32(KeyValue);
            RMC_MemberMaterial entity = MemberMaterialCurrent.Find(f => f.MemberMaterialId == _KeyValue).SingleOrDefault();
            return Content(entity.ToJson());
        }
        /// <summary>
        /// 提交文件夹表单
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="KeyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        //[LoginAuthorize]
        public virtual ContentResult SubmitDataForm(RMC_MemberMaterial entity, string KeyValue, string TreeId)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int _KeyValue = Convert.ToInt32(KeyValue);
                    RMC_MemberMaterial Oldentity = MemberMaterialCurrent.Find(t => t.MemberMaterialId == _KeyValue).SingleOrDefault();//获取没更新之前实体对象
                    Oldentity.RawMaterialId = entity.RawMaterialId;
                    Oldentity.MaterialNumber = entity.MaterialNumber;
                    Oldentity.Description = entity.Description;
                    MemberMaterialCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                             //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    var MemberMaterialList = MemberMaterialCurrent.Find(f => f.MemberId == entity.MemberId).ToList();
                    var a = MemberMaterialList.Where(f => f.RawMaterialId == entity.RawMaterialId).SingleOrDefault();
                    if (a == null)
                    {
                        MemberMaterialCurrent.Add(entity);

                        var Member = MemberLibraryCurrent.Find(f => f.MemberID == entity.MemberId).SingleOrDefault();
                        Member.IsRawMaterial = 1;
                        MemberLibraryCurrent.Modified(Member);

                        IsOk = 1;

                    }
                    else
                    {
                        return Content(new JsonMessage { Success = false, Code = "-1", Message = "该数据已存在！" }.ToString());
                    }

                    //this.WSectionalSize_r = entity.SectionalSize_r;riteLog(IsOk, entity, null, KeyValue, Message);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                //this.WriteInertiaDistance_x1 = entity.InertiaDistance_x1;Log(-1, entity, null, KeyValue, "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        public virtual ActionResult DeleteMemberMaterial(string KeyValue, string MemberId)
        {

            int IsOk = -1;
            int key_value = Convert.ToInt32(KeyValue);
            List<int> ids = new List<int>();
            ids.Add(key_value);
            IsOk = MemberMaterialCurrent.Remove(ids);

            int _MemberId = Convert.ToInt32(MemberId);
            var MemberMaterial = MemberMaterialCurrent.Find(f => f.MemberId == _MemberId).ToList();
            if (MemberMaterial.Count() == 0)
            {
                var Member = MemberLibraryCurrent.Find(f => f.MemberID == _MemberId).SingleOrDefault();
                Member.IsRawMaterial = 0;
                MemberLibraryCurrent.Modified(Member);
            }
            return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "删除成功。" }.ToString());
        }

        /// <summary>
        /// 获取原材料
        /// </summary>
        /// <param name="MaterialClassId"></param>
        /// <returns></returns>
        public virtual ActionResult GetMaterialName(string MaterialClassId, string MemberId)
        {
           var Entitys = new List<RMC_RawMaterialLibrary>();
            if (MaterialClassId != "")
            {//筛选出还没有添加至该构件的材料
                List<string> MemberRawMaterial1 = new List<string>();
                List<string> MemberRawMaterial2 = new List<string>();
              
                int TreeId = Convert.ToInt32(MaterialClassId);
                int _MemberId = Convert.ToInt32(MemberId);
                var Entity = RawMaterialCurrent.Find(f => f.TreeId == TreeId).ToList();
                if (Entity.Count() > 0)
                {
                    foreach (var item in Entity)
                    {
                        MemberRawMaterial1.Add(item.RawMaterialId.ToString());
                    }
                }
                var Entity1 = MemberMaterialCurrent.Find(f => f.MemberId== _MemberId).ToList();
                if (Entity1.Count() > 0)
                {
                    foreach (var item1 in Entity1)
                    {
                        MemberRawMaterial2.Add(item1.RawMaterialId.ToString());
                    }
                }
                var MemberRawMaterial3 = MemberRawMaterial1.Where(c => !MemberRawMaterial2.Contains(c)).ToList();
                foreach (var item in MemberRawMaterial3)
                {
                    int _item = Convert.ToInt32(item);
                    var Model = Entity.Where(f => f.RawMaterialId == _item).SingleOrDefault();
                    Entitys.Add(Model);
                }
                //

            }


            return Json(Entitys);
        }
        #endregion

        #region 构件制程
        public ActionResult MemberProcessIndex()
        {
            return View();
        }
        public ActionResult GridListJsonMemberProcess(string KeyValue, JqGridParam jqgridparam)
        {
            int _KeyValue = Convert.ToInt32(KeyValue);
            try
            {
                int total = 0;
                Expression<Func<RMC_MemberProcess, bool>> func = ExpressionExtensions.True<RMC_MemberProcess>();
                func = f => f.MemberId == _KeyValue;
                #region 查询条件拼接
                //if (keywords != null && keywords != "&nbsp;")
                //{
                //    func = func.And(f => f.MemberNumbering.Contains(MemberNumbering));
                //}
                //if (!string.IsNullOrEmpty(MemberModel))
                //{
                //    func = func.And(f => f.MemberModel.Contains(MemberModel)); /*func = func.And(f => f.FullPath.Contains(model.FilePath))*/
                //}
                #endregion

                List<RMC_MemberProcess> listfile = MemberProcessCurrent.FindPage<string>(jqgridparam.page
                                         , jqgridparam.rows
                                         , func
                                         , false
                                         , f => f.SortCode.ToString()
                                         , out total
                                         ).ToList();
                var JsonData = new
                {
                    rows = listfile,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }

        }

        public readonly RepositoryFactory<Base_User> repositoryfactory = new RepositoryFactory<Base_User>();
        public ActionResult GetUserName()
        {

            List<Base_User> entity = repositoryfactory.Repository().FindListTop(25);
            //string JsonData = entity.ToJson();
            ////自定义
            //JsonData = JsonData.Insert(1, Sys_FormAttributeBll.Instance.GetBuildForm(KeyValue));
            return Json(entity);
        }
        //表单
        public ActionResult MemberProcessForm()
        {
            return View();
        }

        public ActionResult SetMemberProcessForm(string KeyValue)
        {
            int _KeyValue = Convert.ToInt32(KeyValue);
            RMC_MemberProcess entity = MemberProcessCurrent.Find(f => f.MemberProcessId == _KeyValue).SingleOrDefault();
            //string JsonData = entity.ToJson();
            ////自定义
            //JsonData = JsonData.Insert(1, Sys_FormAttributeBll.Instance.GetBuildForm(KeyValue));
            return Content(entity.ToJson());
        }
        /// <summary>
        /// 提交文件夹表单
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="KeyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        //[LoginAuthorize]
        public virtual ActionResult SubmitMemberProcessForm(RMC_MemberProcess entity, string KeyValue, string OrderId)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";

                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int _KeyValue = Convert.ToInt32(KeyValue);
                    RMC_MemberProcess Oldentity = MemberProcessCurrent.Find(t => t.MemberProcessId == _KeyValue).SingleOrDefault();//获取没更新之前实体对象
                    Oldentity.OperationTime = entity.OperationTime;
                    Oldentity.ProcessName = entity.ProcessName;
                    Oldentity.ProcessRequirements = entity.ProcessRequirements;
                    Oldentity.SortCode = entity.SortCode;
                    Oldentity.ProcessMan = entity.ProcessMan;
                    Oldentity.Description = entity.Description;
                    MemberProcessCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                             //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    int MemberProcessId = MemberProcessCurrent.Add(entity).MemberProcessId;
                    IsOk = 1;

                    int _OrderId = Convert.ToInt32(OrderId);
                    if (_OrderId != 0)
                    {
                        RMC_ProcessManagement Entity = new RMC_ProcessManagement();
                        Entity.OrderId = _OrderId;
                        Entity.MemberId = entity.MemberId;
                        Entity.MemberProcessId = MemberProcessId;
                        Entity.IsProcessStatus = 0;
                        ProcessManagementCurrent.Add(Entity);

                        var Member = MemberLibraryCurrent.Find(f => f.MemberID == entity.MemberId).SingleOrDefault();
                        Member.IsProcess = 1;
                        MemberLibraryCurrent.Modified(Member);
                    }

                    //this.WSectionalSize_r = entity.SectionalSize_r;riteLog(IsOk, entity, null, KeyValue, Message);
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                //this.WriteInertiaDistance_x1 = entity.InertiaDistance_x1;Log(-1, entity, null, KeyValue, "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        public virtual ActionResult DeleteMemberProcess(RMC_MemberLibrary entity, string KeyValue)
        {

            int IsOk = -1;
            int key_value = Convert.ToInt32(KeyValue);
            List<int> ids = new List<int>();
            ids.Add(key_value);
            IsOk = MemberProcessCurrent.Remove(ids);
            return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "删除成功。" }.ToString());
        }
        #endregion
    }
}








