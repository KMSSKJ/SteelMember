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
    
    public partial class RMC_Tree
    {
        public int TreeID { get; set; }
        public Nullable<int> ParentID { get; set; }
        public Nullable<int> ItemID { get; set; }
        public Nullable<int> IsItem { get; set; }
        public string TreeName { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public Nullable<int> IsMenu { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> DeleteFlag { get; set; }
        public Nullable<int> Enabled { get; set; }
        public Nullable<int> Oderby { get; set; }
        public Nullable<System.DateTime> UploadTime { get; set; }
        public Nullable<System.DateTime> ModifiedTime { get; set; }
        public Nullable<System.DateTime> OverdueTime { get; set; }
        public string Description { get; set; }
        public Nullable<int> IsReview { get; set; }
        public string FileType { get; set; }
    }
}
