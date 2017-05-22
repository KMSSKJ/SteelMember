using LeaRun.Business;
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
using System.Web;
using System.Web.Mvc;

namespace LeaRun.WebApp.Areas.SteelMember.Controllers
{
    public class CollarManagementController : BaseController
    {
        //
        // GET: /SteelMember/MemberSettlement/
        [Inject]
        public CollarIBLL CollarManagementCurrent { get; set; }
        [Inject]
        public CollarMemberIBLL CollarMemberCurrent { get; set; }
        [Inject]
        public MemberMaterialIBLL MemberMaterialCurrent { get; set; }
        [Inject]
        public ProjectManagementIBLL ProjectManagementCurrent { get; set; }
        [Inject]
        public ProjectWarehouseIBLL ProjectWarehouseCurrent { get; set; }
        [Inject]
        public RawMaterialIBLL RawMaterialCurrent { get; set; }
        [Inject]
        public FileIBLL MemberLibraryCurrent { get; set; }
        

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult QueryPage()
        {
            return View();
        }
        /// <summary>
        /// 创建领用单
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateCollarForm()
        {
            ViewBag.CollarNumbering = "LYD" + DateTime.Now.ToString("yyyyMMddhhmmssffff");
            ViewData["Librarian"] = currentUser.RealName;
            return View();
        }
        public ActionResult ItemList()
        {
            return View();
        }
        public ActionResult CollarForm()
        {
            return View();
        }

        public ActionResult DetailForm()
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
            int CollarId = Convert.ToInt32(KeyValue);
            RMC_Collar entity = CollarManagementCurrent.Find(f => f.CollarId == CollarId).SingleOrDefault();
            return Content(entity.ToJson());
            //return Json(entity);
        }


        [HttpPost]
        [ValidateInput(false)]
        //[LoginAuthorize]
        public virtual ActionResult SubmitCollarForm(RMC_Collar entity, string KeyValue, string TreeId, string POOrderEntryJson)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "变更成功。" : "新增成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int _CollarId = Convert.ToInt32(KeyValue);
                    RMC_Collar Oldentity = CollarManagementCurrent.Find(f => f.CollarId == _CollarId).SingleOrDefault();
                    Oldentity.Borrow = entity.Borrow;
                    Oldentity.CollarMan =entity.CollarMan;
                    Oldentity.CollarTime= entity.CollarTime;
                    Oldentity.Description = entity.Description;

                    List<int> Ids = new List<int>();
                    List<RMC_CollarMember> OrderMemberList = CollarMemberCurrent.Find(f => f.CollarId == _CollarId).ToList();
                    for (int i = 0; i < OrderMemberList.Count(); i++)
                    {
                        int CollarMemberId = OrderMemberList[i].CollarMemberId;
                        Ids.Add(CollarMemberId);
                        int ProjectDemandId = Convert.ToInt32(OrderMemberList[i].ProjectDemandId);
                        var CollarMember = CollarMemberCurrent.Find(f => f.CollarMemberId == CollarMemberId).SingleOrDefault();
                        var ProjectDemand = ProjectManagementCurrent.Find(f => f.ProjectDemandId == ProjectDemandId).SingleOrDefault();
                        ProjectDemand.CollarNumbered = ProjectDemand.CollarNumbered - CollarMember.Qty;
                        ProjectManagementCurrent.Modified(ProjectDemand);
                    }
                    CollarMemberCurrent.Remove(Ids);

                    //构件单
                    List<CollarModel> POOrderEntryList = POOrderEntryJson.JonsToList<CollarModel>();
                    //int index = 1;
                    foreach (CollarModel poorderentry in POOrderEntryList)
                    {
                        if (!string.IsNullOrEmpty(poorderentry.MemberNumbering))
                        {
                            RMC_CollarMember CollarModel = new RMC_CollarMember();
                            CollarModel.CollarId = _CollarId;
                            CollarModel.ProjectDemandId = Convert.ToInt32(poorderentry.ProjectDemandId);
                            CollarModel.MemberId = Convert.ToInt32(poorderentry.MemberID);
                            CollarModel.Description = poorderentry.Description;
                            CollarModel.MemberNumbering = poorderentry.MemberNumbering;
                            CollarModel.MemberModel = poorderentry.MemberModel;
                            CollarModel.MemberName = poorderentry.MemberName;
                            CollarModel.MemberUnit = poorderentry.MemberUnit;
                            //CollarModel.Price = Convert.ToDecimal(poorderentry.Price);
                            //CollarModel.PriceAmount = Convert.ToDecimal(poorderentry.PriceAmount);
                            CollarModel.Qty = Convert.ToInt32(poorderentry.Qty);


                            var ProjectDemand = ProjectManagementCurrent.Find(f => f.ProjectDemandId == CollarModel.ProjectDemandId).SingleOrDefault();
                            ProjectDemand.OrderQuantityed = ProjectDemand.CollarNumbered+ Convert.ToInt32(poorderentry.Qty);
                            ProjectManagementCurrent.Modified(ProjectDemand);

                            CollarMemberCurrent.Add(CollarModel);
                            //index++;
                        }
                    }

                }
                else
                {
                    int _TreeId = Convert.ToInt32(TreeId);
                    RMC_Collar Oldentity = new RMC_Collar();
                    Oldentity.TreeId = _TreeId;
                    Oldentity.Borrow = entity.Borrow;
                    Oldentity.CollarNumbering = entity.CollarNumbering;
                    Oldentity.Use = entity.Use;
                    Oldentity.Librarian = entity.Librarian;
                    Oldentity.CollarMan = entity.CollarMan;
                    Oldentity.CollarTime = entity.CollarTime;
                    Oldentity.Description = entity.Description;
                    int CollarId = CollarManagementCurrent.Add(Oldentity).CollarId;

                    RMC_CollarMember CollarModel = new RMC_CollarMember();
                    List<CollarModel> POOrderEntryList = POOrderEntryJson.JonsToList<CollarModel>();
                    int index = 1;
                    foreach (CollarModel poorderentry in POOrderEntryList)
                    {
                        if (!string.IsNullOrEmpty(poorderentry.MemberNumbering))
                        {
                            //poorderentry.SortCode = index;
                            //poorderentry.Create();
                            CollarModel.CollarId = CollarId;
                            CollarModel.ProjectDemandId = Convert.ToInt32(poorderentry.ProjectDemandId);
                            CollarModel.MemberId = Convert.ToInt32(poorderentry.MemberID);
                            CollarModel.Description = poorderentry.Description;
                            CollarModel.MemberNumbering = poorderentry.MemberNumbering;
                            CollarModel.MemberModel = poorderentry.MemberModel;
                            CollarModel.MemberName = poorderentry.MemberName;
                            CollarModel.MemberUnit = poorderentry.MemberUnit;
                            //CollarModel.Price = Convert.ToDecimal(poorderentry.Price);
                            //CollarModel.PriceAmount = Convert.ToDecimal(poorderentry.PriceAmount);
                            CollarModel.Qty = Convert.ToInt32(poorderentry.Qty);

                            var Demand = ProjectManagementCurrent.Find(f => f.ProjectDemandId == CollarModel.ProjectDemandId).SingleOrDefault();
                            Demand.OrderQuantityed = Demand.CollarNumbered + CollarModel.Qty;
                            ProjectManagementCurrent.Modified(Demand);

                            CollarMemberCurrent.Add(CollarModel);
                            index++;
                        }
                    }
                }
                IsOk = 1;
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                //this.WriteLog(-1, entity, null, KeyValue, "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

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
                if (ParameterJson != "[{\"CollarNumbering\":\"\",\"InBeginTime\":\"\",\"InEndTime\":\"\"}]")
                {
                    List<FileViewModel> query_member = JsonHelper.JonsToList<FileViewModel>(ParameterJson);
                    for (int i = 0; i < query_member.Count(); i++)
                    {
                        model.CollarNumbering = query_member[i].CollarNumbering;
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
                Expression<Func<RMC_Collar, bool>> func = ExpressionExtensions.True<RMC_Collar>();
                #region 查询条件拼接

                if (TreeId.ToString() != "" && TreeId != 0)
                {
                    func = func.And(f => f.TreeId == TreeId);

                }
                if (model.CollarNumbering != null && model.CollarNumbering.ToString() != "")
                {
                    func = func.And(f => f.CollarNumbering.Contains(model.CollarNumbering));

                }
                if (model.InBeginTime != null && model.InBeginTime.ToString() != "0001/1/1 0:00:00")
                {
                    func = func.And(f => f.CollarTime >= model.InBeginTime);

                }
                if (model.InEndTime != null && model.InEndTime.ToString() != "0001/1/1 0:00:00")
                {
                    func = func.And(f => f.CollarTime <= model.InEndTime);
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
                List<RMC_Collar> listfile = CollarManagementCurrent.FindPage<string>(jqgridparam.page
                                         , jqgridparam.rows
                                         , func
                                         , true
                                         , f => f.TreeId.ToString()
                                         , out total
                                         ).ToList();
                List<ProjectDemandModel> projectdemandlist = new List<ProjectDemandModel>();
                foreach (var item in listfile)
                {
                    ProjectDemandModel projectdemand = new ProjectDemandModel();
                    projectdemand.CollarId = item.CollarId;
                    projectdemand.CollarNumbering = item.CollarNumbering;
                    projectdemand.Use = item.Use;
                    projectdemand.CollarTime = item.CollarTime;
                    projectdemand.CollarMan = item.CollarMan;
                    projectdemand.Librarian = item.Librarian;
                    projectdemand.Description = item.Description;
                    projectdemandlist.Add(projectdemand);
                }
                if (projectdemandlist.Count() > 0)// && listtree.Count() > 0
                {

                    //ListData0 = ListToDataTable(listtree);
                    ListData1 = DataHelper.ListToDataTable(projectdemandlist);
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
                    ListData = DataHelper.ListToDataTable(listfile);
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
        /// 订单明细列表（返回Json）
        /// </summary>
        /// <param name="POOrderId">订单主键</param>
        /// <returns></returns>
        public ActionResult GetCollarEntryList(string CollarId)
        {
            int _CollarId = Convert.ToInt32(CollarId);
            try
            {
                List<OrderModel> OrderModellist = new List<OrderModel>();
                var listfile = CollarMemberCurrent.Find(f => f.CollarId == _CollarId).ToList();
                foreach (var item in listfile)
                {
                    OrderModel OrderModel = new OrderModel();
                    OrderModel.MemberNumbering = item.MemberNumbering;
                    OrderModel.MemberName = item.MemberName;
                    OrderModel.MemberModel = item.MemberModel;
                    OrderModel.MemberUnit = item.MemberUnit;
                    OrderModel.Qty = item.Qty.ToString();
                    //OrderModel.Price = item.Price.ToString();
                    //OrderModel.PriceAmount = item.PriceAmount.ToString();
                    OrderModel.Description = item.Description;
                    OrderModellist.Add(OrderModel);
                }



                var JsonData = new
                {
                    rows = OrderModellist,
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }

        public ActionResult GridListJsonCollar (FileViewModel model, string ParameterJson, string TreeID, JqGridParam jqgridparam, string IsPublic)
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
                func = f => f.DeleteFlag != 1 && f.IsShiped == 1&&f.InStock>0;
                #region 查询条件拼接
                if (model.TreeID != 0 && model.TreeID.ToString() != "")
                {
                    func = func.And(f => f.TreeId == model.TreeID);
                }
                if (model.MemberModel != null && model.MemberModel != "")
                {
                    var member = MemberLibraryCurrent.Find(fm => fm.MemberModel == model.MemberModel).SingleOrDefault();
                    func = func.And(f => f.MemberId == member.MemberID);
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
                    func = func.And(f => f.Class == model.Class.Trim()); /*func = func.And(f => f.FullPath.Contains(model.FilePath))*/
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
                    ProjectWarehouse.ProjectWarehouseId = item.ProjectWarehouseId;
                    ProjectWarehouse.ProjectDemandId = item.ProjectDemandId;
                    ProjectWarehouse.MemberId = item.MemberId;
                    var memberlibrary = MemberLibraryCurrent.Find(f => f.MemberID == item.MemberId).SingleOrDefault();
                    ProjectWarehouse.MemberName = memberlibrary.MemberName;
                    ProjectWarehouse.MemberModel = memberlibrary.MemberModel;
                    ProjectWarehouse.MemberNumbering = memberlibrary.MemberNumbering.ToString();
                    ProjectWarehouse.MemberUnit = memberlibrary.MemberUnit;
                    ProjectWarehouse.InStock = item.InStock;
                    var ProjectDomend = ProjectManagementCurrent.Find(f=>f.ProjectDemandId==item.ProjectDemandId).SingleOrDefault();
                    ProjectWarehouse.CollarNumbered = ProjectDomend.CollarNumbered;
                    ProjectWarehouse.Description = item.Description;
                    ProjectWarehouselist.Add(ProjectWarehouse);
                }
                if (ProjectWarehouselist.Count() > 0)// && listtree.Count() > 0
                {

                    //ListData0 = ListToDataTable(listtree);
                    ListData1 = DataHelper.ListToDataTable(ProjectWarehouselist);
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
                    ListData = DataHelper.ListToDataTable(ProjectWarehouselist);
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
        public ActionResult DeleteProjectCollar(string KeyValue)
        {
            try
            {
                List<int> ids = new List<int>();
                List<int> ids1 = new List<int>();
                int CollarId = Convert.ToInt32(KeyValue);
                ids.Add(CollarId);
                CollarManagementCurrent.Remove(ids);


                List<RMC_CollarMember> OrderMemberList = CollarMemberCurrent.Find(f => f.CollarId== CollarId).ToList();
                if (OrderMemberList.Count() > 0)
                {
                    for (int i = 0; i < OrderMemberList.Count(); i++)
                    {
                        ids1.Add(Convert.ToInt32(OrderMemberList[i].CollarMemberId));

                        var OrderMember = CollarMemberCurrent.Find(f => f.CollarMemberId == OrderMemberList[i].CollarMemberId).SingleOrDefault();
                        var Demand = ProjectManagementCurrent.Find(f => f.ProjectDemandId == OrderMember.ProjectDemandId).SingleOrDefault();
                        Demand.OrderQuantityed = Demand.OrderQuantityed - OrderMemberList[i].Qty;
                        ProjectManagementCurrent.Modified(Demand);
                    }
                    CollarMemberCurrent.Remove(ids1);
                }
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


    }
}
