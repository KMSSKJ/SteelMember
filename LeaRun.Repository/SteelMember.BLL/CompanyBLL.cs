
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
    public class CompanyBLL : CompanyIBLL
    {
        //ICompanyDAL CurrentDAL = IoC.Resolve<ICompanyDAL>();

        [Ninject.Inject]
        public CompanyIDAL CurrentDAL { get; set; }
        public CompanyBLL()
        {

        }
        public RMC_Company Add(RMC_Company model)
        {
            return CurrentDAL.Add(model);
        }

        public int Remove(List<int> delbyid)
        {
            delbyid.ToList().ForEach(id =>
            {
                CurrentDAL.Remove(Find(p => p.MemberCompanyId == id).Single());
            }); 

            return CurrentDAL.SaveChange();
        }

        public IEnumerable<RMC_Company> Find(Expression<Func<RMC_Company, bool>> whereLambda)
        {
            return CurrentDAL.Find(whereLambda);
        }
 
        public IEnumerable<RMC_Company> FindPage<S>(int pageIndex, int pageSize, Expression<Func<RMC_Company, bool>> whereLambda, bool isAsc, Expression<Func<RMC_Company, S>> orderbyLambda, out int total)
        {
            return CurrentDAL.FindPaging(pageIndex, pageSize, whereLambda, isAsc, orderbyLambda, out total);
        }

        public IEnumerable<RMC_Company> SelectPaging<S>(int pageIndex, int pageSize, Expression<Func<RMC_Company, bool>> whereLambda, Expression<Func<RMC_Company, bool>> whereLambda1, bool isAsc, Expression<Func<RMC_Company, S>> orderbyLambda, out int total)
        {
            return CurrentDAL.FindPaging(pageIndex, pageSize, whereLambda, isAsc, orderbyLambda, out total);
        }

        public List<RMC_Company> SaveList(Stream file, RMC_Company model)
        {
            throw new NotImplementedException();
        }

        public int Modified(RMC_Company model)
        {
            return CurrentDAL.Modified(model);
        }

        public string Remove_str(List<string> delbyid)
        {
            throw new NotImplementedException();
        }
    }
}

