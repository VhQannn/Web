using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;

namespace Web.Pages
{
    public class MyPaymentModel : PageModel
    {
        private readonly Web.DbConnection.WebContext _context;

        public MyPaymentModel(Web.DbConnection.WebContext context)
        {
            _context = context;
        }

        


	}
}
