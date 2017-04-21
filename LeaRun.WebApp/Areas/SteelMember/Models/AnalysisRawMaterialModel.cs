using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaRun.WebApp.Areas.SteelMember.Models
{
    public class AnalysisRawMaterialModel
    {
        public int AnalysisRawMaterialId { get; set; }
        public Nullable<int> TreeId { get; set; }
        public string OrderNumbering { get; set; }
        public string MaterialName { get; set; }
        public string MaterialStandard { get; set; }
        public Nullable<int> Number { get; set; }
        public Nullable<int> UnitId { get; set; }
        public string UnitName { get; set; }
        public Nullable<int> UnitPrice { get; set; }
        public string MaterialBudget { get; set; }
        public string AnalysisMan { get; set; }
        public string Description { get; set; }
    }
}