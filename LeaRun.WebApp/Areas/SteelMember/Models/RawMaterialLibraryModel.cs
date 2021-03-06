﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaRun.WebApp.Areas.SteelMember.Models
{
    public class RawMaterialLibraryModel
    {
        public int PurchaseId { get; set; }

        public string UnitName { get; set; }
        public string RawMaterialId { get; set; }
        public DateTime? WarehousingTime { get; set; }
        public string RawMaterialName { get; set; }
        public Nullable<int> RawMaterialNumber { get; set; }
        public string RawMaterialStandard { get; set; }
        public string Price { get; set; }
        public string UnitPrice { get; set; }
        public string PriceAmount { get; set; }
        public string Qty { get; set; }
        public Nullable<int> TreeId { get; set; }
        public string Description { get; set; }
    }
}