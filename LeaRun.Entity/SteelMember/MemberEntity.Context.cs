﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class kskfEntities : DbContext
    {
        public kskfEntities()
            : base("name=kskfEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<RMC_AnalysisRawMaterial> RMC_AnalysisRawMaterial { get; set; }
        public virtual DbSet<RMC_Company> RMC_Company { get; set; }
        public virtual DbSet<RMC_FactoryWarehouse> RMC_FactoryWarehouse { get; set; }
        public virtual DbSet<RMC_MemberMaterial> RMC_MemberMaterial { get; set; }
        public virtual DbSet<RMC_MemberUnit> RMC_MemberUnit { get; set; }
        public virtual DbSet<RMC_OrderMember> RMC_OrderMember { get; set; }
        public virtual DbSet<RMC_ProjectInfo> RMC_ProjectInfo { get; set; }
        public virtual DbSet<RMC_RawMaterialLibrary> RMC_RawMaterialLibrary { get; set; }
        public virtual DbSet<RMC_ShipManagement> RMC_ShipManagement { get; set; }
        public virtual DbSet<RMC_Tree> RMC_Tree { get; set; }
        public virtual DbSet<RMC_ProcessManagement> RMC_ProcessManagement { get; set; }
        public virtual DbSet<RMC_MemberProcess> RMC_MemberProcess { get; set; }
        public virtual DbSet<RMC_CollarMember> RMC_CollarMember { get; set; }
        public virtual DbSet<RMC_Collar> RMC_Collar { get; set; }
        public virtual DbSet<RMC_ProjectWarehouse> RMC_ProjectWarehouse { get; set; }
        public virtual DbSet<RMC_ProjectDemand> RMC_ProjectDemand { get; set; }
        public virtual DbSet<RMC_MemberLibrary> RMC_MemberLibrary { get; set; }
        public virtual DbSet<RMC_ProjectOrder> RMC_ProjectOrder { get; set; }
    }
}
