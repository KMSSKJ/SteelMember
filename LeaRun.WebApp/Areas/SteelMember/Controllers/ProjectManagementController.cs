﻿using LeaRun.Business;
using LeaRun.Entity;
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
    public class ProjectManagementController : BaseController
    {
        //
        // GET: /Hello/
        public Base_ModuleBll Sys_modulebll = new Base_ModuleBll();
        public Base_ButtonBll Sys_buttonbll = new Base_ButtonBll();

        [Inject]
        public TreeIBLL TreeCurrent { get; set; }
        [Inject]
        public ProjectInfoIBLL ProjectInfoCurrent { get; set; }
        [Inject]
        public FileIBLL MemberLibraryCurrent { get; set; }
        [Inject]
        public ProjectManagementIBLL ProjectManagementCurrent { get; set; }
        [Inject]
        public OrderManagementIBLL OrderManagementCurrent { get; set; }
        [Inject]
        public RawMaterialIBLL RawMaterialCurrent { get; set; }
        [Inject]
        public ShipManagementIBLL ShipManagementCurrent { get; set; }
        [Inject]
        public ProjectWarehouseIBLL ProjectWarehouseCurrent { get; set; }

        [Inject]
        public MemberUnitIBLL MemberUnitCurrent { get; set; }

        [Inject]
        public OrderMemberIBLL OrderMemberCurrent { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QueryPage()
        {
            return View();
        }

        /// <summary>
        /// 【项目需求管理】返回树JONS
        /// </summary>
        /// <returns></returns>      
        public ActionResult TreeJson(string ItemId)
        {
            int itemid = Convert.ToInt32(ItemId);
            List<RMC_Tree> list, list1, list2;
            list1 = TreeCurrent.Find(t => t.DeleteFlag != 1 && t.ItemClass == 0).ToList();
            list2 = TreeCurrent.Find(t => t.ItemID == itemid && t.DeleteFlag != 1&&t.ItemClass==2).ToList();
            list = list1.Concat(list2).Distinct().ToList();

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
                tree.ismenu = item.IsMenu.ToString();
                tree.url = item.Url;
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

        #region 项目构件需求管理
        /// <summary>
        /// 【工程项目管理】返回文件（夹）列表JSON
        /// </summary>
        /// <param name="keywords">文件名搜索条件</param>
        /// <param name="FolderId">文件夹ID</param>
        /// <param name="IsPublic">是否公共 1-公共、0-我的</param>
        /// <returns></returns>         
        public ActionResult GridListJson(FileViewModel model, string TreeID, JqGridParam jqgridparam, string IsPublic, string ParameterJson)
        {

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
            try
            {
                int TreeId;
                //int FolderId = Convert.ToInt32(FolderId);
                if (TreeID == "" || TreeID == null)
                {
                    TreeId = 1;
                }
                else
                {
                    TreeId = Convert.ToInt32(TreeID);
                }

              
              


                int total = 0;
                Expression<Func<RMC_ProjectDemand, bool>> func = ExpressionExtensions.True<RMC_ProjectDemand>();
                func = f => f.DeleteFlag != 1 && f.TreeId == TreeId;
                #region 查询条件拼接
                if (TreeId.ToString() != "" && TreeId != 0)
                {
                    func = func.And(f => f.TreeId == TreeId);

                }
                if (model.OrderNumbering != null && model.OrderNumbering.ToString() != "")
                {
                    func = func.And(f => f.MemberModel.Contains(model.MemberModel));

                }
                if (model.InBeginTime != null && model.InBeginTime.ToString() != "0001/1/1 0:00:00")
                {
                    func = func.And(f => f.CreateTime >= model.InBeginTime);

                }
                if (model.InEndTime != null && model.InEndTime.ToString() != "0001/1/1 0:00:00")
                {
                    func = func.And(f => f.CreateTime <= model.InEndTime);
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
                List<RMC_ProjectDemand> listfile = ProjectManagementCurrent.FindPage<string>(jqgridparam.page
                                         , jqgridparam.rows
                                         , func
                                         , true
                                         , f => f.CreateTime.ToString()
                                         , out total
                                         ).OrderBy(f=>f.CreateTime).ToList();
                List<ProjectDemandModel> projectdemandlist = new List<ProjectDemandModel>();
                foreach (var item in listfile)
                {
                    ProjectDemandModel projectdemand = new ProjectDemandModel();
                    projectdemand.ProjectDemandId = item.ProjectDemandId;
                    var memberunit = MemberUnitCurrent.Find(f => f.UnitId == item.UnitId).SingleOrDefault();
                    var memberlibrary = MemberLibraryCurrent.Find(f => f.MemberID == item.MemberId).SingleOrDefault();
                    projectdemand.MemberName = memberlibrary.MemberName;
                    projectdemand.MemberModel = memberlibrary.MemberModel;
                    projectdemand.CreateTime = item.CreateTime;
                    projectdemand.MemberUnit = memberunit.UnitName;
                    projectdemand.UnitPrice = memberlibrary.UnitPrice;
                    projectdemand.MemberId = memberlibrary.MemberID;
                    projectdemand.MemberNumbering = memberlibrary.MemberNumbering.ToString();
                    projectdemand.IsReview = item.IsReview;
                    projectdemand.ReviewMan = item.ReviewMan;
                    projectdemand.MemberNumber = item.MemberNumber;
                    projectdemand.OrderQuantityed = item.OrderQuantityed;
                    projectdemand.Productioned = item.Productioned;
                    projectdemand.CollarNumbered = item.CollarNumbered;
                    //projectdemand.MemberWeight = item.MemberWeight;
                    //var company = CompanyCurrent.Find(f=>f.MemberCompanyId==item.MemberCompanyId).SingleOrDefault();
                    //projectdemand.MemberCompany = company.FullName;
                    projectdemand.Description = item.Description;
                    projectdemandlist.Add(projectdemand);
                }

                if (projectdemandlist.Count() > 0)// && listtree.Count() > 0
                {
                    //ListData0 = ListToDataTable(listtree);
                    ListData1 =DataHelper.ListToDataTable(projectdemandlist);
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
                    ListData =DataHelper.ListToDataTable(projectdemandlist);
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
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
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
        /// 【项目信息管理】返回文件夹对象JSON
        /// </summary>
        /// <param name="KeyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        //[LoginAuthorize]
        public ActionResult SetDataForm(string KeyValue)
        {
            //List<ProjectDemandModel> projectdemandlist =null;
            ProjectDemandModel projectdemand = new ProjectDemandModel();
            if (KeyValue != "")
            {
                int ProjectDemandId = Convert.ToInt32(KeyValue);
                var entity = ProjectManagementCurrent.Find(f => f.ProjectDemandId == ProjectDemandId).SingleOrDefault();
                var entity1 = MemberLibraryCurrent.Find(f => f.MemberID == entity.MemberId).SingleOrDefault();
                var entity_tree = TreeCurrent.Find(f => f.TreeID == entity.MemberClassId).SingleOrDefault();
                projectdemand.ProjectDemandId = ProjectDemandId;
                projectdemand.MemberClassId = entity.MemberClassId;
                projectdemand.MemberClassName = entity_tree.TreeName;
                projectdemand.MemberId = entity1.MemberID;
                projectdemand.UnitId = entity.UnitId;
                projectdemand.MemberModel = entity1.MemberModel;
                projectdemand.MemberNumber = entity.MemberNumber;
                projectdemand.Description = entity.Description;
                var filename = entity1.CAD_Drawing.Substring(0, entity1.CAD_Drawing.LastIndexOf('.'));//获取文件名称，去除后缀名
                string virtualPath = "../../Resource/Document/NetworkDisk/System/Member/" + filename + "/";
                var filename1 = entity1.Model_Drawing.Substring(0, entity1.Model_Drawing.LastIndexOf('.'));//获取文件名称，去除后缀名
                string virtualPath1 = "../../Resource/Document/NetworkDisk/System/Member/" + filename1 + "/";
                projectdemand.CADDrawing = virtualPath + entity1.CAD_Drawing;
                projectdemand.ModelDrawing = virtualPath1 + entity1.Model_Drawing;
            }


            return Content(projectdemand.ToJson());
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
        public virtual ActionResult SubmitDataForm(RMC_ProjectDemand entity, string KeyValue, string TreeID)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int keyvalue = Convert.ToInt32(KeyValue);
                    RMC_ProjectDemand Oldentity = ProjectManagementCurrent.Find(t => t.ProjectDemandId == keyvalue).SingleOrDefault();//获取没更新之前实体对象
                    Oldentity.MemberId = entity.MemberId;
                    Oldentity.MemberClassId = entity.MemberClassId;
                    Oldentity.MemberNumber = entity.MemberNumber;
                    Oldentity.MemberWeight = entity.MemberWeight;
                    Oldentity.MemberCompanyId = entity.MemberCompanyId;
                    Oldentity.Description = entity.Description;
                    ProjectManagementCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                    //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }     
                else
                {
                    int treeid = Convert.ToInt32(TreeID);
                    RMC_ProjectDemand Oldentity = new RMC_ProjectDemand();
                    Oldentity.TreeId = treeid;
                    Oldentity.MemberClassId = entity.MemberClassId;
                   // Oldentity.ProjectId = entity.ProjectId;//给旧实体重新赋值
                    Oldentity.MemberId = entity.MemberId;
                    var Member = MemberLibraryCurrent.Find(f => f.MemberID == entity.MemberId).SingleOrDefault();
                    Oldentity.MemberNumbering = Member.MemberNumbering;
                    Oldentity.MemberModel = Member.MemberModel;
                    Oldentity.UnitId = entity.UnitId;
                    Oldentity.IsSubmit = 0;
                    Oldentity.IsDemandSubmit= 0;
                    Oldentity.IsReview = 0;
                    Oldentity.OrderQuantityed = 0;
                    Oldentity.CreateTime=DateTime.Now;
                    Oldentity.MemberNumber = entity.MemberNumber;
                    Oldentity.MemberWeight = entity.MemberWeight;
                    Oldentity.MemberCompanyId = entity.MemberCompanyId;
                    Oldentity.Description = entity.Description;
                    ProjectManagementCurrent.Add(Oldentity);

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
        public ActionResult DeleteProjectDemand(string KeyValue)
        {
            try
            {
                List<int> ids = new List<int>();
                int ProjectDemandId = Convert.ToInt32(KeyValue);
                ids.Add(ProjectDemandId);
                ProjectManagementCurrent.Remove(ids);
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
        /// <summary>
        /// 获取分项目名称
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult GetTreeName(string KeyValue)
        {
            int TreeId = Convert.ToInt32(KeyValue);
            RMC_Tree entity = TreeCurrent.Find(f => f.TreeID == TreeId).SingleOrDefault();
            //string JsonData = entity.ToJson();
            ////自定义
            //JsonData = JsonData.Insert(1, Sys_FormAttributeBll.Instance.GetBuildForm(KeyValue));
            return Content(entity.ToJson());
            //return Json(entity);
        }

        /// <summary>
        /// 获取构件名称
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult GetMemderName(string KeyValue)
        {
            List<RMC_MemberLibrary> Entity = null;
            if (KeyValue != "")
            {
                int treeid = Convert.ToInt32(KeyValue);
                Entity = MemberLibraryCurrent.Find(f => f.TreeID == treeid).ToList();
            }
            return Json(Entity);
        }

        /// <summary>
        /// 获取构件单位
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMemderUnit()
        {
            List<RMC_MemberUnit> Entity = null;
            Entity = MemberUnitCurrent.Find(f => f.UnitId != 0).ToList();
            return Json(Entity);

        }

        /// <summary>
            /// 获取构件图纸模型
            /// </summary>
            /// <param name="KeyValue"></param>
            /// <returns></returns>
        public ActionResult GetMemderDrawing(string KeyValue)
        {
            ProjectDemandModel projectdemand = new ProjectDemandModel();
            if (KeyValue != "")
            {
                int memberid = Convert.ToInt32(KeyValue);
                var Entity = MemberLibraryCurrent.Find(f => f.MemberID == memberid).SingleOrDefault();
                var filename = Entity.CAD_Drawing.Substring(0, Entity.CAD_Drawing.LastIndexOf('.'));//获取文件名称，去除后缀名
                string virtualPath = "../../Resource/Document/NetworkDisk/System/Member/" + filename + "/";
                var filename1 = Entity.Model_Drawing.Substring(0, Entity.Model_Drawing.LastIndexOf('.'));//获取文件名称，去除后缀名
                string virtualPath1 = "../../Resource/Document/NetworkDisk/System/Member/" + filename1 + "/";

                projectdemand.CADDrawing = virtualPath + Entity.CAD_Drawing;
                projectdemand.ModelDrawing = virtualPath1 + Entity.Model_Drawing;
            }
            return Json(projectdemand);
        }
        #endregion

        #region 审核需求

        /// <summary>
        /// 审核需求
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult ReviewProjectDemand(string KeyValue,string IsReview)
        {
            try
            {
                string Message = IsReview == "2" ? "驳回成功。" : "审核成功。";
                int ProjectDemandId = Convert.ToInt32(KeyValue);
                var file = ProjectManagementCurrent.Find(f => f.ProjectDemandId == ProjectDemandId).First();
                file.ModifiedTime = DateTime.Now;
                file.ReviewMan = currentUser.RealName;
                file.IsReview =Convert.ToInt32(IsReview);
                ProjectManagementCurrent.Modified(file);
                return Content(new JsonMessage { Success = true, Code = "1", Message = Message }.ToString());
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

        /// <summary>
        /// 批量提交项目
        /// </summary>
        /// <param name="TreeId"></param>
        /// <returns></returns>
        public ActionResult SubmitProjectItemDemand(string KeyValue, RMC_ProjectDemand Entity)
        {
            var message = "";
            try
            {
                int TreeId = Convert.ToInt32(KeyValue);
                List<RMC_ProjectDemand> OldEntity = ProjectManagementCurrent.Find(f => f.TreeId == TreeId).ToList();
                if (OldEntity.Count() > 0)
                {
                    for (int i = 0; i < OldEntity.Count(); i++)
                    {

                        ProjectManagementCurrent.Find(f => f.ProjectDemandId == OldEntity[i].ProjectDemandId).SingleOrDefault();
                        Entity.ModifiedTime = DateTime.Now;
                        Entity.IsSubmit = 1;
                        ProjectManagementCurrent.Modified(Entity);
                        RMC_ProjectOrder Entity1 = new RMC_ProjectOrder();
                        Entity.ProjectDemandId = OldEntity[i].ProjectDemandId;
                        OrderManagementCurrent.Add(Entity1);

                    }
                    //foreach (var item in OldEntity)
                    //{
                    //    ProjectManagementCurrent.Find(f => f.ProjectDemandId == item.ProjectDemandId).SingleOrDefault();
                    //    Entity.ModifiedTime = DateTime.Now;
                    //    Entity.IsSubmit = 1;
                    //    ProjectManagementCurrent.Modified(Entity);
                    //    RMC_ProjectOrder Entity1 = new RMC_ProjectOrder();
                    //    Entity.ProjectDemandId = item.ProjectDemandId;
                    //    OrderManagementCurrent.Add(Entity1);
                    //}
                }
                else
                {
                    message = "该项目无项目信息";
                }
                return Content(new JsonMessage { Success = true, Code = "1", Message = message }.ToString());
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
