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
    
    public partial class RMC_OrderMember
    {
        public int OrderMemberId { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> ProjectDemandId { get; set; }
        public Nullable<int> MemberId { get; set; }
        public string MemberNumbering { get; set; }
        public string MemberName { get; set; }
        public string MemberModel { get; set; }
        public string MemberUnit { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> PriceAmount { get; set; }
        public string Description { get; set; }
    }
}
