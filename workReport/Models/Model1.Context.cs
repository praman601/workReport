﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace workReport.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class workReportEntities : DbContext
    {
        public workReportEntities()
            : base("name=workReportEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<mun> mun { get; set; }
        public virtual DbSet<user> user { get; set; }
        public virtual DbSet<workList> workList { get; set; }
        public virtual DbSet<workTypes> workTypes { get; set; }
    }
}
