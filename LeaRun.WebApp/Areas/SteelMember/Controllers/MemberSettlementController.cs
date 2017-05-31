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
    public class MemberSettlementController : BaseController
    {
        //
        // GET: /SteelMember/MemberSettlement/
        [Inject]
        public FileIBLL MemberLibraryCurrent { get; set; }
        [Inject]
        public CollarMemberIBLL CollarMemberCurrent { get; set; }

        [Inject]
        public MemberMaterialIBLL MemberMaterialCurrent { get; set; }
        [Inject]
        public RawMaterialIBLL RawMaterialCurrent { get; set; }

        [Inject]
        public ProjectManagementIBLL ProjectManagementCurrent { get; set; }

        [Inject]
        public CollarIBLL CollarManagementCurrent { get; set; }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult QueryPage()
        {
            return View();
        }
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
                //func = f => f.DeleteFlag != 1 &&f.IsSubmit==1;
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
                    var CollarMember = CollarMemberCurrent.Find(f => f.CollarId == item.CollarId).ToList();
                    int Number =0;
                    int UnitPrice = 0;
                    int CostBudget = 0;
                    int MemberId = 0;
                    int ProjectDemandId = 0;
                    var CollarNumbering = "";
                    for (int i = 0; i < CollarMember.Count(); i++)
                    {
                        CollarNumbering = CollarMember[i].CollarNumbering;
                        MemberId = Convert.ToInt32(CollarMember[i].MemberId);
                        ProjectDemandId = Convert.ToInt32(CollarMember[i].ProjectDemandId);

                        var MemberMaterial = MemberMaterialCurrent.Find(f => f.MemberId == MemberId).ToList();
                        for (int i0 = 0; i0 < MemberMaterial.Count(); i0++)
                        {
                            //Numbers = MemberMaterial[i0].MaterialNumber;
                            int RawMaterialId = Convert.ToInt32(MemberMaterial[i0].RawMaterialId);
                            var Material = RawMaterialCurrent.Find(f => f.RawMaterialId == RawMaterialId).SingleOrDefault();
                            CostBudget += Convert.ToInt32(MemberMaterial[i0].MaterialNumber) * Convert.ToInt32(Material.UnitPrice) * Convert.ToInt32(CollarMember[i].Qty);
                            UnitPrice += Convert.ToInt32(MemberMaterial[i0].MaterialNumber) * Convert.ToInt32(Material.UnitPrice);
                        }
                        Number += Convert.ToInt32(CollarMember[i].Qty);
                    }
                    var Member = MemberLibraryCurrent.Find(f => f.MemberID == MemberId).SingleOrDefault();
                    var ProjectDemand = ProjectManagementCurrent.Find(f => f.ProjectDemandId == ProjectDemandId).SingleOrDefault();
                    projectdemand.CollarId = item.CollarId;
                    projectdemand.CollarNumbering = CollarNumbering;
                    projectdemand.MemberName = Member.MemberName;
                    projectdemand.MemberModel = Member.MemberModel;
                    projectdemand.UnitPrice = UnitPrice.ToString();
                    projectdemand.LeaderNumber = Number;
                    projectdemand.CostBudget = CostBudget.ToString();
                    projectdemand.LeaderTime = item.CollarTime;
                    projectdemand.CreateMan = ProjectDemand.CreateMan;
                    projectdemand.ReviewMan = currentUser.RealName;
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


    }
}
