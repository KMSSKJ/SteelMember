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
    
    public partial class RMC_ProcessManagement
    {
        public int ProcessId { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> MemberId { get; set; }
        public Nullable<int> MemberProcessId { get; set; }
        public Nullable<System.DateTime> ProduceStartDate { get; set; }
        public Nullable<System.DateTime> ProduceEndDate { get; set; }
        public string ProcessMan { get; set; }
        public Nullable<int> UnqualifiedNumber { get; set; }
        public Nullable<int> QualifiedNumber { get; set; }
        public string Description { get; set; }
        public Nullable<int> IsProcessStatus { get; set; }
        public string ProcessManImge { get; set; }
        public string ProcessName { get; set; }
        public Nullable<int> ProcessNumbered { get; set; }
    }
}
