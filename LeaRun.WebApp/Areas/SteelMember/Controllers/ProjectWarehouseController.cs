
using LeaRun.Business;
using LeaRun.Entity.SteelMember;
using LeaRun.Repository.SteelMember.IBLL;
using LeaRun.Utilities;
using LeaRun.WebApp.Controllers;
using Ninject;
using SteelMember.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace LeaRun.WebApp.Areas.SteelMember.Controllers
{
    public class ProjectWarehouseController : BaseController
    {

        public Base_ModuleBll Sys_modulebll = new Base_ModuleBll();
        public Base_ButtonBll Sys_buttonbll = new Base_ButtonBll();
        [Inject]
        public ProjectInfoIBLL ProjectInfoCurrent { get; set; }
        [Inject]
        public ProjectManagementIBLL ProjectManagementCurrent { get; set; }
        [Inject]
        public TreeIBLL TreeCurrent { get; set; }
        [Inject]
        public FileIBLL MemberLibraryCurrent { get; set; }
        [Inject]
        public CompanyIBLL CompanyCurrent { get; set; }
        [Inject]
        public OrderManagementIBLL OrderManagementCurrent { get; set; }
        [Inject]
        public ProjectWarehouseIBLL ProjectWarehouseCurrent { get; set; }
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 【工程项目管理】返回树JONS
        /// </summary>
        /// <returns></returns>      
        public ActionResult TreeJson(string ItemId)
        {
            
            int _ItemId = Convert.ToInt32(ItemId);
            List<RMC_Tree> list, list1, list2, list3;
            list1 = TreeCurrent.Find(t => t.DeleteFlag != 1 && t.ItemClass == 0).ToList();
            list2 = TreeCurrent.Find(t => t.ItemID == _ItemId && t.DeleteFlag != 1 && t.ItemClass == 2).ToList();
            list3 = TreeCurrent.Find(t => t.ItemID == _ItemId && t.DeleteFlag != 1 && t.ItemClass == 5).ToList();
            list = list1.Concat(list2).Concat(list3).Distinct().ToList();

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

        #region 项目仓库管理
        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        public ActionResult QueryPage()
        {
            return View();
        }
        /// <summary>
        /// 【工程项目管理】返回文件（夹）列表JSON
        /// </summary>
        /// <param name="keywords">文件名搜索条件</param>
        /// <param name="FolderId">文件夹ID</param>
        /// <param name="IsPublic">是否公共 1-公共、0-我的</param>
        /// <returns></returns>         
        public ActionResult GridListJson(FileViewModel model, string ParameterJson, string TreeID, JqGridParam jqgridparam, string IsPublic)
        {
            if (ParameterJson != null)
            {
                if (ParameterJson != "[{\"MemberModel\":\"\",\"InBeginTime\":\"\",\"InEndTime\":\"\",\"Class\":\"\"}]")
                {
                    List<FileViewModel> query_member = JsonHelper.JonsToList<FileViewModel>(ParameterJson);
                    for (int i = 0; i < query_member.Count(); i++)
                    {
                        model.MemberModel = query_member[i].MemberModel;
                        model.InBeginTime = query_member[i].InBeginTime;
                        model.InEndTime = query_member[i].InEndTime;
                        model.Class = query_member[i].Class;
                    }
                }
            }
            try
            {
                model.TreeID = Convert.ToInt32(TreeID);
                int total = 0;
                Expression<Func<RMC_ProjectWarehouse, bool>> func = ExpressionExtensions.True<RMC_ProjectWarehouse>();
                func = f => f.DeleteFlag != 1&&f.IsShiped==1;
                #region 查询条件拼接
                if (model.TreeID != 0 && model.MemberModel != "")
                {
                    func = func.And(f => f.TreeId == model.TreeID);
                }
                if (model.MemberModel != null && model.MemberModel != "")
                {
                    var member = MemberLibraryCurrent.Find(fm => fm.MemberModel == model.MemberModel).SingleOrDefault();
                    func = func.And(f => f.MemberId== member.MemberID);
                }
                if (model.InBeginTime != null && model.InBeginTime.ToString() != "0001/1/1 0:00:00")
                {
                    func = func.And(f => f.ModifyTime >= model.InBeginTime);
                }
                if (model.InEndTime != null && model.InEndTime.ToString() != "0001/1/1 0:00:00")
                {
                    func = func.And(f => f.ModifyTime <= model.InEndTime);
                }
                if (!string.IsNullOrEmpty(model.Class))
                {
                    func = func.And(f => f.Class== model.Class.Trim()); /*func = func.And(f => f.FullPath.Contains(model.FilePath))*/
                }
                #endregion

                DataTable ListData, ListData1;
                ListData = null;
                //List<RMC_Tree> listtree = TreeCurrent.FindPage<string>(jqgridparam.page
                //                         , jqgridparam.rows
                //                         , func1
                //                         , true
                //                         , f => f.TreeID.ToString()
                //                         , out total
                //                         ).ToList();
                List<RMC_ProjectWarehouse> listfile = ProjectWarehouseCurrent.FindPage<string>(jqgridparam.page
                                         , jqgridparam.rows
                                         , func
                                         , true
                                         , f => f.ModifyTime.ToString()
                                         , out total
                                         ).ToList();
                List<ProjectWarehouseModel> ProjectWarehouselist = new List<ProjectWarehouseModel>();
                foreach (var item in listfile)
                {
                    ProjectWarehouseModel ProjectWarehouse = new ProjectWarehouseModel();
                    ProjectWarehouse.ProjectWarehouseId= item.ProjectWarehouseId;
                    //var projectinfo = ProjectInfoCurrent.Find(f => f.ProjectId == item.ProjectId).SingleOrDefault();
                    //projectdemand.ProjectName = projectinfo.ProjectName;
                    var memberlibrary = MemberLibraryCurrent.Find(f => f.MemberID == item.MemberId).SingleOrDefault();
                    ProjectWarehouse.MemberName = memberlibrary.MemberName;
                    ProjectWarehouse.MemberModel = memberlibrary.MemberModel;
                    ProjectWarehouse.MemberNumbering = memberlibrary.MemberNumbering.ToString();
                    ProjectWarehouse.Class= item.Class;
                    ProjectWarehouse.Damage = item.Damage;
                    ProjectWarehouse.InStock = item.InStock;
                    ProjectWarehouse.Leader = item.Leader;
                    ProjectWarehouse.Librarian = item.Librarian;
                    ProjectWarehouse.Description = item.Description;
                    ProjectWarehouselist.Add(ProjectWarehouse);
                }
                if (ProjectWarehouselist.Count() > 0)// && listtree.Count() > 0
                {
                 
                    //ListData0 = ListToDataTable(listtree);
                    ListData1 =DataHelper.ListToDataTable(ProjectWarehouselist);
                    ListData = ListData1.Clone();
                    object[] obj = new object[ListData.Columns.Count];
                    ////添加DataTable0的数据
                    //for (int i = 0; i < ListData0.Rows.Count; i++)
                    //{
                    //    ListData0.Rows[i].ItemArray.CopyTo(obj, 0);
                    //    ListData.Rows.Add(obj);
                    //}
                    //添加DataTable1的数据
                    for (int i = 0; i < ListData1.Rows.Count; i++)
                    {
                        ListData1.Rows[i].ItemArray.CopyTo(obj, 0);
                        ListData.Rows.Add(obj);
                    }
                  
                }
                //else if (listtree.Count() > 0)
                //{
                //    ListData = ListToDataTable(listtree);
                //}
                else if (listfile.Count() > 0)
                {
                    ListData =DataHelper.ListToDataTable(ProjectWarehouselist);
                }
                else
                {
                    ListData = null;
                }

                var JsonData = new
                {
                    rows = ListData,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                return Content("<script>alertDialog('" + ex.Message + "');</script>");
            }
        }

        /// <summary>
        /// 表单视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Form()
        {
            return View();
        }

        /// <summary>
        /// 【信息管理】返回文件夹对象JSON
        /// </summary>
        /// <param name="KeyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        //[LoginAuthorize]
        public ActionResult SetDataForm(string KeyValue)
        {
            int key_value = Convert.ToInt32(KeyValue);
            RMC_ProjectWarehouse entity = ProjectWarehouseCurrent.Find(f => f.ProjectWarehouseId == key_value).SingleOrDefault();
            //string JsonData = entity.ToJson();
            ////自定义
            //JsonData = JsonData.Insert(1, Sys_FormAttributeBll.Instance.GetBuildForm(KeyValue));
            return Content(entity.ToJson());
            //return Json(entity);
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
        public virtual ActionResult SubmitDataForm(RMC_ProjectWarehouse entity, string KeyValue, string TreeId)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProjectWarehouse Oldentity = ProjectWarehouseCurrent.Find(t => t.ProjectWarehouseId == key_value).SingleOrDefault();//获取没更新之前实体对象
                    Oldentity.Damage = entity.Damage;//给旧实体重新赋值
                    Oldentity.Class = entity.Class;
                    Oldentity.Librarian ="1";
                    Oldentity.Leader = entity.Leader;
                    Oldentity.Description = entity.Description;
                    ProjectWarehouseCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                    //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    int treeid = Convert.ToInt32(TreeId);
                    RMC_ProjectWarehouse Oldentity = new RMC_ProjectWarehouse();
                    Oldentity.TreeId = treeid;
                    Oldentity.Damage = entity.Damage;//给旧实体重新赋值
                    Oldentity.Class = entity.Class;
                    Oldentity.Librarian = "1";
                    Oldentity.ModifyTime = DateTime.Now;
                    Oldentity.Leader = entity.Leader;
                    Oldentity.Description = entity.Description;
                    ProjectWarehouseCurrent.Add(Oldentity);
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
        /// 删除文件
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public ActionResult DeleteData(string KeyValue)
        {
            try
            {
                List<int> ids = new List<int>();
                int Id = Convert.ToInt32(KeyValue);
                ids.Add(Id);
                ProjectWarehouseCurrent.Remove(ids);
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
        #endregion
    }
}
