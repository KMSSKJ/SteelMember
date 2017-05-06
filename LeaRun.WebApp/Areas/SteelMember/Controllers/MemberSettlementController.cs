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
using System.Web;
using System.Web.Mvc;

namespace LeaRun.WebApp.Areas.SteelMember.Controllers
{
    public class MemberSettlementController : Controller
    {
        //
        // GET: /SteelMember/MemberSettlement/
        [Inject]
        public OrderManagementIBLL OrderManagementCurrent { get; set; }
        [Inject]
        public OrderMemberIBLL OrderMemberCurrent { get; set; }

        [Inject]
        public MemberMaterialIBLL MemberMaterialCurrent { get; set; }
        [Inject]
        public RawMaterialIBLL RawMaterialCurrent { get; set; }
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridListJson(/*ProjectInfoViewModel model,*/ string TreeID, JqGridParam jqgridparam, string IsPublic)
        {
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
                                         , f => f.TreeId.ToString()
                                         , out total
                                         ).ToList();
                List<ProjectDemandModel> projectdemandlist = new List<ProjectDemandModel>();
                foreach (var item in listfile)
                {
                    ProjectDemandModel projectdemand = new ProjectDemandModel();
                    projectdemand.OrderId = item.OrderId;
                    projectdemand.OrderNumbering = item.OrderNumbering;
                    var OrderMember = OrderMemberCurrent.Find(f => f.OrderId == item.OrderId).ToList();
                    int Number=0;
                    for (int i = 0; i < OrderMember.Count(); i++)
                    {
                        int MemberId =Convert.ToInt32(OrderMember[i].MemberId);
                        var MemberMaterial = MemberMaterialCurrent.Find(f => f.MemberId == MemberId).ToList();
                        for (int i0 = 0; i0 < MemberMaterial.Count(); i0++)
                        {
                            int RawMaterialId =Convert.ToInt32(MemberMaterial[i0].RawMaterialId);
                            var Material= RawMaterialCurrent.Find(f => f.RawMaterialId == RawMaterialId).SingleOrDefault();
                            Number += Convert.ToInt32(MemberMaterial[i0].MaterialNumber) * Convert.ToInt32(Material.UnitPrice) * Convert.ToInt32(OrderMember[i].Qty);
                        }
                    }

                    projectdemand.CostBudget = Number.ToString();
                    projectdemand.ReviewMan = "System";
                    projectdemand.CreateTime = item.CreateTime;
                    projectdemand.CreateMan = "System";
                    projectdemand.Productioned = Convert.ToInt32(item.Productioned);
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
                return Content("<script>alertDialog('" + ex.Message + "');</script>");
            }
        }


    }
}
