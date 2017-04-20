using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SteelMember.Models
{
    public class ProcessManagementModel
    {
        public int ProcessId { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> TreeId { get; set; }
        public Nullable<int> MemberNumber { get; set; }
        public string MemberName { get; set; }
        public string MemberNumbering { get; set; }//Nullable<long>MemberNumbering
        public Nullable<System.DateTime> ProduceStartDate { get; set; }
        public Nullable<System.DateTime> ProduceEndDate { get; set; }
        public Nullable<int> MemberCompanyId { get; set; }
        public string SetUpProcessing { get; set; }
        public Nullable<int> SetUpProcessingDays { get; set; }
        public Nullable<int> IsSetUpProcessingTask { get; set; }
        public Nullable<int> IsSetUpProcessing { get; set; }
        public string SubmergedArcProcessing { get; set; }
        public Nullable<int> SubmergedArcProcessingDays { get; set; }
        public Nullable<int> IsSubmergedArcProcessingTask { get; set; }
        public Nullable<int> IsSubmergedArcProcessing { get; set; }
        public string RivetingProcessing { get; set; }
        public Nullable<int> RivetingProcessingDays { get; set; }
        public Nullable<int> IsRivetingProcessingTask { get; set; }
        public Nullable<int> IsRivetingProcessing { get; set; }
        public string PaintProcessing { get; set; }
        public Nullable<int> PaintProcessingDays { get; set; }
        public Nullable<int> IsPaintProcessingTask { get; set; }
        public Nullable<int> IsPaintProcessing { get; set; }
        public Nullable<int> QualityInspection { get; set; }
        public Nullable<int> Supervision { get; set; }
        public Nullable<int> IsPacking { get; set; }
        public string Packing { get; set; }
        public Nullable<int> QualifiedNumber { get; set; }
        public Nullable<int> UnqualifiedNumber { get; set; }
        public byte[] QRCode { get; set; }
        public string Description { get; set; }
    }
}