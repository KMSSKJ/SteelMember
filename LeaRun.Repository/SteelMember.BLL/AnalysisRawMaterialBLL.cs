
using LeaRun.Entity.SteelMember;
using LeaRun.Repository.SteelMember.IBLL;
using LeaRun.Repository.SteelMember.IDAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LeaRun.Repository.SteelMember.BLL
{
    public class AnalysisRawMaterialBLL : AnalysisRawMaterialIBLL
    {
        //ICompanyDAL CurrentDAL = IoC.Resolve<ICompanyDAL>();

        [Ninject.Inject]
        public AnalysisRawMaterialIDAL CurrentDAL { get; set; }
        public AnalysisRawMaterialBLL()
        {

        }
        public RMC_AnalysisRawMaterial Add(RMC_AnalysisRawMaterial model)
        {
            return CurrentDAL.Add(model);
        }

        public int Remove(List<int> delbyid)
        {
            delbyid.ToList().ForEach(id =>
            {
                CurrentDAL.Remove(Find(p => p.AnalysisRawMaterialId == id).Single());
            }); 

            return CurrentDAL.SaveChange();
        }

        public IEnumerable<RMC_AnalysisRawMaterial> Find(Expression<Func<RMC_AnalysisRawMaterial, bool>> whereLambda)
        {
            return CurrentDAL.Find(whereLambda);
        }
 
        public IEnumerable<RMC_AnalysisRawMaterial> FindPage<S>(int pageIndex, int pageSize, Expression<Func<RMC_AnalysisRawMaterial, bool>> whereLambda, bool isAsc, Expression<Func<RMC_AnalysisRawMaterial, S>> orderbyLambda, out int total)
        {
            return CurrentDAL.FindPaging(pageIndex, pageSize, whereLambda, isAsc, orderbyLambda, out total);
        }

        public IEnumerable<RMC_AnalysisRawMaterial> SelectPaging<S>(int pageIndex, int pageSize, Expression<Func<RMC_AnalysisRawMaterial, bool>> whereLambda, Expression<Func<RMC_AnalysisRawMaterial, bool>> whereLambda1, bool isAsc, Expression<Func<RMC_AnalysisRawMaterial, S>> orderbyLambda, out int total)
        {
            return CurrentDAL.FindPaging(pageIndex, pageSize, whereLambda, isAsc, orderbyLambda, out total);
        }

        public List<RMC_AnalysisRawMaterial> SaveList(Stream file, RMC_AnalysisRawMaterial model)
        {
            throw new NotImplementedException();
        }

        public int Modified(RMC_AnalysisRawMaterial model)
        {
            return CurrentDAL.Modified(model);
        }

        public string Remove_str(List<string> delbyid)
        {
            throw new NotImplementedException();
        }
    }
}

