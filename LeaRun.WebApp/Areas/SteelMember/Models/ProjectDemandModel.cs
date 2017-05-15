using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SteelMember.Models
{
    public class ProjectDemandModel
    {
        public int ProjectDemandId { get; set; }
        public int OrderId { get; set; }
        public Nullable<int> MemberClassId { get; set; }
        public string MemberClassName{ get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> MemberId { get; set; }
        public Nullable<int> UnitId { get; set; }
        public string MemberUnit { get; set; }
        public string MemberName{ get; set; }
        public string MemberCompany { get; set; }
        public Nullable<int> MemberNumber { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public string MemberModel { get; set; }
        public string MemberNumbering { get; set; }//Nullable<long>
        public string OrderNumbering { get; set; }
        public string MemberWeight { get; set; }
        public string Description { get; set; }
        public string TheoreticalWeight { get; set; }
        public string UnitPrice { get; set; }
        public string CostBudget { get; set; }
        public Nullable<int> IsSubmit { get; set; }
        public Nullable<int> IsReview { get; set; }
        public string ReviewMan { get; set; }
        public Nullable<int> OrderQuantityed { get; set; }
        public Nullable<int> Productioned { get; set; }
        public Nullable<DateTime> CreateTime { get; set; }
        public string CreateMan { get; set; }
        public Nullable<int> IsDemandSubmit { get; set; }
        public string CADDrawing { get; set; }
        public string ModelDrawing { get; set; }
        public int? CollarNumbered { get; set; }
    }
}