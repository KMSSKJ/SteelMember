using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using ThoughtWorks.QRCode.Codec;
using System.Drawing;
using ThoughtWorks.QRCode.Codec.Data;
using System.IO;
using Ninject;
using LeaRun.Business;
using LeaRun.Repository.SteelMember.IBLL;
using LeaRun.Entity.SteelMember;
using LeaRun.Utilities;
using SteelMember.Models;

namespace LeaRun.WebApp.Areas.SteelMember.Controllers
{
    public class MemberProduceController : Controller
    {
        // GET: DocManagement/Tree
        public Base_ModuleBll Sys_modulebll = new Base_ModuleBll();
        public Base_ButtonBll Sys_buttonbll = new Base_ButtonBll();

        [Inject]
        public TreeIBLL TreeCurrent { get; set; }
        [Inject]
        public FileIBLL MemberLibraryCurrent { get; set; }
        [Inject]
        public ProjectInfoIBLL ProjectInfoCurrent { get; set; }
        [Inject]
        public ProjectManagementIBLL ProjectManagementCurrent { get; set; }
        [Inject]
        public OrderManagementIBLL OrderManagementCurrent { get; set; }
        [Inject]
        public RawMaterialIBLL RawMaterialCurrent { get; set; }
        [Inject]
        public ProcessManagementIBLL ProcessManagementCurrent { get; set; }
        [Inject]
        public ShipManagementIBLL ShipManagementCurrent { get; set; }
        [Inject]
        public ProjectWarehouseIBLL ProjectWarehouseCurrent { get; set; }
        public virtual ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 【项目管理】返回树JONS
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
                tree.icon = item.Icon;
                tree.ismenu = item.IsMenu.ToString();
                tree.url = item.Url;
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

        #region 构件厂订单管理
        public virtual ActionResult OrderIndex()
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
        public ActionResult GridListJsonOrder(/*ProjectInfoViewModel model,*/ string TreeID, JqGridParam jqgridparam, string IsPublic)
        {
            try
            {
                int TreeId;
                //int FolderId = Convert.ToInt32(FolderId);
                if (TreeID == "" || TreeID == null)
                {
                    TreeId = 22;
                }
                else
                {
                    TreeId = Convert.ToInt32(TreeID);
                }

                int total = 0;
                Expression<Func<RMC_ProjectOrder, bool>> func = ExpressionExtensions.True<RMC_ProjectOrder>();
                func = f => f.DeleteFlag != 1 && f.TreeId == TreeId&&f.IsSubmit==1;
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
                                         , f => f.ModifiedTime.ToString()
                                         , out total
                                         ).ToList();
                List<ProduceOrderModel> produceorderlist = new List<ProduceOrderModel>();
                foreach (var item in listfile)
                {
                    ProduceOrderModel produceorder = new ProduceOrderModel();
                    produceorder.OrderId = item.OrderId;
                    var order = OrderManagementCurrent.Find(f => f.OrderId == item.OrderId).SingleOrDefault();
                    var demand = ProjectManagementCurrent.Find(f => f.ProjectDemandId == order.ProjectDemandId).SingleOrDefault();
                    var member = MemberLibraryCurrent.Find(f => f.MemberID == demand.MemberId).SingleOrDefault();
                    produceorder.MemberId = member.MemberID;
                    produceorder.MemberNumbering =member.MemberNumbering.ToString();
                    produceorder.OrderNumbering = order.OrderNumbering;
                    produceorder.MemberName = member.MemberName;
                    produceorder.MemberModel = member.MemberModel;
                    produceorder.IsSubmit = item.IsSubmit;
                    produceorder.SubmitMan = item.SubmitMan;
                    produceorder.SubmitTime = item.SubmitTime.ToString();
                    produceorder.ConfirmOrder = item.ConfirmOrder;
                    produceorder.ConfirmMan = item.ConfirmMan;
                    produceorder.Description = item.Description;
                    produceorderlist.Add(produceorder);
                }
                if (produceorderlist.Count() > 0)// && listtree.Count() > 0
                {

                    //ListData0 = ListToDataTable(listtree);
                    ListData1 = DataHelper.ListToDataTable(produceorderlist);
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
                    ListData = DataHelper.ListToDataTable(produceorderlist);
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
        public ActionResult OrderForm()
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
            int key_value = Convert.ToInt32(KeyValue);
            RMC_ProjectOrder entity = OrderManagementCurrent.Find(f => f.OrderId == key_value).SingleOrDefault();
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
        public virtual ActionResult SubmitDataForm(RMC_ProjectOrder entity, string KeyValue, string OrderId)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProjectOrder Oldentity = OrderManagementCurrent.Find(t => t.OrderId == key_value).SingleOrDefault();//获取没更新之前实体对象
                    Oldentity.RawMaterialConsumption = entity.RawMaterialConsumption;
                    Oldentity.OrderBudget = entity.OrderBudget;
                    Oldentity.Description = entity.Description;
                    OrderManagementCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                    //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    int orderid = Convert.ToInt32(OrderId);
                    RMC_ProjectOrder Oldentity = new RMC_ProjectOrder();
                    //Oldentity.OrderId = orderid;
                    Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    Oldentity.RawMaterialConsumption = entity.RawMaterialConsumption;
                    Oldentity.OrderBudget = entity.OrderBudget;
                    //OrderManagementCurrent.Add(Oldentity);
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
        public ActionResult DeleteProjectOrder(string KeyValue)
        {
            try
            {
                List<int> ids = new List<int>();
                int key_value = Convert.ToInt32(KeyValue);
                ids.Add(key_value);
                OrderManagementCurrent.Remove(ids);
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
        /// 确认文件
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public ActionResult ConfirmOrder(string KeyValue, RMC_ProcessManagement Entity)
        {
            try
            {
                int OrderId = Convert.ToInt32(KeyValue);
                var file = OrderManagementCurrent.Find(f => f.OrderId == OrderId).First();
                file.ModifiedTime = DateTime.Now;
                file.ConfirmOrder = 1;
                OrderManagementCurrent.Modified(file);
                var tree = TreeCurrent.Find(f => f.TreeName == "生产制程设计").SingleOrDefault();
                if (tree != null)
                {
                    Entity.OrderId = OrderId;
                    Entity.TreeId = tree.TreeID;
                    Entity.Supervision = 0;
                    Entity.QualityInspection = 0;
                    Entity.MemberCompanyId = file.ProjectDemandId;
                }
                ProcessManagementCurrent.Add(Entity);
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功。" }.ToString());
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

        #region 构件厂原材料库存管理
        public ActionResult RawMaterialsIndex()
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
        public ActionResult GridListJsonRawMaterials(/*ProjectInfoViewModel model,*/ string TreeID, JqGridParam jqgridparam, string IsPublic)
        {
            try
            {
                int TreeId;
                //int FolderId = Convert.ToInt32(FolderId);
                if (TreeID == "" || TreeID == null)
                {
                    TreeId = 24;
                }
                else
                {
                    TreeId = Convert.ToInt32(TreeID);
                }

                int total = 0;
                Expression<Func<RMC_RawMaterialLibrary, bool>> func = ExpressionExtensions.True<RMC_RawMaterialLibrary>();
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
                List<RMC_RawMaterialLibrary> listfile = RawMaterialCurrent.FindPage<string>(jqgridparam.page
                                         , jqgridparam.rows
                                         , func
                                         , true
                                         , f => f.RawMaterialId.ToString()
                                         , out total
                                         ).ToList();
                //List<ProduceOrderModel> produceorderlist = new List<ProduceOrderModel>();
                //foreach (var item in listfile)
                //{
                //    ProduceOrderModel produceorder = new ProduceOrderModel();
                //    produceorder.OrderId = item.OrderId;
                //    var order = OrderManagementCurrent.Find(f => f.OrderId == item.OrderId).SingleOrDefault();
                //    var demand = ProjectManagementCurrent.Find(f => f.ProjectDemandId == order.ProjectDemandId).SingleOrDefault();
                //    var member = MemberLibraryCurrent.Find(f => f.MemberID == demand.MemberId).SingleOrDefault();
                //    produceorder.MemberNumbering = member.MemberNumbering;
                //    produceorder.MemberName = member.MemberModel;
                //    produceorder.RawMaterialConsumption = item.RawMaterialConsumption;
                //    produceorder.OrderBudget = item.OrderBudget;
                //    produceorder.ConfirmOrder = item.ConfirmOrder;
                //    produceorder.Description = item.Description;
                //    produceorderlist.Add(produceorder);
                //}
                if (listfile.Count() > 0)// && listtree.Count() > 0
                {

                    //ListData0 = ListToDataTable(listtree);
                    ListData1 = DataHelper.ListToDataTable(listfile);
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
                return Content("<script>alertDialog('" + ex.Message + "');</script>");
            }
        }

        /// <summary>
        /// 表单视图
        /// </summary>
        /// <returns></returns>
        public ActionResult RawMaterialsForm()
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
        public ActionResult SetRawMaterialsForm(string KeyValue)
        {
            RMC_RawMaterialLibrary entity = new RMC_RawMaterialLibrary();
            if (!string.IsNullOrEmpty(KeyValue))
            {
                int key_value = Convert.ToInt32(KeyValue);
                entity = RawMaterialCurrent.Find(f => f.RawMaterialId == key_value).SingleOrDefault();
            }
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
        public virtual ActionResult SubmitRawMaterialsForm(RMC_RawMaterialLibrary entity, string KeyValue, string OrderId)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_RawMaterialLibrary Oldentity = RawMaterialCurrent.Find(t => t.RawMaterialId == key_value).SingleOrDefault();//获取没更新之前实体对象
                    //Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    Oldentity.RawMaterialName = entity.RawMaterialName;
                    Oldentity.RawMaterialNumber = entity.RawMaterialNumber;
                    Oldentity.RawMaterialStandard = entity.RawMaterialStandard;
                    Oldentity.RawMaterialUnit = entity.RawMaterialUnit;
                    Oldentity.RawMaterialNumber = entity.RawMaterialNumber;
                    Oldentity.Description = entity.Description;
                    RawMaterialCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                    //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_RawMaterialLibrary Oldentity = new RMC_RawMaterialLibrary();
                    Oldentity.TreeId = key_value;
                    Oldentity.RawMaterialName = entity.RawMaterialName;
                    Oldentity.RawMaterialNumber = entity.RawMaterialNumber;
                    Oldentity.RawMaterialStandard = entity.RawMaterialStandard;
                    Oldentity.RawMaterialUnit = entity.RawMaterialUnit;
                    Oldentity.RawMaterialNumber = entity.RawMaterialNumber;
                    Oldentity.Description = entity.Description;
                    RawMaterialCurrent.Add(Oldentity);
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
        public ActionResult DeleteRawMaterials(string KeyValue)
        {
            try
            {
                List<int> ids = new List<int>();
                int key_value = Convert.ToInt32(KeyValue);
                ids.Add(key_value);
                RawMaterialCurrent.Remove(ids);
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

        #region 构件厂生产制程设计
        public ActionResult ProcessIndex()
        {
            return View();
        }

        public ActionResult GridListJsonProcess(/*ProjectInfoViewModel model,*/ string TreeID, JqGridParam jqgridparam, string IsPublic)
        {
            try
            {
                int TreeId;
                //int FolderId = Convert.ToInt32(FolderId);
                if (TreeID == "" || TreeID == null)
                {
                    TreeId = 24;
                }
                else
                {
                    TreeId = Convert.ToInt32(TreeID);
                }

                int total = 0;
                Expression<Func<RMC_ProcessManagement, bool>> func = ExpressionExtensions.True<RMC_ProcessManagement>();
                func = f => f.TreeId == TreeId;
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
                List<RMC_ProcessManagement> listfile = ProcessManagementCurrent.FindPage<string>(jqgridparam.page
                                         , jqgridparam.rows
                                         , func
                                         , true
                                         , f => f.ProduceStartDate.ToString()
                                         , out total
                                         ).ToList();
                List<ProcessManagementModel> processmanagementlist = new List<ProcessManagementModel>();
                foreach (var item in listfile)
                {
                    ProcessManagementModel processmanagement = new ProcessManagementModel();
                    processmanagement.ProcessId = item.ProcessId;
                    var Process = ProcessManagementCurrent.Find(f => f.ProcessId == item.ProcessId).SingleOrDefault();
                    var order = OrderManagementCurrent.Find(f => f.OrderId == Process.OrderId).SingleOrDefault();
                    var demand = ProjectManagementCurrent.Find(f => f.ProjectDemandId == order.ProjectDemandId).SingleOrDefault();
                    var member = MemberLibraryCurrent.Find(f => f.MemberID == demand.MemberId).SingleOrDefault();
                    processmanagement.MemberNumbering = member.MemberNumbering.ToString();
                    processmanagement.MemberName = member.MemberName;
                    processmanagement.MemberNumber = demand.MemberNumber;
                    processmanagement.ProduceStartDate = item.ProduceStartDate;
                    processmanagement.ProduceEndDate = item.ProduceEndDate;
                    processmanagement.SetUpProcessing = item.SetUpProcessing;
                    processmanagement.SetUpProcessingDays = item.SetUpProcessingDays;
                    processmanagement.SubmergedArcProcessing = item.SubmergedArcProcessing;
                    processmanagement.SubmergedArcProcessingDays = item.SubmergedArcProcessingDays;
                    processmanagement.RivetingProcessing = item.RivetingProcessing;
                    processmanagement.RivetingProcessingDays = item.RivetingProcessingDays;
                    processmanagement.PaintProcessing = item.PaintProcessing;
                    processmanagement.PaintProcessingDays = item.PaintProcessingDays;
                    processmanagement.Description = item.Description;
                    processmanagementlist.Add(processmanagement);
                }
                if (processmanagementlist.Count() > 0)// && listtree.Count() > 0
                {
                    //ListData0 = ListToDataTable(listtree);
                    ListData1 = DataHelper.ListToDataTable(processmanagementlist);
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
                    ListData = DataHelper.ListToDataTable(processmanagementlist);
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
        public ActionResult ProcessForm()
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
        public ActionResult SetProcessForm(string KeyValue)
        {
            RMC_ProcessManagement entity = new RMC_ProcessManagement();
            if (!string.IsNullOrEmpty(KeyValue))
            {
                int key_value = Convert.ToInt32(KeyValue);
                entity = ProcessManagementCurrent.Find(f => f.ProcessId == key_value).SingleOrDefault();
            }
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
        public virtual ActionResult SubmitProcessForm(RMC_ProcessManagement entity, string KeyValue, string OrderId)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = ProcessManagementCurrent.Find(t => t.ProcessId == key_value).SingleOrDefault();//获取没更新之前实体对象
                    //Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    Oldentity.PaintProcessing = entity.PaintProcessing;
                    Oldentity.PaintProcessingDays = entity.PaintProcessingDays;
                    Oldentity.ProduceStartDate = entity.ProduceStartDate;
                    Oldentity.ProduceEndDate = entity.ProduceEndDate;
                    Oldentity.RivetingProcessing = entity.RivetingProcessing;
                    Oldentity.RivetingProcessingDays = entity.RivetingProcessingDays;
                    Oldentity.SetUpProcessing = entity.SetUpProcessing;
                    Oldentity.SetUpProcessingDays = entity.SetUpProcessingDays;
                    Oldentity.SubmergedArcProcessing = entity.SubmergedArcProcessing;
                    Oldentity.SubmergedArcProcessingDays = entity.SubmergedArcProcessingDays;
                    Oldentity.Description = entity.Description;
                    ProcessManagementCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                    //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = new RMC_ProcessManagement();
                    //Oldentity.TreeId = key_value;
                    Oldentity.PaintProcessing = entity.PaintProcessing;
                    Oldentity.PaintProcessingDays = entity.PaintProcessingDays;
                    Oldentity.ProduceStartDate = entity.ProduceStartDate;
                    Oldentity.ProduceEndDate = entity.ProduceStartDate;
                    Oldentity.RivetingProcessing = entity.RivetingProcessing;
                    Oldentity.RivetingProcessingDays = entity.RivetingProcessingDays;
                    Oldentity.SetUpProcessing = entity.SetUpProcessing;
                    Oldentity.SetUpProcessingDays = entity.SetUpProcessingDays;
                    Oldentity.SubmergedArcProcessing = entity.SubmergedArcProcessing;
                    Oldentity.SubmergedArcProcessingDays = entity.SubmergedArcProcessingDays;
                    Oldentity.Supervision = 0;
                    Oldentity.QualityInspection = 0;
                    Oldentity.Description = entity.Description;
                    //ProcessManagementCurrent.Add(Oldentity);
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
        public ActionResult DeleteProcess(string KeyValue)
        {
            try
            {
                List<int> ids = new List<int>();
                int key_value = Convert.ToInt32(KeyValue);
                ids.Add(key_value);
                ProcessManagementCurrent.Remove(ids);
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

        #region 构件厂生产制程管理
        public ActionResult ProcessManagementIndex()
        {
            return View();
        }
        public ActionResult GridListJson( string KeyValue)
        {
            RMC_ProcessManagement entity = new RMC_ProcessManagement();
            if (!string.IsNullOrEmpty(KeyValue))
            {
                int key_value = Convert.ToInt32(KeyValue);
                entity = ProcessManagementCurrent.Find(f => f.TreeId == key_value).SingleOrDefault();
            }
            return Content(entity.ToJson());

        }

        public ActionResult GridListJsonProcessManagement(/*ProjectInfoViewModel model,*/ string TreeID, JqGridParam jqgridparam, string IsPublic)
        {
            try
            {
                int TreeId;
                //int FolderId = Convert.ToInt32(FolderId);
                if (TreeID == "" || TreeID == null)
                {
                    TreeId = 24;
                }
                else
                {
                    TreeId = Convert.ToInt32(TreeID);
                }

                int total = 0;
                Expression<Func<RMC_ProcessManagement, bool>> func = ExpressionExtensions.True<RMC_ProcessManagement>();
                func = f => f.TreeId == TreeId;
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
                List<RMC_ProcessManagement> listfile = ProcessManagementCurrent.FindPage<string>(jqgridparam.page
                                         , jqgridparam.rows
                                         , func
                                         , true
                                         , f => f.ProduceStartDate.ToString()
                                         , out total
                                         ).ToList();
                List<ProcessManagementModel> processmanagementlist = new List<ProcessManagementModel>();
                foreach (var item in listfile)
                {
                    ProcessManagementModel processmanagement = new ProcessManagementModel();
                    processmanagement.ProcessId = item.ProcessId;
                    processmanagement.OrderId = item.OrderId;
                    var Process = ProcessManagementCurrent.Find(f => f.ProcessId == item.ProcessId).SingleOrDefault();
                    var order = OrderManagementCurrent.Find(f => f.OrderId == Process.OrderId).SingleOrDefault();
                    var demand = ProjectManagementCurrent.Find(f => f.ProjectDemandId == order.ProjectDemandId).SingleOrDefault();
                    var member = MemberLibraryCurrent.Find(f => f.MemberID == demand.MemberId).SingleOrDefault();
                    processmanagement.MemberNumbering = member.MemberNumbering.ToString();
                    processmanagement.MemberName = member.MemberName;
                    processmanagement.MemberNumber = demand.MemberNumber;
                    processmanagement.ProduceStartDate = item.ProduceStartDate;
                    processmanagement.ProduceEndDate = item.ProduceEndDate;
                    processmanagement.IsSetUpProcessingTask = item.IsSetUpProcessingTask;
                    processmanagement.IsSetUpProcessing = item.IsSetUpProcessing;
                    processmanagement.IsSubmergedArcProcessingTask = item.IsSubmergedArcProcessingTask;
                    processmanagement.IsSubmergedArcProcessing = item.IsSubmergedArcProcessing;
                    processmanagement.IsRivetingProcessingTask = item.IsRivetingProcessingTask;
                    processmanagement.IsRivetingProcessing = item.IsRivetingProcessing;
                    processmanagement.IsPaintProcessingTask = item.IsPaintProcessingTask;
                    processmanagement.IsPaintProcessing = item.IsPaintProcessing;
                    processmanagement.QualifiedNumber = item.QualifiedNumber;
                    processmanagement.UnqualifiedNumber = item.UnqualifiedNumber;
                    processmanagement.QualityInspection = item.QualityInspection;
                    processmanagement.IsPacking = item.IsPacking;
                    processmanagement.Supervision = item.Supervision;
                    processmanagement.Description = item.Description;
                    processmanagementlist.Add(processmanagement);
                }
                if (processmanagementlist.Count() > 0)// && listtree.Count() > 0
                {
                    //ListData0 = ListToDataTable(listtree);
                    ListData1 = DataHelper.ListToDataTable(processmanagementlist);
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
                    ListData = DataHelper.ListToDataTable(processmanagementlist);
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
        public ActionResult ProcessManagementForm()
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
        public ActionResult SetProcessManagementForm(string KeyValue)
        {
            RMC_ProcessManagement entity = new RMC_ProcessManagement();
            if (!string.IsNullOrEmpty(KeyValue))
            {
                int key_value = Convert.ToInt32(KeyValue);
                entity = ProcessManagementCurrent.Find(f => f.ProcessId == key_value).SingleOrDefault();
            }
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
        public virtual ActionResult SubmitProcessManagementForm(RMC_ProcessManagement entity, string KeyValue, string OrderId)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = ProcessManagementCurrent.Find(t => t.ProcessId == key_value).SingleOrDefault();//获取没更新之前实体对象
                    //Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    Oldentity.PaintProcessing = entity.PaintProcessing;
                    Oldentity.QualityInspection = entity.QualityInspection;
                    Oldentity.RivetingProcessing = entity.RivetingProcessing;
                    Oldentity.SetUpProcessing = entity.SetUpProcessing;
                    Oldentity.SubmergedArcProcessing = entity.SubmergedArcProcessing;
                    Oldentity.Supervision = entity.Supervision;
                    Oldentity.Description = entity.Description;
                    ProcessManagementCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                    //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = new RMC_ProcessManagement();
                    Oldentity.TreeId = key_value;
                    Oldentity.PaintProcessing = entity.PaintProcessing;
                    Oldentity.QualityInspection = entity.QualityInspection;
                    Oldentity.RivetingProcessing = entity.RivetingProcessing;
                    Oldentity.SetUpProcessing = entity.SetUpProcessing;
                    Oldentity.SubmergedArcProcessing = entity.SubmergedArcProcessing;
                    Oldentity.Supervision = entity.Supervision;
                    Oldentity.Description = entity.Description;
                    ///ProcessManagementCurrent.Add(Oldentity);
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
        public ActionResult DeleteProcessManagement(string KeyValue)
        {
            try
            {
                List<int> ids = new List<int>();
                int key_value = Convert.ToInt32(KeyValue);
                ids.Add(key_value);
                ProcessManagementCurrent.Remove(ids);
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
        /// 领取组立任务
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult SetUpProcessingTask(string KeyValue)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "成功。" : "领取成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = ProcessManagementCurrent.Find(t => t.ProcessId == key_value).SingleOrDefault();//获取没更新之前实体对象                                                                                                              //Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    if (Oldentity.IsSetUpProcessingTask == 1)
                    {
                        Message = "该任务已领取过";
                    }
                    else
                    {
                        Oldentity.IsSetUpProcessingTask = 1;
                        ProcessManagementCurrent.Modified(Oldentity);
                        IsOk = 1;//更新实体对象
                    }

                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        /// <summary>
        /// 组立提交
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult SetUpProcessing(string KeyValue)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "成功。" : "提交成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = ProcessManagementCurrent.Find(t => t.ProcessId == key_value).SingleOrDefault();//获取没更新之前实体对象                                                                                                              //Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    if (Oldentity.IsSetUpProcessing == 1)
                    {
                        Message = "该工序已提交过！";
                    }
                    else
                    {
                        Oldentity.IsSetUpProcessing = 1;
                        ProcessManagementCurrent.Modified(Oldentity);
                        IsOk = 1;//更新实体对象
                    }
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        /// <summary>
        /// 领取埋弧任务
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult SubmergedArcProcessingTask(string KeyValue)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "成功。" : "领取成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = ProcessManagementCurrent.Find(t => t.ProcessId == key_value).SingleOrDefault();//获取没更新之前实体对象                                                                                                              //Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    if (Oldentity.IsSubmergedArcProcessingTask == 1)
                    {
                        Message = "该任务已领取过";
                    }
                    else
                    {
                        Oldentity.IsSubmergedArcProcessingTask = 1;
                        ProcessManagementCurrent.Modified(Oldentity);
                        IsOk = 1;//更新实体对象
                    }

                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        /// <summary>
        /// 埋弧提交
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult SubmergedArcProcessing(string KeyValue)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "成功。" : "提交成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = ProcessManagementCurrent.Find(t => t.ProcessId == key_value).SingleOrDefault();//获取没更新之前实体对象                                                                                                              //Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    if (Oldentity.IsSubmergedArcProcessing == 1)
                    {
                        Message = "该工序已提交过！";
                    }
                    else
                    {
                        Oldentity.IsSubmergedArcProcessing = 1;
                        ProcessManagementCurrent.Modified(Oldentity);
                        IsOk = 1;//更新实体对象
                    }
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        /// <summary>
        /// 领取铆焊任务
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult RivetingProcessingTask(string KeyValue)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "成功。" : "领取成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = ProcessManagementCurrent.Find(t => t.ProcessId == key_value).SingleOrDefault();//获取没更新之前实体对象                                                                                                              //Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    if (Oldentity.IsRivetingProcessingTask == 1)
                    {
                        Message = "该任务已领取过";
                    }
                    else
                    {
                        Oldentity.IsRivetingProcessingTask = 1;
                        ProcessManagementCurrent.Modified(Oldentity);
                        IsOk = 1;//更新实体对象
                    }

                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        /// <summary>
        /// 铆焊提交
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult RivetingProcessing(string KeyValue)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "成功。" : "提交成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = ProcessManagementCurrent.Find(t => t.ProcessId == key_value).SingleOrDefault();//获取没更新之前实体对象                                                                                                              //Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    if (Oldentity.IsRivetingProcessing == 1)
                    {
                        Message = "该工序已提交过！";
                    }
                    else
                    {
                        Oldentity.IsRivetingProcessing = 1;
                        ProcessManagementCurrent.Modified(Oldentity);
                        IsOk = 1;//更新实体对象
                    }
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        /// <summary>
        /// 领取油漆任务
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult PaintProcessingTask(string KeyValue)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "成功。" : "领取成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = ProcessManagementCurrent.Find(t => t.ProcessId == key_value).SingleOrDefault();//获取没更新之前实体对象                                                                                                              //Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    if (Oldentity.IsPaintProcessingTask == 1)
                    {
                        Message = "该任务已领取过";
                    }
                    else
                    {
                        Oldentity.IsPaintProcessingTask = 1;
                        ProcessManagementCurrent.Modified(Oldentity);
                        IsOk = 1;//更新实体对象
                    }

                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        /// <summary>
        /// 油漆提交
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult PaintProcessing(string KeyValue)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "成功。" : "提交成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = ProcessManagementCurrent.Find(t => t.ProcessId == key_value).SingleOrDefault();//获取没更新之前实体对象                                                                                                              //Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    if (Oldentity.IsPaintProcessing == 1)
                    {
                        Message = "该工序已提交过！";
                    }
                    else
                    {
                        Oldentity.IsPaintProcessing = 1;
                        ProcessManagementCurrent.Modified(Oldentity);
                        IsOk = 1;//更新实体对象
                    }
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        /// <summary>
        /// 质检提交
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult QualityInspection(string KeyValue, string QualityInspection)
        {
            try
            {
                int IsOk = 0;
                string Message = QualityInspection == "2" ? "产品驳回！" : "质检通过。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = ProcessManagementCurrent.Find(t => t.ProcessId == key_value).SingleOrDefault();//获取没更新之前实体对象                                                                                                              //Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    if (Oldentity.QualityInspection == 1|| Oldentity.QualityInspection ==2)
                    {
                        Message = "该工序已提交过！";
                    }
                    else
                    {
                        Oldentity.QualityInspection =Convert.ToInt32(QualityInspection);
                        ProcessManagementCurrent.Modified(Oldentity);
                        IsOk = 1;//更新实体对象
                    }
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        /// <summary>
        /// 监理提交
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult Supervision(string KeyValue,string Supervision)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "2" ? "成功。" : "质检通过。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = ProcessManagementCurrent.Find(t => t.ProcessId == key_value).SingleOrDefault();//获取没更新之前实体对象                                                                                                              //Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    if (Oldentity.Supervision == 1|| Oldentity.Supervision==2)
                    {
                        Message = "该工序已提交过！";
                    }
                    else
                    {
                        Oldentity.Supervision =Convert.ToInt32(Supervision);
                        ProcessManagementCurrent.Modified(Oldentity);
                        IsOk = 1;//更新实体对象
                    }
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        /// <summary>
        /// 打包
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult Packing(string KeyValue,RMC_ShipManagement Entity)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement ProManagement = ProcessManagementCurrent.Find(t => t.ProcessId == key_value).SingleOrDefault();//获取没更新之前实体对象                                                                                                              //Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    RMC_ProjectDemand Demand = ProjectManagementCurrent.Find(f => f.ProjectDemandId == ProManagement.OrderId).SingleOrDefault();
                    RMC_ShipManagement ShipManagement = ShipManagementCurrent.Find(f => f.MemberId == Demand.MemberId).SingleOrDefault();
                    if (ProManagement.IsPacking ==1)
                    {
                        Message = "该工序已提交过！";
                    }
                    else
                    {
                        ProManagement.IsPacking =1;
                        ProcessManagementCurrent.Modified(ProManagement);
                        ShipManagement.IsPackaged = 1;
                        ShipManagementCurrent.Modified(ShipManagement);
                        IsOk = 1;//更新实体对象

                    }
                }
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = Message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        #endregion

        #region 二维码解析，生成，打印
        /// <summary>
        /// 表单
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult QRCodeForm()
        {
            return View();
        }

        public ActionResult SetQRCodeForm(string KeyValue, string OrderId, QRCodeModel EntityModel)
        {
            //long key_value = Convert.ToInt64(KeyValue);
            int order_id = Convert.ToInt32(OrderId);
            var Member = MemberLibraryCurrent.Find(f => f.MemberNumbering == KeyValue).SingleOrDefault();
            var Order = OrderManagementCurrent.Find(f => f.OrderId == order_id).SingleOrDefault();
            var MemberDemend = ProjectManagementCurrent.Find(f => f.ProjectDemandId == Order.ProjectDemandId).SingleOrDefault();
            var Project = ProjectInfoCurrent.Find(f => f.ProjectId == MemberDemend.ProjectId).SingleOrDefault();
            EntityModel.MemberName = Member.MemberName;
            EntityModel.MemberNumbering = Member.MemberNumbering;
            EntityModel.MemberModel = Member.MemberModel;
            EntityModel.TheoreticalWeight = Member.TheoreticalWeight;
            EntityModel.ProjectName = Project.ProjectName;
            return Content(EntityModel.ToJson());
        }



        //生成二维码方法一
        private void CreateCode_Simple(string nr)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 4;
            qrCodeEncoder.QRCodeVersion = 8;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            //System.Drawing.Image image = qrCodeEncoder.Encode("4408810820 深圳－广州 小江");
            System.Drawing.Image image = qrCodeEncoder.Encode(nr);
            string filename = DateTime.Now.ToString("yyyymmddhhmmssfff").ToString() + ".jpg";
            string filepath = Server.MapPath(@"~\Upload") + "\\" + filename;
            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
            image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);

            fs.Close();
            image.Dispose();
            //二维码解码
            var codeDecoder = CodeDecoder(filepath);
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="strData">要生成的文字或者数字，支持中文。如： "4408810820 深圳－广州" 或者：4444444444</param>
        /// <param name="qrEncoding">三种尺寸：BYTE ，ALPHA_NUMERIC，NUMERIC</param>
        /// <param name="level">大小：L M Q H</param>
        /// <param name="version">版本：如 8</param>
        /// <param name="scale">比例：如 4</param>
        /// <returns></returns>
        public ActionResult CreateCode_Choose(string strData, string qrEncoding, string level, int version, int scale)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            string encoding = qrEncoding;
            switch (encoding)
            {
                case "Byte":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                    break;
                case "AlphaNumeric":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
                    break;
                case "Numeric":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
                    break;
                default:
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                    break;
            }

            qrCodeEncoder.QRCodeScale = scale;
            qrCodeEncoder.QRCodeVersion = version;
            switch (level)
            {
                case "L":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
                    break;
                case "M":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                    break;
                case "Q":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
                    break;
                default:
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
                    break;
            }
            //文字生成图片
            Image image = qrCodeEncoder.Encode(strData);
            string filename = DateTime.Now.ToString("yyyymmddhhmmssfff").ToString() + ".jpg";
            string virtualPath = this.Server.MapPath("~") + "~/Resource/Document/NetworkDisk/QRCode";
            string filepath = virtualPath + "/" + filename;
            //string filepath = Server.MapPath(@"~\Upload") + "\\" + filename;
            if (Directory.Exists(virtualPath))
            {
                Directory.Delete(virtualPath, true);//pdf路径
                Directory.CreateDirectory(virtualPath);
            }
            else
            {
                Directory.CreateDirectory(virtualPath);//如果文件夹不存在，则创建
            }
            //如果文件夹不存在，则创建
            //if (!Directory.Exists(filepath))
            //    Directory.CreateDirectory(filepath);
            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
            image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
            fs.Close();
            image.Dispose();
            return Content("../../Resource/Document/NetworkDisk/QRCode/" + filename);
        }

        /// <summary>
        /// 二维码解码
        /// </summary>
        /// <param name="filePath">图片路径</param>
        /// <returns></returns>
        public string CodeDecoder(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return null;
            Bitmap myBitmap = new Bitmap(Image.FromFile(filePath));
            QRCodeDecoder decoder = new QRCodeDecoder();
            string decodedString = decoder.decode(new QRCodeBitmapImage(myBitmap));
            return decodedString;
        }

        /// <summary>
        /// 打印当前页
        /// </summary>
        /// <returns></returns>
        public ActionResult PrintPage()
        {
            return View();
        }

        #endregion

        #region 构件厂发货管理
        public ActionResult ShipManagementIndex()
        {
            return View();
        }
        public ActionResult GridListJsonShipManagement(/*ProjectInfoViewModel model,*/ string TreeID, JqGridParam jqgridparam, string IsPublic)
        {
            try
            {
                int TreeId;
                //int FolderId = Convert.ToInt32(FolderId);
                if (TreeID == "" || TreeID == null)
                {
                    TreeId = 24;
                }
                else
                {
                    TreeId = Convert.ToInt32(TreeID);
                }

                int total = 0;
                Expression<Func<RMC_ShipManagement, bool>> func = ExpressionExtensions.True<RMC_ShipManagement>();
                func = f => f.TreeId == TreeId&&f.IsPackaged==1;
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
                List<RMC_ShipManagement> listfile = ShipManagementCurrent.FindPage<string>(jqgridparam.page
                                         , jqgridparam.rows
                                         , func
                                         , true
                                         , f => f.ShipId.ToString()
                                         , out total
                                         ).ToList();
                //List<ProcessManagementModel> processmanagementlist = new List<ProcessManagementModel>();
                foreach (var item in listfile)
                {
                    if (!string.IsNullOrEmpty(item.MemberId.ToString()))
                    {
                        var member = MemberLibraryCurrent.Find(f => f.MemberID == item.MemberId).SingleOrDefault();
                        item.MemberName = member.MemberName;
                        item.MemberModel = member.MemberModel;
                        item.MemberNumbering = member.MemberNumbering;
                        item.UnitPrice = member.UnitPrice;
                        string a = member.UnitPrice;
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

                        string membernumber = item.ShipNumber;
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
                        item.TotalPrice = (Number * d).ToString();
                    }
                }
                if (listfile.Count() > 0)// && listtree.Count() > 0
                {
                    //ListData0 = ListToDataTable(listtree);
                    ListData1 = DataHelper.ListToDataTable(listfile);
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
                return Content("<script>alertDialog('" + ex.Message + "');</script>");
            }
        }

        /// <summary>
        /// 表单视图
        /// </summary>
        /// <returns></returns>
        public ActionResult ShipManagementForm()
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
        public ActionResult SetShipManagementForm(string KeyValue)
        {
            RMC_ShipManagement entity = new RMC_ShipManagement();
            if (!string.IsNullOrEmpty(KeyValue))
            {
                int key_value = Convert.ToInt32(KeyValue);
                entity = ShipManagementCurrent.Find(f => f.ShipId == key_value).SingleOrDefault();
            }
            return Content(entity.ToJson());
            //return Json(entity);
        }

        /// <summary>
        /// 提交文件表单
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="KeyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        //[LoginAuthorize]
        public virtual ActionResult SubmitShipManagementForm(RMC_ShipManagement entity, string KeyValue, string TreeId)
        {
            try
            {
                int IsOk = 0;
                string message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ShipManagement Oldentity = ShipManagementCurrent.Find(t => t.ShipId == key_value).SingleOrDefault();//获取没更新之前实体对象
                    //Oldentity.OrderId = entity.OrderId;//给旧实体重新赋值
                    Oldentity.Description = entity.Description;
                    Oldentity.LogisticsStatus = entity.LogisticsStatus;
                    Oldentity.ShipMan = entity.ShipMan;
                    Oldentity.MemberClassId = entity.MemberClassId;
                    Oldentity.MemberId = entity.MemberId;
                    Oldentity.ShipNumber = entity.ShipNumber;
                    Oldentity.ShippingAddress = entity.ShippingAddress;
                    Oldentity.ShippingMan = entity.ShippingMan;
                    Oldentity.ShippingTEL = entity.ShippingTEL;
                    ShipManagementCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                    //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    int TreeID = Convert.ToInt32(TreeId);
                    RMC_ShipManagement Oldentity = new RMC_ShipManagement();
                    Oldentity.TreeId = TreeID;
                    Oldentity.MemberId = entity.MemberId;
                    Oldentity.Description = entity.Description;
                    Oldentity.LogisticsStatus ="0";
                    Oldentity.ShipMan = entity.ShipMan;
                    Oldentity.MemberClassId = entity.MemberClassId;
                    Oldentity.MemberId = entity.MemberId;
                    Oldentity.ShipNumber ="0";
                    Oldentity.ShippingAddress = entity.ShippingAddress;
                    Oldentity.ShippingMan = entity.ShippingMan;
                    Oldentity.ShippingTEL = entity.ShippingTEL;
                    ShipManagementCurrent.Add(Oldentity);
                    IsOk = 1;
                    //this.WriteLog(IsOk, entity, null, KeyValue, Message);
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
        /// 删除（销毁）文件
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult DeleteShipManagement(string KeyValue)
        {
            try
            {
                List<int> ids = new List<int>();
                int key_value = Convert.ToInt32(KeyValue);
                ids.Add(key_value);
                ShipManagementCurrent.Remove(ids);
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

        public ActionResult Ship(string KeyValue)
        {
            try
            {
                int key_value = Convert.ToInt32(KeyValue);
                RMC_ShipManagement Oldentity = ShipManagementCurrent.Find(t => t.ShipId == key_value).SingleOrDefault();//获取没更新之前实体对象
                Oldentity.ShipDate = DateTime.Now;
                Oldentity.LogisticsStatus = "1";
                Oldentity.ShippingMan = "1";
                ShipManagementCurrent.Modified(Oldentity);
                RMC_ProjectWarehouse ProjectWarehouse = ProjectWarehouseCurrent.Find(f => f.MemberId == Oldentity.MemberId).SingleOrDefault();

                int shipnumber = 0;
                if (ProjectWarehouse.ProjectWarehouseId > 0)
                {
                    string a = Oldentity.ShipNumber;
                    char[] b = a.ToArray();
                    string ShipNumber = "";
                    for (int sn = 0; sn < b.Length; sn++)
                    {
                        if (("0123456789.").IndexOf(b[sn] + "") != -1)
                        {
                            ShipNumber += b[sn];
                        }
                    }
                    shipnumber = Convert.ToInt32(ShipNumber);

                    RMC_ProjectWarehouse OldEntity = ProjectWarehouseCurrent.Find(f => f.ProjectWarehouseId == ProjectWarehouse.ProjectWarehouseId).SingleOrDefault();
                    if (ProjectWarehouse.InStock != null|| ProjectWarehouse.InStock.ToString()!="")
                    {
                        ProjectWarehouse.InStock = ProjectWarehouse.InStock + shipnumber;
                    }
                    else
                    {
                        ProjectWarehouse.InStock = 0;
                        ProjectWarehouse.InStock = ProjectWarehouse.InStock + shipnumber;
                    }
                    ProjectWarehouse.ModifyTime = DateTime.Now;
                    ProjectWarehouse.IsShiped =1;
                    ProjectWarehouse.Class = "入库".Trim();
                    ProjectWarehouseCurrent.Modified(ProjectWarehouse);
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

        #endregion

    }
}