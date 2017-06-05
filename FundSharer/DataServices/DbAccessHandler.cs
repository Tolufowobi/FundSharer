using FundSharer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FundSharer.DataServices
{

    public class DbAccessHandler
    {
        private static ApplicationDbContext _DbContext = null;
    
        public static void Initialiaze()
        {
            ApplicationDbContext _DbContext = ApplicationDbContext.Create();
        }

        public static ApplicationDbContext DbContext
        {
            get { return _DbContext ?? ApplicationDbContext.Create(); }
        }
    }
}