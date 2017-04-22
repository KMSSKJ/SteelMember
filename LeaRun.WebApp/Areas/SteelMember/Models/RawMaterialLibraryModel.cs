using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaRun.WebApp.Areas.SteelMember.Models
{
    public class RawMaterialLibraryModel
    {
        public string UnitName { get; set; }
        public Nullable<int> RawMaterialId { get; set; }
        public string RawMaterialName { get; set; }
        public Nullable<int> RawMaterialNumber { get; set; }
        public string RawMaterialStandard { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<int> TreeId { get; set; }
        public string Description { get; set; }
    }
}