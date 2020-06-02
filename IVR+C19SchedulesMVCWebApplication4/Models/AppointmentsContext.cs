﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IVR_C19SchedulesMVCWebApplication4.Models
{
    public class AppointmentsContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public AppointmentsContext() : base("name=AppointmentsContext")
        {
        }

        public System.Data.Entity.DbSet<IVR_C19SchedulesMVCWebApplication4.Models.Appointment> Appointments { get; set; }
    }
}
