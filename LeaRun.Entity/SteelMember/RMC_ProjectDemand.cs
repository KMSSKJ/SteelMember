//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace LeaRun.Entity.SteelMember
{
    using System;
    using System.Collections.Generic;
    
    public partial class RMC_ProjectDemand
    {
        public int ProjectDemandId { get; set; }
        public Nullable<int> TreeId { get; set; }
        public Nullable<int> ProjectId { get; set; }
        public Nullable<int> MemberId { get; set; }
        public Nullable<int> MemberCompanyId { get; set; }
        public Nullable<int> MemberClassId { get; set; }
        public Nullable<int> UnitId { get; set; }
        public string MemberWeight { get; set; }
        public string UnitPrice { get; set; }
        public Nullable<decimal> CostBudget { get; set; }
        public Nullable<int> IsReview { get; set; }
        public string ReviewMan { get; set; }
        public Nullable<int> IsDemandSubmit { get; set; }
        public Nullable<int> IsSubmit { get; set; }
        public string Description { get; set; }
        public Nullable<int> DeleteFlag { get; set; }
        public Nullable<int> MemberNumber { get; set; }
        public Nullable<int> OrderQuantityed { get; set; }
        public Nullable<int> Productioned { get; set; }
        public Nullable<System.DateTime> DeleteTime { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
    }
}
