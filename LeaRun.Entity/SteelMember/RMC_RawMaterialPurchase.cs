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
    
    public partial class RMC_RawMaterialPurchase
    {
        public int RawMaterialPurchaseId { get; set; }
        public Nullable<int> TreeId { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> RawMaterialId { get; set; }
        public Nullable<int> PurchaseId { get; set; }
        public Nullable<int> MaterialClassId { get; set; }
        public string OrderNumbering { get; set; }
        public string RawMaterialName { get; set; }
        public string RawMaterialStandard { get; set; }
        public Nullable<int> Qty { get; set; }
        public string UnitName { get; set; }
        public Nullable<int> UnitPrice { get; set; }
        public string MaterialBudget { get; set; }
        public Nullable<int> DeleteFlag { get; set; }
        public string Description { get; set; }
    }
}
