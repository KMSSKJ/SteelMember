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
using LeaRun.WebApp.Areas.SteelMember.Models;
using System.Text;
using LeaRun.Entity;
using LeaRun.WebApp.Controllers;
using System.Diagnostics;

namespace LeaRun.WebApp.Areas.SteelMember.Controllers
{
    public class MemberProduceController : BaseController
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
        [Inject]
        public MemberUnitIBLL MemberUnitCurrent { get; set; }
        [Inject]
        public AnalysisRawMaterialIBLL AnalysisRawMaterialCurrent { get; set; }
        [Inject]
        public OrderMemberIBLL OrderMemberCurrent { get; set; }
        [Inject]
        public MemberMaterialIBLL MemberMaterialCurrent { get; set; }
        [Inject]
        public MemberProcessIBLL MemberProcessCurrent { get; set; }
        [Inject]
        public FactoryWarehouseIBLL FactoryWarehouseCurrent { get; set; }
        public virtual ActionResult Index()
        {
            return View();
        }

        //获取树字节子节点(自循环)
        public IEnumerable<RMC_Tree> GetSonId(int p_id)
        {
            List<RMC_Tree> list = TreeCurrent.Find(p => p.ParentID == p_id).ToList();
            return list.Concat(list.SelectMany(t => GetSonId(t.TreeID)));
        }


        /// <summary>
        /// 【项目管理】返回树JONS
        /// </summary>
        /// <returns></returns>      
        public ActionResult TreeJson(string ItemId)
        {
            List<RMC_Tree> list, list1, list2;
            list1 = TreeCurrent.Find(t => t.DeleteFlag != 1 && t.ItemClass == 0).ToList();
            list2 = TreeCurrent.Find(t => t.DeleteFlag != 1 && t.ItemClass == 3).ToList();
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
        /// 【项目管理】返回树JONS(生产订单)
        /// </summary>
        /// <returns></returns>      
        public ActionResult TreeJsonOrder(string ItemId)
        {
            int itemid = Convert.ToInt32(ItemId);

            List<RMC_Tree> list, list1, list2;
            list1 = TreeCurrent.Find(t => t.DeleteFlag != 1 && t.ItemClass == 0).ToList();
            list2 = TreeCurrent.Find(t => t.DeleteFlag != 1 && t.ItemClass == 3).ToList();
            list = list1.Concat(list2).Distinct().ToList();

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

        /// <summary>
        /// 【项目管理】返回树JONS
        /// </summary>
        /// <returns></returns>      
        public ActionResult TreeJsonProcess(string ItemId)
        {
            int _ItemId = Convert.ToInt32(ItemId);
            List<RMC_Tree> list, list1, list2;
            list1 = TreeCurrent.Find(t => t.DeleteFlag != 1 && t.ItemClass == 0).ToList();
            list2 = TreeCurrent.Find(t => t.DeleteFlag != 1 && t.ItemID == _ItemId && t.ItemClass == 2).ToList();
            list = list1.Concat(list2).Distinct().ToList();

            List<RMC_Tree> lists = new List<RMC_Tree>();

            List<RMC_ProjectOrder> OrderList = OrderManagementCurrent.Find(f => f.OrderId > 0 && f.ConfirmOrder == 1).ToList();
            for (int i = 0; i < OrderList.Count(); i++)
            {
                RMC_Tree tree = new RMC_Tree();
                tree.TreeID = OrderList[i].OrderId;
                tree.TreeName = "订单-" + OrderList[i].OrderNumbering;
                tree.Icon = OrderList[i].Icon;
                tree.State = 0;
                tree.ParentID = OrderList[i].TreeId;
                lists.Add(tree);
            }
            list = list.Concat(lists).ToList();//把集合A.B合并
                                               // List<int> Result = listA.Union(listB).ToList<int>();          //剔除重复项 


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
        /// 

        public ActionResult GridListJsonOrder(FileViewModel model, string TreeId, JqGridParam jqgridparam, string ConfirmOrder, string Productioned, string ParameterJson)
        {
            try
            {
                #region 查询条件拼接
                int _ConfirmOrder = Convert.ToInt32(ConfirmOrder);
                int _Productioned = Convert.ToInt32(Productioned);
                if (ParameterJson != null)
                {
                    if (ParameterJson != "[{\"OrderNumbering\":\"\",\"InBeginTime\":\"\",\"InEndTime\":\"\"}]")
                    {
                        List<FileViewModel> query_member = JsonHelper.JonsToList<FileViewModel>(ParameterJson);
                        for (int i = 0; i < query_member.Count(); i++)
                        {
                            model.OrderNumbering = query_member[i].OrderNumbering;
                            model.InBeginTime = query_member[i].InBeginTime;
                            model.InEndTime = query_member[i].InEndTime;
                        }
                    }
                }

                Expression<Func<RMC_ProjectOrder, bool>> func = ExpressionExtensions.True<RMC_ProjectOrder>();
                Expression<Func<RMC_ProjectOrder, bool>> func0 = ExpressionExtensions.True<RMC_ProjectOrder>();
                func0 = f => f.DeleteFlag != 1 && f.IsSubmit == 1 && f.ConfirmOrder == _ConfirmOrder && f.Productioned == _Productioned;
                Func<RMC_ProjectOrder, bool> func1 = f => f.TreeId != 0;

                var _a = model.OrderNumbering != null && model.OrderNumbering.ToString() != "";
                var _b = model.InBeginTime != null && model.InBeginTime.ToString() != "0001/1/1 0:00:00";
                var _c = model.InEndTime != null && model.InEndTime.ToString() != "0001/1/1 0:00:00";

                if (_a && _b && _c)
                {
                    func1 = f => f.OrderNumbering.Contains(model.OrderNumbering) && f.CreateTime >= model.InBeginTime && f.CreateTime <= model.InEndTime;
                }
                else if (_a)
                {
                    func = func.And(f => f.OrderNumbering.Contains(model.OrderNumbering));
                    func1 = f => f.OrderNumbering.Contains(model.OrderNumbering);
                }
                else if (_b)
                {
                    func = func.And(f => f.CreateTime >= model.InBeginTime);
                    func1 = f => f.CreateTime >= model.InBeginTime;
                }
                else if (_c)
                {
                    func = func.And(f => f.CreateTime <= model.InEndTime);
                    func1 = f => f.CreateTime <= model.InEndTime;
                }
                else if (_a && _b)
                {
                    func1 = f => f.OrderNumbering.Contains(model.OrderNumbering) && f.CreateTime >= model.InBeginTime;
                }
                else if (_a && _c)
                {
                    func1 = f => f.OrderNumbering.Contains(model.OrderNumbering) && f.CreateTime <= model.InEndTime;
                }
                else if (_b && _c)
                {
                    func1 = f => f.CreateTime >= model.InBeginTime && f.CreateTime <= model.InEndTime;
                }
                #endregion

                var ProjectOrderList_ = new List<RMC_ProjectOrder>();
                //var ProjectDemandModelList_ = new List<RMC_ProjectOrder>();

                Stopwatch watch = CommonHelper.TimerStart();
                int total = 0;
                List<RMC_ProjectOrder> ProjectOrderList = new List<RMC_ProjectOrder>();
                if (TreeId == "" || (_ConfirmOrder == 2 && _Productioned == 2))
                {
                    func.And(f => f.DeleteFlag != 1 & f.ProjectDemandId > 0);

                    ProjectOrderList = ProjectOrderList_ = OrderManagementCurrent.FindPage<string>(jqgridparam.page
                                             , jqgridparam.rows
                                             , func
                                             , false
                                             , f => f.CreateTime.ToString()
                                             , out total
                                             ).ToList();
                }
                else
                {
                    //int _id = Convert.ToInt32(TreeId);
                    //var list = GetSonId(_id).ToList();

                    //list.Add(TreeCurrent.Find(p => p.TreeID == _id).Single());
                    //foreach (var item in list)
                    //{
                    //    var _ProjectOrderList = OrderManagementCurrent.Find(m => m.TreeId == item.TreeID).ToList();
                    //    if (_ProjectOrderList.Count() > 0)
                    //    {
                    //        ProjectOrderList = ProjectOrderList.Concat(_ProjectOrderList).ToList();
                    //    }
                    //}
                    ProjectOrderList = OrderManagementCurrent.FindPage<string>(jqgridparam.page
                                           , jqgridparam.rows
                                           , func0
                                           , false
                                           , f => f.CreateTime.ToString()
                                           , out total
                                           ).ToList();

                    // ProjectOrderList = ProjectOrderList.Where(func1).ToList();
                    ProjectOrderList_ = ProjectOrderList.Take(jqgridparam.rows * jqgridparam.page).Skip(jqgridparam.rows * (jqgridparam.page - 1)).ToList();
                    total = ProjectOrderList.Count();
                }

                var JsonData = new
                {
                    total = total / jqgridparam.rows + 1,
                    page = jqgridparam.page,
                    records = total,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ProjectOrderList_.OrderByDescending(f => f.CreateTime),
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
        /// 确认生产订单
        /// </summary>
        /// <param name="FolderId"></param>
        /// <returns></returns>
        public ActionResult ConfirmOrder(string KeyValue, RMC_ProcessManagement Entity, RMC_AnalysisRawMaterial AnalysisRawMaterialEntity)
        {
            try
            {
                int OrderId = Convert.ToInt32(KeyValue);
                var file = OrderManagementCurrent.Find(f => f.OrderId == OrderId).First();
                file.ModifiedTime = DateTime.Now;
                file.ConfirmOrder = 1;
                file.ConfirmMan = currentUser.RealName;
                OrderManagementCurrent.Modified(file);

                // var demand = ProjectManagementCurrent.Find(f => f.ProjectDemandId == file.ProjectDemandId).SingleOrDefault();
                //  var member = MemberLibraryCurrent.Find(f => f.MemberID == demand.MemberId).SingleOrDefault();
                AnalysisRawMaterialEntity.TreeId = file.TreeId;
                // AnalysisRawMaterialEntity.MaterialName = member.MemberName;
                AnalysisRawMaterialEntity.OrderNumbering = file.OrderNumbering;
                AnalysisRawMaterialEntity.OrderId = file.OrderId;
                // AnalysisRawMaterialEntity.UnitId = demand.UnitId;
                AnalysisRawMaterialEntity.ParentId = 0;
                AnalysisRawMaterialEntity.RawMaterialId = 0;
                AnalysisRawMaterialCurrent.Add(AnalysisRawMaterialEntity);

                List<RMC_OrderMember> CollarMemberList = OrderMemberCurrent.Find(f => f.OrderId == OrderId).ToList();
                for (int i = 0; i < CollarMemberList.Count(); i++)
                {
                    int _MemberId = Convert.ToInt32(CollarMemberList[i].MemberId);
                    var MemberProcess = MemberProcessCurrent.Find(f => f.MemberId == _MemberId).ToList();
                    for (int i0 = 0; i0 < MemberProcess.Count(); i0++)
                    {
                        Entity.OrderId = OrderId;
                        Entity.MemberId = MemberProcess[i0].MemberId;
                        Entity.MemberProcessId = MemberProcess[i0].MemberProcessId;
                        Entity.IsProcessStatus = 0;
                        ProcessManagementCurrent.Add(Entity);


                    }


                }
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

        #region 原材料用量分析
        public ActionResult AnalysisRawMaterialIndex()
        {
            return View();
        }
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
        public ActionResult TreeGridJsonAnalysisRawMaterial(JqGridParam jqgridparam)
        {
            List<AnalysisRawMaterialModel> AnalysisRawMaterialModellist = new List<AnalysisRawMaterialModel>();
            try
            {
                List<RMC_ProjectOrder> ProjectOrderList = new List<RMC_ProjectOrder>();
                List<RMC_OrderMember> CollarMemberList = new List<RMC_OrderMember>();
                List<RMC_MemberMaterial> MemberMaterialList = new List<RMC_MemberMaterial>();
                List<RMC_RawMaterialLibrary> RawMaterialLibraryList = new List<RMC_RawMaterialLibrary>();

                ProjectOrderList = OrderManagementCurrent.Find(f => f.OrderId > 0 && f.ConfirmOrder == 1 && f.Productioned == 0).ToList();//所有订单
                if (ProjectOrderList.Count > 0)
                {
                    for (int i0 = 0; i0 < ProjectOrderList.Count; i0++)
                    {
                        int OrderId = Convert.ToInt32(ProjectOrderList[i0].OrderId);
                        CollarMemberList = OrderMemberCurrent.Find(f => f.OrderId == OrderId).ToList();//单个订单所需构件
                        if (CollarMemberList.Count() > 0)
                        {
                            for (int i1 = 0; i1 < CollarMemberList.Count(); i1++)
                            {
                                int MemberId = Convert.ToInt32(CollarMemberList[i1].MemberId);
                                MemberMaterialList = MemberMaterialCurrent.Find(f => f.MemberId == MemberId).ToList();//单个构件所需原材料
                                if (MemberMaterialList.Count() > 0)
                                {
                                    for (int i2 = 0; i2 < MemberMaterialList.Count(); i2++)
                                    {
                                        AnalysisRawMaterialModel AnalysisRawMaterialModel = new AnalysisRawMaterialModel();
                                        if (AnalysisRawMaterialModellist.Count() != 0)//判断构造函数是否有数据，没有就新增
                                        {
                                            var a = AnalysisRawMaterialModellist.Where(w => w.RawMaterialId == MemberMaterialList[i2].RawMaterialId);//筛选构造函数中的数据，
                                            if (a.Count() != 0)//判断构造函数中是否有相同数据，没有就新增
                                            {
                                                foreach (var item in a)
                                                {
                                                    item.OrderProcessingNumber += MemberMaterialList[i2].MaterialNumber * CollarMemberList[i1].Qty;
                                                    AnalysisRawMaterialModel.OrderProcessingNumber = item.OrderProcessingNumber;
                                                }

                                            }
                                            else
                                            {
                                                AnalysisRawMaterialModel.RawMaterialId = Convert.ToInt32(MemberMaterialList[i2].RawMaterialId);
                                                var RawMaterial = RawMaterialCurrent.Find(f => f.RawMaterialId == AnalysisRawMaterialModel.RawMaterialId).SingleOrDefault();
                                                AnalysisRawMaterialModel.RawMaterialName = RawMaterial.RawMaterialName;
                                                AnalysisRawMaterialModel.RawMaterialNumber = RawMaterial.RawMaterialNumber;
                                                AnalysisRawMaterialModel.RawMaterialStandard = RawMaterial.RawMaterialStandard;
                                                AnalysisRawMaterialModel.OrderProcessingNumber = MemberMaterialList[i2].MaterialNumber * CollarMemberList[i1].Qty;
                                                AnalysisRawMaterialModel.Description = MemberMaterialList[i2].Description;
                                                AnalysisRawMaterialModellist.Add(AnalysisRawMaterialModel);
                                            }


                                        }
                                        else
                                        {
                                            AnalysisRawMaterialModel.RawMaterialId = Convert.ToInt32(MemberMaterialList[i2].RawMaterialId);
                                            var RawMaterial = RawMaterialCurrent.Find(f => f.RawMaterialId == AnalysisRawMaterialModel.RawMaterialId).SingleOrDefault();
                                            AnalysisRawMaterialModel.RawMaterialName = RawMaterial.RawMaterialName;
                                            AnalysisRawMaterialModel.RawMaterialNumber = RawMaterial.RawMaterialNumber;
                                            AnalysisRawMaterialModel.RawMaterialStandard = RawMaterial.RawMaterialStandard;
                                            AnalysisRawMaterialModel.OrderProcessingNumber = MemberMaterialList[i2].MaterialNumber * CollarMemberList[i1].Qty;
                                            AnalysisRawMaterialModel.Description = MemberMaterialList[i2].Description;
                                            AnalysisRawMaterialModellist.Add(AnalysisRawMaterialModel);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // int total = AnalysisRawMaterialModellist.Distinct().Count();

                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    rows = AnalysisRawMaterialModellist.Distinct(),
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
        public ActionResult AnalysisRawMaterialForm()
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
        public ActionResult SetAnalysisRawMaterialForm(string KeyValue)
        {
            RMC_AnalysisRawMaterial entity = new RMC_AnalysisRawMaterial();
            if (!string.IsNullOrEmpty(KeyValue))
            {
                int key_value = Convert.ToInt32(KeyValue);
                entity = AnalysisRawMaterialCurrent.Find(f => f.AnalysisRawMaterialId == key_value).SingleOrDefault();
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
        public virtual ActionResult SubmitAnalysisRawMaterialForm(RMC_AnalysisRawMaterial entity, string KeyValue, string OrderNumbering)
        {
            try
            {
                int IsOk = 0;
                string Message = OrderNumbering == "" ? "编辑成功。" : "新增成功。";
                if (!string.IsNullOrEmpty(OrderNumbering))
                {

                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_AnalysisRawMaterial AnalysisRawMaterial = AnalysisRawMaterialCurrent.Find(t => t.AnalysisRawMaterialId == key_value).SingleOrDefault();
                    RMC_AnalysisRawMaterial Oldentity = new RMC_AnalysisRawMaterial();
                    Oldentity.TreeId = AnalysisRawMaterial.TreeId;
                    Oldentity.ParentId = key_value;
                    Oldentity.OrderId = 0;
                    Oldentity.RawMaterialId = entity.RawMaterialId;
                    var RawMaterial = RawMaterialCurrent.Find(f => f.RawMaterialId == entity.RawMaterialId).SingleOrDefault();
                    Oldentity.UnitId = RawMaterial.UnitId;
                    Oldentity.MaterialClassId = entity.MaterialClassId;
                    Oldentity.Number = entity.Number;
                    Oldentity.MaterialName = entity.MaterialName;
                    Oldentity.Description = entity.Description;
                    AnalysisRawMaterialCurrent.Add(Oldentity);
                    IsOk = 1;
                    //this.WriteLog(IsOk, entity, null, KeyValue, Message);
                }
                else
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_AnalysisRawMaterial Oldentity = AnalysisRawMaterialCurrent.Find(t => t.AnalysisRawMaterialId == key_value).SingleOrDefault();//获取没更新之前实体对象

                    Oldentity.Number = entity.Number;
                    Oldentity.RawMaterialId = entity.RawMaterialId;
                    Oldentity.Description = entity.Description;
                    AnalysisRawMaterialCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                    //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
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
        public ActionResult DeleteAnalysisRawMaterial(string KeyValue)
        {
            try
            {
                List<int> ids = new List<int>();
                int key_value = Convert.ToInt32(KeyValue);
                ids.Add(key_value);
                AnalysisRawMaterialCurrent.Remove(ids);
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


        public ActionResult GetRawMaterialName()
        {
            List<SelectListItem> List = new List<SelectListItem>();
            List<RMC_RawMaterialLibrary> ProjectList = RawMaterialCurrent.Find(f => f.RawMaterialId > 0).ToList();
            foreach (var Item in ProjectList)
            {
                SelectListItem item = new SelectListItem();
                item.Text = Item.RawMaterialName;
                item.Value = Item.RawMaterialId.ToString();
                List.Add(item);
            }
            return Json(List, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region 原材料采购
        public ActionResult RawMaterialsPurchaseIndex()
        {
            return View();
        }

        public ActionResult ItemList()
        {
            return View();
        }

        /// <summary>
        /// 领用单表单
        /// </summary>
        /// <returns></returns>
        public ActionResult RawMaterialsPurchaseForm(string KeyValue)
        {
            if (KeyValue == "" || KeyValue == null)
            {
                ViewBag.PurchaseNumbering = "CG" + DateTime.Now.ToString("yyyyMMddhhmmssffff");
                ViewData["Librarian"] = currentUser.RealName;
            }
            return View();
        }

        public ActionResult RawMaterialsPurchaseDetailForm()
        {
            return View();
        }

        #endregion

        #region 构件厂原材料库存管理

        public ActionResult TreeJsonRawMaterials(string ItemId)
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
        public ActionResult GridListJsonRawMaterials(FileViewModel model, string TreeId, JqGridParam jqgridparam, string IsPublic, string ParameterJson)
        {
            try
            {
                #region 查询条件拼接
                if (ParameterJson != null)
                {
                    if (ParameterJson != "[{\"RawMaterialName\":\"\",\"InBeginTime\":\"\",\"InEndTime\":\"\"}]")
                    {
                        List<FileViewModel> query_member = JsonHelper.JonsToList<FileViewModel>(ParameterJson);
                        for (int i = 0; i < query_member.Count(); i++)
                        {
                            model.RawMaterialName = query_member[i].RawMaterialName;
                            model.InBeginTime = query_member[i].InBeginTime;
                            model.InEndTime = query_member[i].InEndTime;
                        }
                    }
                }

                Expression<Func<RMC_RawMaterialLibrary, bool>> func = ExpressionExtensions.True<RMC_RawMaterialLibrary>();
                Func<RMC_RawMaterialLibrary, bool> func1 = f => f.TreeId != 0;

                var _a = model.RawMaterialName != null && model.RawMaterialName.ToString() != "";
                var _b = model.InBeginTime != null && model.InBeginTime.ToString() != "0001/1/1 0:00:00";
                var _c = model.InEndTime != null && model.InEndTime.ToString() != "0001/1/1 0:00:00";

                if (_a && _b && _c)
                {
                    func1 = f => f.RawMaterialName.Contains(model.RawMaterialName) && f.WarehousingTime >= model.InBeginTime && f.WarehousingTime <= model.InEndTime;
                }
                else if (_a)
                {
                    func = func.And(f => f.RawMaterialName.Contains(model.RawMaterialName));
                    func1 = f => f.RawMaterialName.Contains(model.RawMaterialName);
                }
                else if (_b)
                {
                    func = func.And(f => f.WarehousingTime >= model.InBeginTime);
                    func1 = f => f.WarehousingTime >= model.InBeginTime;
                }
                else if (_c)
                {
                    func = func.And(f => f.WarehousingTime <= model.InEndTime);
                    func1 = f => f.WarehousingTime <= model.InEndTime;
                }
                else if (_a && _b)
                {
                    func1 = f => f.RawMaterialName.Contains(model.RawMaterialName) && f.WarehousingTime >= model.InBeginTime;
                }
                else if (_a && _c)
                {
                    func1 = f => f.RawMaterialName.Contains(model.RawMaterialName) && f.WarehousingTime <= model.InEndTime;
                }
                else if (_b && _c)
                {
                    func1 = f => f.WarehousingTime >= model.InBeginTime && f.WarehousingTime <= model.InEndTime;
                }
                #endregion

                var MemberList_ = new List<RMC_RawMaterialLibrary>();
                var RawMaterialLibraryModellist = new List<RawMaterialLibraryModel>();
                Stopwatch watch = CommonHelper.TimerStart();
                int total = 0;
                List<RMC_RawMaterialLibrary> MemberList = new List<RMC_RawMaterialLibrary>();
                if (TreeId == "")
                {
                    func.And(f => f.DeleteFlag != 1 & f.RawMaterialId> 0);
                    MemberList = MemberList_ = RawMaterialCurrent.FindPage<string>(jqgridparam.page
                                             , jqgridparam.rows
                                             , func
                                             , false
                                             , f => f.WarehousingTime.ToString()
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
                        var _MemberList = RawMaterialCurrent.Find(m => m.TreeId == item.TreeID).ToList();
                        if (_MemberList.Count() > 0)
                        {
                            MemberList = MemberList.Concat(_MemberList).ToList();
                        }
                    }

                    MemberList = MemberList.Where(func1).ToList();
                    MemberList_ = MemberList.Take(jqgridparam.rows * jqgridparam.page).Skip(jqgridparam.rows * (jqgridparam.page - 1)).ToList();
                    total = MemberList.Count();
                }

                foreach (var item in MemberList_)
                {
                    RawMaterialLibraryModel RawMaterialLibraryModel = new RawMaterialLibraryModel();
                    var Unit = MemberUnitCurrent.Find(f => f.UnitId == item.UnitId).SingleOrDefault();
                    RawMaterialLibraryModel.UnitName = Unit.UnitName;
                    RawMaterialLibraryModel.RawMaterialId = item.RawMaterialId;
                    RawMaterialLibraryModel.RawMaterialName = item.RawMaterialName;
                    RawMaterialLibraryModel.RawMaterialNumber = item.RawMaterialNumber;
                    RawMaterialLibraryModel.RawMaterialStandard = item.RawMaterialStandard;
                    RawMaterialLibraryModel.UnitPrice = item.UnitPrice;
                    RawMaterialLibraryModel.TreeId = item.TreeId;
                    RawMaterialLibraryModel.Description = item.Description;
                    RawMaterialLibraryModellist.Add(RawMaterialLibraryModel);
                }

                var JsonData = new
                {
                    total = total / jqgridparam.rows + 1,
                    page = jqgridparam.page,
                    records = total,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = RawMaterialLibraryModellist.OrderByDescending(f => f.WarehousingTime),
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
        public virtual ActionResult SubmitRawMaterialsForm(RMC_RawMaterialLibrary entity, string KeyValue, string TreeId)
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
                    Oldentity.UnitId = entity.UnitId;
                    Oldentity.UnitPrice = entity.UnitPrice;
                    Oldentity.Description = entity.Description;
                    RawMaterialCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                    //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {

                    RMC_RawMaterialLibrary Oldentity = new RMC_RawMaterialLibrary();
                    Oldentity.TreeId = Convert.ToInt32(TreeId);
                    Oldentity.RawMaterialName = entity.RawMaterialName;
                    Oldentity.RawMaterialNumber = entity.RawMaterialNumber;
                    Oldentity.RawMaterialStandard = entity.RawMaterialStandard;
                    Oldentity.UnitId = entity.UnitId;
                    Oldentity.UnitPrice = entity.UnitPrice;
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
        public ActionResult DeleteRawMaterial(string KeyValue)
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

        #region 构件厂生产制程管理
        public ActionResult ProcessIndex()
        {
            return View();
        }




        public ActionResult NumberForm()
        {
            return View();
        }

        public ActionResult GridListJsonProcess(/*ProjectInfoViewModel model,*/ string TreeId, JqGridParam jqgridparam, string IsPublic)
        {
            try
            {
                Expression<Func<RMC_OrderMember, bool>> func = ExpressionExtensions.True<RMC_OrderMember>();
                //Func<RMC_MemberLibrary, bool> func1 = f => f.TreeID != 0;
                var MemberList_ = new List<RMC_OrderMember>();
                var ProduceOrderModelList = new List<ProduceOrderModel>();
                Stopwatch watch = CommonHelper.TimerStart();
                int total = 0;
                List<RMC_OrderMember> MemberList = new List<RMC_OrderMember>();
                if (TreeId == "")
                {
                    func.And(f=>f.OrderMemberId > 0);
                    MemberList = MemberList_ = OrderMemberCurrent.FindPage<string>(jqgridparam.page
                                             , jqgridparam.rows
                                             , func
                                             , false
                                             , f => f.OrderMemberId.ToString()
                                             , out total
                                             ).ToList();
                }
                else
                {
                    int _id = Convert.ToInt32(TreeId);
                    var list = GetSonId(_id).ToList();
                    try
                    {
                        list.Add(TreeCurrent.Find(p => p.TreeID == _id).Single());
                    }
                    catch {
                        var _MemberList = OrderMemberCurrent.Find(f => f.OrderId == _id).ToList();
                        if (_MemberList.Count() > 0)
                        {
                            MemberList = MemberList.Concat(_MemberList).ToList();
                        }
                    }

                    foreach (var item in list)
                    {
                        var Ordrer = OrderManagementCurrent.Find(f=>f.TreeId==item.TreeID).ToList();
                        if (Ordrer.Count() > 0) { 
                        foreach (var item1 in Ordrer)
                        {
                            var _MemberList = OrderMemberCurrent.Find(m => m.OrderId == item1.OrderId).ToList();
                            if (_MemberList.Count() > 0)
                            {
                                MemberList = MemberList.Concat(_MemberList).ToList();
                            }
                        }
                        }
                    }

                    //MemberList = MemberList.Where(func1).ToList();
                    MemberList_ = MemberList.Take(jqgridparam.rows * jqgridparam.page).Skip(jqgridparam.rows * (jqgridparam.page - 1)).ToList();
                    total = MemberList.Count();
                }


                foreach (var item in MemberList_)
                {
                    ProduceOrderModel _ProduceOrderModel = new ProduceOrderModel();
                    _ProduceOrderModel.OrderId = Convert.ToInt32(item.OrderId);
                    _ProduceOrderModel.MemberNumbering = item.MemberNumbering;
                    _ProduceOrderModel.MemberId = item.MemberId;
                    _ProduceOrderModel.MemberName = item.MemberName;
                    _ProduceOrderModel.MemberNumber = item.Qty;
                    var ProjectDemand = ProjectManagementCurrent.Find(f => f.ProjectDemandId == item.ProjectDemandId).SingleOrDefault();
                    _ProduceOrderModel.ProductionNumber = ProjectDemand.ProductionNumber;
                    var Order = OrderManagementCurrent.Find(f => f.OrderId == item.OrderId).SingleOrDefault();
                    _ProduceOrderModel.Description = item.Description;
                    ProduceOrderModelList.Add(_ProduceOrderModel);
                }

                var JsonData = new
                {
                    total = total / jqgridparam.rows + 1,
                    page = jqgridparam.page,
                    records = total,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ProduceOrderModelList.OrderByDescending(f => f.CreateTime),
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        public ActionResult GetProcessEntryList(string MemberId, string OrderId, JqGridParam jqgridparam)
        {
            int _MemberId = Convert.ToInt32(MemberId);
            int _OrderId = Convert.ToInt32(OrderId);
            try
            {
                var OrderMember = OrderMemberCurrent.Find(f => f.OrderId == _OrderId && f.MemberId == _MemberId).SingleOrDefault();

                List<ProcessManagementModel> ProcessManagementModelList = new List<ProcessManagementModel>();
                var Proceess = ProcessManagementCurrent.Find(f => f.OrderId == _OrderId && f.MemberId == _MemberId).ToList();
                for (int i = 0; i < Proceess.Count(); i++)
                {
                    int _MemberProcessId = Convert.ToInt32(Proceess[i].MemberProcessId);
                    ProcessManagementModel ProcessManagement = new ProcessManagementModel();
                    var _MemberProcess = MemberProcessCurrent.Find(f => f.MemberProcessId == _MemberProcessId).SingleOrDefault();
                    ProcessManagement.ProcessId = Proceess[i].ProcessId;
                    ProcessManagement.ProcessName = _MemberProcess.ProcessName;
                    ProcessManagement.ProcessRequirements = _MemberProcess.ProcessRequirements;
                    ProcessManagement.SortCode = _MemberProcess.SortCode;
                    ProcessManagement.ProcessMan = _MemberProcess.ProcessMan;
                    ProcessManagement.ProcessManImge = _MemberProcess.ProcessManImge;
                    ProcessManagement.ProcessNumbered = Proceess[i].ProcessNumbered;
                    ProcessManagement.UnqualifiedNumber = Proceess[i].UnqualifiedNumber;
                    ProcessManagement.Description = Proceess[i].Description;
                    ProcessManagement.IsProcessTask = Proceess[i].IsProcessTask;
                    ProcessManagement.IsProcessStatus = Proceess[i].IsProcessStatus;
                    ProcessManagementModelList.Add(ProcessManagement);
                }


                var JsonData = new
                {
                    rows = ProcessManagementModelList.OrderBy(f => f.SortCode),
                };
                return Content(JsonData.ToJson());
            }
            catch (Exception ex)
            {
                Base_SysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
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
                int _KeyValue = Convert.ToInt32(KeyValue);
                entity = ProcessManagementCurrent.Find(f => f.ProcessId == _KeyValue).SingleOrDefault();
                var MemberProcess = MemberProcessCurrent.Find(f => f.MemberProcessId == entity.MemberProcessId).SingleOrDefault();
                entity.ProcessName = MemberProcess.ProcessName;
            }
            return Content(entity.ToJson());
            //return Json(entity);
        }

        public ActionResult ReceiveProcessed(string KeyValue)
        {
            try
            {
                int IsOk = 0;
                //string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
                string Message = "操作成功";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = ProcessManagementCurrent.Find(t => t.ProcessId == key_value).SingleOrDefault();//获取没更新之前实体对象
                    Oldentity.IsProcessTask = 1;
                    ProcessManagementCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                    //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
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
                    Oldentity.ProcessMan = entity.ProcessMan;
                    Oldentity.Description = entity.Description;
                    ProcessManagementCurrent.Modified(Oldentity);
                    IsOk = 1;//更新实体对象
                    //this.WriteLog(IsOk, entity, Oldentity, KeyValue, Message);
                }
                else
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = new RMC_ProcessManagement();
                    Oldentity.ProduceStartDate = entity.ProduceStartDate;
                    Oldentity.ProduceEndDate = entity.ProduceStartDate;
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

        /// <summary>
        /// 制程状态控制
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        public ActionResult Processed(RMC_ProcessManagement Process, string KeyValue, string MemberId, string OrderId, string IsQqualified)
        {
            try
            {
                int IsOk = 0;
                string Message = KeyValue == "2" ? "质检通过" : "操作成功。";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    int key_value = Convert.ToInt32(KeyValue);
                    RMC_ProcessManagement Oldentity = ProcessManagementCurrent.Find(t => t.ProcessId == key_value).SingleOrDefault();//获取没更新之前实体对象赋值
                    if (Oldentity.IsProcessStatus == 1 || Oldentity.IsProcessStatus == 2)
                    {
                        Message = "该工序已提交过！";
                    }
                    else
                    {
                        int _MemberId = Convert.ToInt32(MemberId);
                        int _OrderId = Convert.ToInt32(OrderId);

                        Oldentity.UnqualifiedNumber = Convert.ToInt32(Oldentity.UnqualifiedNumber) + Convert.ToInt32(Process.UnqualifiedNumber);
                        Oldentity.ProcessNumbered = Convert.ToInt32(Oldentity.ProcessNumbered) + Convert.ToInt32(Process.ProcessNumbered);
                        var OrderMember = OrderMemberCurrent.Find(f => f.OrderId == _OrderId && f.MemberId == _MemberId).SingleOrDefault();
                        if (OrderMember.Qty <= Oldentity.ProcessNumbered)
                        {
                            Oldentity.IsProcessStatus = Convert.ToInt32(IsQqualified);
                            Oldentity.ProcessNumbered = OrderMember.Qty;
                        }
                        ProcessManagementCurrent.Modified(Oldentity);//修改完成工艺量
                        IsOk = 1;



                        if (_MemberId == 0)
                        {
                            _MemberId = Convert.ToInt32(Oldentity.MemberId);
                            _OrderId = Convert.ToInt32(Oldentity.OrderId);
                        }

                        List<RMC_ProcessManagement> ProcessManagementList = ProcessManagementCurrent.Find(f => f.MemberId == _MemberId && f.OrderId == _OrderId).ToList();
                        var Order = OrderManagementCurrent.Find(f => f.OrderId == _OrderId).SingleOrDefault();
                        int a = 0;
                        for (int i = 0; i < ProcessManagementList.Count(); i++)
                        {
                            if (ProcessManagementList[i].IsProcessStatus == 1)
                            {
                                a++;
                            }
                        }
                        if (a == ProcessManagementList.Count())
                        {
                            Operating(_MemberId, _OrderId);
                            Order.Productioned = 1;
                        }
                        else if (a == 0)
                        {
                            Order.Productioned = 0;
                        }
                        else
                        {
                            Order.Productioned = 2;
                        }
                        OrderManagementCurrent.Modified(Order);
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
        /// 生产量，工厂库存，修改发货构件显示
        /// </summary>
        public void Operating(int _MemberId, int _OrderId)
        {
            var OrderMember = OrderMemberCurrent.Find(f => f.MemberId == _MemberId && f.OrderId == _OrderId).SingleOrDefault();
            var Member = MemberLibraryCurrent.Find(f => f.MemberID == _MemberId).SingleOrDefault();
            RMC_ProjectDemand _MemberDemend = new RMC_ProjectDemand();
            _MemberDemend = ProjectManagementCurrent.Find(f => f.MemberId == _MemberId).SingleOrDefault();
            _MemberDemend.ProductionNumber = Convert.ToInt32(_MemberDemend.ProductionNumber) + Convert.ToInt32(OrderMember.Qty);
            ProjectManagementCurrent.Modified(_MemberDemend);

            RMC_FactoryWarehouse FactoryWarehouse = new RMC_FactoryWarehouse();
            List<RMC_FactoryWarehouse> FactoryWarehouseList = FactoryWarehouseCurrent.Find(f => f.MemberId == Member.MemberID).ToList();
            if (FactoryWarehouseList.Count() == 0)
            {
                FactoryWarehouse.MemberId = Member.MemberID;
                FactoryWarehouse.MemberModel = Member.MemberModel;
                FactoryWarehouse.InStockNumber = OrderMember.Qty;
                FactoryWarehouseCurrent.Add(FactoryWarehouse);
            }
            else
            {
                var _FactoryWarehouse = FactoryWarehouseCurrent.Find(f => f.MemberId == Member.MemberID).SingleOrDefault();
                _FactoryWarehouse.InStockNumber = Convert.ToInt32(_FactoryWarehouse.InStockNumber) + Convert.ToInt32(OrderMember.Qty);
                FactoryWarehouseCurrent.Modified(_FactoryWarehouse);
            }


            RMC_ShipManagement ShipManagement = ShipManagementCurrent.Find(f => f.MemberId == _MemberDemend.MemberId).SingleOrDefault();
            if (ShipManagement.IsPackaged == 1)
            {
            }
            else
            {
                ShipManagement.IsPackaged = 1;
                ShipManagement.ShipNumber = OrderMember.Qty.ToString();
                ShipManagementCurrent.Modified(ShipManagement);
            }

        }

        #endregion

        #region 二维码解析，生成，打印

        public ActionResult IsPrint(string KeyValue, string OrderId)
        {
            int _KeyValue = Convert.ToInt32(KeyValue);
            int _OrderId = Convert.ToInt32(OrderId);
            string Message;
            List<RMC_ProcessManagement> ProcessManagementList = ProcessManagementCurrent.Find(f => f.MemberId == _KeyValue && f.OrderId == _OrderId).ToList();
            int a = 0;
            for (int i = 0; i < ProcessManagementList.Count(); i++)
            {
                if (ProcessManagementList[i].IsProcessStatus == 1)
                {
                    a++;
                }
            }
            if (a == ProcessManagementList.Count())
            {
                Message = "1";
            }
            else
            {
                Message = "存在未做制程或存在不合格构件！";
            }

            return Content(Message);
        }




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
            try
            {
                int _KeyValue = Convert.ToInt32(KeyValue);
                int _OrderId = Convert.ToInt32(OrderId);
                var Member = MemberLibraryCurrent.Find(f => f.MemberID == _KeyValue).SingleOrDefault();
                var MemberDemend = ProjectManagementCurrent.Find(f => f.MemberId == Member.MemberID).SingleOrDefault();
                //var Project = ProjectInfoCurrent.Find(f => f.ProjectId == MemberDemend.ProjectId).SingleOrDefault();
                EntityModel.MemberName = Member.MemberName;
                EntityModel.MemberNumbering = Member.MemberNumbering;
                EntityModel.MemberModel = Member.MemberModel;
                EntityModel.TheoreticalWeight = Member.TheoreticalWeight;
                // EntityModel.ProjectName = Project.ProjectName;
                return Content(EntityModel.ToJson());
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
            string virtualPath = this.Server.MapPath("~") + "Resource/Document/NetworkDisk/QRCode";
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

        /// <summary>
        /// 发货树
        /// </summary>
        /// <param name="ItemId"></param>
        /// <returns></returns>
        public ActionResult TreeJsonShip(string ItemId)
        {
            int _ItemId = Convert.ToInt32(ItemId);
            List<RMC_Tree> list, list1, list2;
            list1 = TreeCurrent.Find(t => t.DeleteFlag != 1 && t.ItemClass == 0).ToList();
            list2 = TreeCurrent.Find(t => t.DeleteFlag != 1 && t.ItemID == _ItemId && t.ItemClass == 4).ToList();
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
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="TreeID"></param>
        /// <param name="jqgridparam"></param>
        /// <param name="IsPublic"></param>
        /// <param name="ParameterJson"></param>
        /// <returns></returns>
        public ActionResult GridListJsonShipManagement(FileViewModel model, string TreeID, JqGridParam jqgridparam, string IsPublic, string ParameterJson)
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
                Expression<Func<RMC_ShipManagement, bool>> func = ExpressionExtensions.True<RMC_ShipManagement>();
                func = f => f.ShipId > 0 && f.IsPackaged == 1;
                #region 查询条件拼接

                if (model.OrderNumbering != null && model.OrderNumbering.ToString() != "")
                {
                    func = func.And(f => f.MemberModel.Contains(model.MemberModel));

                }
                if (model.InBeginTime != null && model.InBeginTime.ToString() != "0001/1/1 0:00:00")
                {
                    func = func.And(f => f.ShipDate >= model.InBeginTime);

                }
                if (model.InEndTime != null && model.InEndTime.ToString() != "0001/1/1 0:00:00")
                {
                    func = func.And(f => f.ShipDate <= model.InEndTime);
                }
                #endregion

                DataTable ListData, ListData1;
                ListData = null;
                //List<RMC_Tree> listtree = TreeCurrent.FindPage<string>(jqgridparam.page
                //                         , jqgridparam.rows
                //                         , func1
                //                         , false
                //                         , f => f.TreeID.ToString()
                //                         , out total
                //                         ).ToList();
                List<RMC_ShipManagement> listfile = ShipManagementCurrent.FindPage<string>(jqgridparam.page
                                         , jqgridparam.rows
                                         , func
                                         , false
                                         , f => f.ShipId.ToString()
                                         , out total
                                         ).ToList();
                //List<ProcessManagementModel> processmanagementlist = new List<ProcessManagementModel>();
                foreach (var item in listfile)
                {
                    if (!string.IsNullOrEmpty(item.MemberId.ToString()))
                    {
                        var member = MemberLibraryCurrent.Find(f => f.MemberID == item.MemberId).SingleOrDefault();
                        var FactoryWarehouse = FactoryWarehouseCurrent.Find(f => f.MemberId == item.MemberId).SingleOrDefault();
                        item.MemberName = member.MemberName;
                        item.MemberModel = member.MemberModel;
                        item.MemberNumbering = member.MemberNumbering;
                        item.LibraryNumber = FactoryWarehouse.InStockNumber;
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
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
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
                var FactoryWarehouse = FactoryWarehouseCurrent.Find(f => f.MemberId == entity.MemberId).SingleOrDefault();
                entity.LibraryNumber = FactoryWarehouse.InStockNumber;
                entity.MemberModel = FactoryWarehouse.MemberModel;
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
                    Oldentity.ShipMan = currentUser.RealName;
                    //Oldentity.ShipDate =Convert.ToDateTime(DateTime.Now.ToString("yyyy-mm-dd hh:MM:ss"));
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
                    //int TreeID = Convert.ToInt32(TreeId);
                    //RMC_ShipManagement Oldentity = new RMC_ShipManagement();
                    //Oldentity.TreeId = TreeID;
                    //Oldentity.MemberId = entity.MemberId;
                    //Oldentity.Description = entity.Description;
                    //Oldentity.LogisticsStatus = "0";
                    //Oldentity.ShipMan = entity.ShipMan;
                    //Oldentity.MemberClassId = entity.MemberClassId;
                    //Oldentity.MemberId = entity.MemberId;
                    //Oldentity.ShipNumber = "0";
                    //Oldentity.ShippingAddress = entity.ShippingAddress;
                    //Oldentity.ShippingMan = entity.ShippingMan;
                    //Oldentity.ShippingTEL = entity.ShippingTEL;
                    //ShipManagementCurrent.Add(Oldentity);
                    //IsOk = 1;
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

                    RMC_FactoryWarehouse FactoryWarehouse = FactoryWarehouseCurrent.Find(f => f.MemberId == Oldentity.MemberId).SingleOrDefault();
                    FactoryWarehouse.InStockNumber = Convert.ToInt32(FactoryWarehouse.InStockNumber) - Convert.ToInt32(Oldentity.ShipNumber);
                    FactoryWarehouseCurrent.Modified(FactoryWarehouse);


                    RMC_ProjectWarehouse OldEntity = ProjectWarehouseCurrent.Find(f => f.ProjectWarehouseId == ProjectWarehouse.ProjectWarehouseId).SingleOrDefault();
                    if (ProjectWarehouse.InStock != null || ProjectWarehouse.InStock.ToString() != "")
                    {
                        ProjectWarehouse.InStock = ProjectWarehouse.InStock + shipnumber;
                    }
                    else
                    {
                        ProjectWarehouse.InStock = 0;
                        ProjectWarehouse.InStock = ProjectWarehouse.InStock + shipnumber;
                    }
                    ProjectWarehouse.ModifyTime = DateTime.Now;
                    ProjectWarehouse.IsShiped = 1;
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