using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using workReport.Models;

namespace workReport.Models
{
    public class ContextHelper : workReportEntities
    {

        //public virtual int DeleteHouse(int hId)
        //{
        //    return this.Database.SqlQuery<int>("proc_deleteFloor @id", new SqlParameter("@id", hId)).Count();
        //}



        public virtual string PR_engtonep(string engdate)

        {
            var x= this.Database.SqlQuery<string>("PR_engtonep @engdate", new SqlParameter("@engdate", engdate)).First();
            return x;
        }

        public virtual string PR_neptoeng(string nepdate)

        {
            var x = this.Database.SqlQuery<string>("PR_neptoeng @nepdate", new SqlParameter("@nepdate", nepdate)).First();
            return x;
        }

        ////public virtual List<Flat_Details> deleteFloor(int fId)
        ////{
        ////    //return this.Database.SqlQuery<Flat_Details>("deleteFloor @floorId", new SqlParameter("@floorId", fId)).ToList();
        ////   return this.Database.ExecuteSqlCommand("exec deleteFloor @floorId", new SqlParameter("@floorId", fId));
        ////}
        //public virtual List<showDetails_Result> getAllDetails(int uId)
        //{
        //    return this.Database.SqlQuery<showDetails_Result>("showDetails @userId", new SqlParameter("@userId", uId)).ToList();

        //}

        //public virtual List<getHouse_Result> getHouse(int userId ,int provinceId,int districtId ,int munId)
        //{
        //    return this.Database.SqlQuery<getHouse_Result>("getHouse @userId,@provinceId,@districtId,@munId", new SqlParameter("@userId", userId), new SqlParameter("@provinceId", provinceId), new SqlParameter("@districtId", districtId), new SqlParameter("@munId", munId)).ToList();

        //}



        //public virtual Floor_Details floorCountsonHouse(int hId)
        //{
        //        return this.Database.SqlQuery<Floor_Details>("floorCountsonHouse @houseId", new SqlParameter("@houseId", hId)).First();          
        //}

        ////public virtual Floor_Details syncAllCounts(int mode, int floorSn, int houseId)
        ////{
        ////    return this.Database.SqlQuery<Floor_Details>("floorCountsonHouse @houseId", new SqlParameter("@houseId", hId)).First();

        ////}
    }


}