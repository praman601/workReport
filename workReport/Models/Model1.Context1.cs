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
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
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
        public virtual DbSet<workTypes> workTypes { get; set; }
        public virtual DbSet<Months> Months { get; set; }
        public virtual DbSet<issues> issues { get; set; }
        public virtual DbSet<posts> posts { get; set; }
        public virtual DbSet<nepMonths> nepMonths { get; set; }
        public virtual DbSet<COM_ENGLISH_NEPALI_DATE> COM_ENGLISH_NEPALI_DATE { get; set; }
        public virtual DbSet<workList> workList { get; set; }
        public virtual DbSet<user> user { get; set; }
    
        public virtual ObjectResult<Nullable<System.DateTime>> PR_neptoeng(string nepdate)
        {
            var nepdateParameter = nepdate != null ?
                new ObjectParameter("nepdate", nepdate) :
                new ObjectParameter("nepdate", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<System.DateTime>>("PR_neptoeng", nepdateParameter);
        }
    
        public virtual ObjectResult<string> PR_engtonep(string engdate)
        {
            var engdateParameter = engdate != null ?
                new ObjectParameter("engdate", engdate) :
                new ObjectParameter("engdate", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("PR_engtonep", engdateParameter);
        }
    }
}
