using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;
using LeaRun.Business;
using LeaRun.Repository.SteelMember.IBLL;
using LeaRun.Entity.SteelMember;
using LeaRun.Utilities;

namespace LeaRun.WebApp.Areas.SteelMember.Controllers
{
    public class TreeController: Controller
    {
        // GET: DocManagement/Tree
        public Base_ModuleBll Sys_modulebll = new Base_ModuleBll();
        public Base_ButtonBll Sys_buttonbll = new Base_ButtonBll();
        [Inject]
        public TreeIBLL TreeCurrent { get; set; }
        [Inject]
        public ProjectInfoIBLL ProjectInfoCurrent { get; set; }
        /// <summary>
        /// 文件夹表单视图
        /// </summary>
        /// <returns></returns>
        //[ManagerPermission(PermissionMode.Enforce)]

        public virtual ActionResult IsItem(string TreeId) {
            int treeid = Convert.ToInt32(TreeId);
            var entitytree = TreeCurrent.Find(f => f.TreeID == treeid).SingleOrDefault();
            return Json(entitytree);
            }
        public virtual ActionResult FolderForm()
        {
            return View();
        }

        /// <summary>
        /// 【控制测量文档管理】返回文件夹对象JSON
        /// </summary>
        /// <param name="KeyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        //[LoginAuthorize]
        public virtual ActionResult SetFolderForm(string KeyValue)
        {
            int folderid = Convert.ToInt32(KeyValue);
            RMC_Tree entity = TreeCurrent.Find(f => f.TreeID == folderid).SingleOrDefault();
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
        public virtual ActionResult SubmitFolderForm(RMC_Tree entity, string KeyValue, string TreeId ,string ItemID,string IsItem)
        {
            try
            {
                int IsOk = 0;
                string message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int keyvalue = Convert.ToInt32(KeyValue);
                    RMC_Tree Oldentity = TreeCurrent.Find(t => t.TreeID == keyvalue).SingleOrDefault();//获取没更新之前实体对象
                    Oldentity.TreeName = entity.TreeName;//给旧实体重新赋值
                    Oldentity.ModifiedTime = DateTime.Now;
                    Oldentity.OverdueTime = entity.OverdueTime;
                    Oldentity.IsMenu = entity.IsMenu;
                    Oldentity.Enabled = entity.Enabled;
                    Oldentity.Description = entity.Description;
                    TreeCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                    //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                        int treeid = Convert.ToInt32(TreeId);
                        RMC_Tree entitys = new RMC_Tree();
                        entitys.ParentID = treeid;
                        entitys.ItemID = Convert.ToInt32(ItemID);
                        entitys.IsItem = Convert.ToInt32(IsItem);
                        entitys.TreeName = entity.TreeName;
                        entitys.UploadTime = DateTime.Now;
                        entitys.ModifiedTime = DateTime.Now;
                        entitys.IsMenu = entity.IsMenu;
                        entitys.OverdueTime = entity.OverdueTime;
                        entitys.Icon = "-1";
                        entitys.Enabled = 1;
                        entitys.IsReview = 0;
                        TreeCurrent.Add(entitys);
                        IsOk = 1;
                    
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = message }.ToString());
            }
            catch (Exception ex)
            {
                //this.WriteLog(-1, entity, null, KeyValue, "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
       
        /// <summary>
        /// 销毁文件树
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult DeleteTree(string KeyValue)
        {
            int Ok = 0;
            try
            {
                List<int> ids = new List<int>();
                int TreeId = Convert.ToInt32(KeyValue);
                ids.Add(TreeId);
                Ok = TreeCurrent.Remove(ids);           
                return Content(new JsonMessage { Success = true, Code = Ok.ToString(), Message = "删除成功。" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage {Success = false,Code = "-1", Message = "操作失败：" + ex.Message}.ToString());
            }
        }
       
        /// <summary>
        /// 标记删除文件树
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult MarkTree(string KeyValue)
        {
            try
            {
                int TreeId = Convert.ToInt32(KeyValue);
                var file = TreeCurrent.Find(f => f.TreeID == TreeId).First();
                file.ModifiedTime = DateTime.Now;
                file.DeleteFlag = 1;
                TreeCurrent.Modified(file);
                return Content(new JsonMessage { Success = true, Code = "1", Message = "删除成功。" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

    }
}