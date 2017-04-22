
using LeaRun.Business;
using LeaRun.Entity.SteelMember;
using LeaRun.Repository.SteelMember.IBLL;
using LeaRun.Utilities;
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
    public class ProjectOrderController : Controller
    {
        //
        // GET: /Hello/
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
        [Inject]
        public ShipManagementIBLL ShipManagementCurrent { get; set; }
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
            int itemid = Convert.ToInt32(ItemId);
            List<RMC_Tree> list = TreeCurrent.Find(t => t.ItemID == itemid && t.DeleteFlag != 1).ToList();
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

        #region 构件订单管理
        /// <summary>
        /// 【工程项目管理】返回文件（夹）列表JSON
        /// </summary>
        /// <param name="keywords">文件名搜索条件</param>
        /// <param name="FolderId">文件夹ID</param>
        /// <param name="IsPublic">是否公共 1-公共、0-我的</param>
        /// <returns></returns>         
        public ActionResult GridListJson(/*ProjectInfoViewModel model,*/ string TreeID, JqGridParam jqgridparam, string IsPublic)
        {
            try
            {
                int TreeId;
                //int FolderId = Convert.ToInt32(FolderId);
                if (TreeID == ""|| TreeID == null)
                {
                    TreeId = 1;
                }
                else
                {
                    TreeId = Convert.ToInt32(TreeID);
                }

                int total = 0;
                Expression<Func<RMC_ProjectOrder, bool>> func = ExpressionExtensions.True<RMC_ProjectOrder>();
                func = f => f.DeleteFlag != 1 && f.TreeId == TreeId;
                #region 查询条件拼接
                //if (model.ProjectName != null && model.ProjectName != "&nbsp;")
                //{
                //    func = func.And(f => f.ProjectName.Contains(model.ProjectName));
                //}
                //if (!string.IsNullOrEmpty(model.ProjectAddress))
                //{
                //    func = func.And(f => f.ProjectAddress == model.ProjectAddress); /*func = func.And(f => f.FullPath.Contains(model.FilePath))*/
                //}
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
                List<RMC_ProjectOrder> listfile = OrderManagementCurrent.FindPage<string>(jqgridparam.page
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
                    projectdemand.OrderId = item.OrderId;
                    var order = OrderManagementCurrent.Find(f => f.OrderId == item.OrderId).SingleOrDefault();
                    var demand = ProjectManagementCurrent.Find(f => f.ProjectDemandId == order.ProjectDemandId).SingleOrDefault();
                    var memberlibrary = MemberLibraryCurrent.Find(f => f.MemberID == demand.MemberId).SingleOrDefault();
                    projectdemand.MemberId = memberlibrary.MemberID;
                    projectdemand.OrderNumbering = item.OrderNumbering;
                    projectdemand.MemberName = memberlibrary.MemberName;
                    projectdemand.MemberModel = memberlibrary.MemberModel;
                    projectdemand.UnitPrice = memberlibrary.UnitPrice;
                    projectdemand.MemberNumbering = memberlibrary.MemberNumbering.ToString();
                    projectdemand.OrderNumber = item.OrderNumber;

                    string a = memberlibrary.UnitPrice;
                    char[] b = a.ToArray();
                    string result = "";
                    for (int i = 0; i < b.Length; i++)
                    {
                        if (("0123456789.").IndexOf(b[i] + "") != -1)
                        {
                            result += b[i];
                        }
                    }
                    decimal d = Convert.ToDecimal(result);

                    string membernumber = item.OrderNumber.ToString();
                    char[] c = membernumber.ToArray();
                    string MemberNumber = "";
                    for (int i = 0; i < c.Length; i++)
                    {
                        if (("0123456789.").IndexOf(c[i] + "") != -1)
                        {
                            MemberNumber += c[i];
                        }
                    }
                    decimal Number = Convert.ToDecimal(MemberNumber);
                    projectdemand.CostBudget = (d * Number).ToString() + "元";
                    //var company = CompanyCurrent.Find(f => f.MemberCompanyId == item.MemberCompanyId).SingleOrDefault();
                    //projectdemand.MemberCompany = company.FullName;
                    projectdemand.IsReview = item.IsReview;
                    projectdemand.ReviewMan = "System";
                    projectdemand.IsSubmit = item.IsSubmit;
                    projectdemand.CreateTime = item.CreateTime;
                    projectdemand.CreateMan = "System";
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
                    ListData =DataHelper.ListToDataTable(listfile);
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
        /// 获取项目名称
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectName()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List<RMC_ProjectInfo> ProjectList =ProjectInfoCurrent.Find(f => f.ProjectId > 0).ToList();
            foreach (var Item in ProjectList)
            {
                SelectListItem item = new SelectListItem();
                item.Text = Item.ProjectName;
                item.Value = Item.ProjectId.ToString();
                List.Add(item);
            }
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取构件名称
        /// </summary>
        /// <returns></returns>
        public ActionResult MemberName(string TreeId)
        {
            int _TreeId = Convert.ToInt32(TreeId);
            List<SelectListItem> List = new List<SelectListItem>();
            List<RMC_MemberLibrary> ProjectList = MemberLibraryCurrent.Find(f => f.TreeID==_TreeId).ToList();
            foreach (var Item in ProjectList)
            {
                SelectListItem item = new SelectListItem();
                item.Text = Item.MemberModel;
                item.Value = Item.MemberID.ToString();
                List.Add(item);
            }
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取构件厂名称
        /// </summary>
        /// <returns></returns>
        public ActionResult MemberCompanyName()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List<RMC_Company> MemberCompanyList = CompanyCurrent.Find(f => f.MemberCompanyId > 0).ToList();
            foreach (var Item in MemberCompanyList)
            {
                SelectListItem item = new SelectListItem();
                item.Text = Item.FullName;
                item.Value = Item.MemberCompanyId.ToString();
                List.Add(item);
            }
            return Json(List, JsonRequestBehavior.AllowGet);
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
            int ProjectDemandId = Convert.ToInt32(KeyValue);
            RMC_ProjectDemand entity = ProjectManagementCurrent.Find(f => f.ProjectDemandId == ProjectDemandId).SingleOrDefault();
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
        public virtual ActionResult SubmitDataForm(RMC_ProjectDemand entity, string KeyValue, string TreeId)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int keyvalue = Convert.ToInt32(KeyValue);
                    RMC_ProjectDemand Oldentity = ProjectManagementCurrent.Find(t => t.ProjectDemandId == keyvalue).SingleOrDefault();//获取没更新之前实体对象
                    Oldentity.ProjectId = entity.ProjectId;//给旧实体重新赋值
                    Oldentity.MemberId = entity.MemberId;
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
                    int treeid = Convert.ToInt32(TreeId);
                    RMC_ProjectDemand Oldentity = new RMC_ProjectDemand();
                    Oldentity.TreeId = treeid;
                    Oldentity.ProjectId = entity.ProjectId;//给旧实体重新赋值
                    Oldentity.MemberId = entity.MemberId;
                    Oldentity.IsSubmit = 0;
                    Oldentity.IsReview = 0;
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
        /// 审核需求
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult ReviewProjectOrder(string KeyValue, string IsReview)
        {
            try
            {
                string Message = IsReview == "2" ? "驳回成功。" : "审核成功。";
                int OrderId = Convert.ToInt32(KeyValue);
                var file = OrderManagementCurrent.Find(f => f.OrderId == OrderId).First();
                file.ModifiedTime = DateTime.Now;
                file.IsReview = Convert.ToInt32(IsReview);
                OrderManagementCurrent.Modified(file);
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
        /// 提交项目订单
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult SubmitProjectOrder(string KeyValue,string MemberId,RMC_ProjectOrder Entity)
        {
            try
            {
                int OrderId = Convert.ToInt32(KeyValue);
                int _MemberId = Convert.ToInt32(MemberId);
                var file = OrderManagementCurrent.Find(f => f.OrderId == OrderId).First();
                file.ModifiedTime = DateTime.Now;
                file.IsSubmit = 1;
                file.SubmitTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                file.SubmitMan = "System";
                OrderManagementCurrent.Modified(file);

                List<RMC_ProjectWarehouse> projectwarehouselist = ProjectWarehouseCurrent.Find(f => f.MemberId == _MemberId).ToList();
                if (projectwarehouselist.Count() == 0)
                {
                    RMC_ProjectWarehouse projectwarehouse = new RMC_ProjectWarehouse();
                    projectwarehouse.OrderId = OrderId;
                    projectwarehouse.MemberId = _MemberId;
                    projectwarehouse.TreeId = file.TreeId;
                    projectwarehouse.IsShiped = 0;
                    ProjectWarehouseCurrent.Add(projectwarehouse);
                }

                List<RMC_ShipManagement> shipmanagementlist = ShipManagementCurrent.Find(f => f.MemberId == _MemberId).ToList();
                if (shipmanagementlist.Count() == 0)
                {
                    RMC_ShipManagement shipmanagement = new RMC_ShipManagement();
                    shipmanagement.OrderId = OrderId;
                    shipmanagement.MemberId = _MemberId;
                    shipmanagement.TreeId = file.TreeId;
                    shipmanagement.IsPackaged = 0;
                    ShipManagementCurrent.Add(shipmanagement);
                }

                return Content(new JsonMessage { Success = true, Code = "1", Message = "提交成功。" }.ToString());
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
        public ActionResult SubmitProjectItemOrder(string KeyValue, RMC_ProjectDemand Entity)
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
                        //RMC_ProjectOrder Entity1 = new RMC_ProjectOrder();
                        //Entity.ProjectDemandId = OldEntity[i].ProjectDemandId;
                        //OrderManagementCurrent.Add(Entity1);

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
