using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.DbConnection;

namespace Web.Pages.Admin
{
    public class VirtualCurrencyManagementModel : PageModel
    {
        private WebContext _context;
        public VirtualCurrencyManagementModel(WebContext context)
        {
            _context = context;
        }

        public decimal TotalAmount { get; set; } = 0;
        public int Id { get; set; }

        public List<VirtualCurrency> VirtualCurrencyList { get; set; }

        public void OnGet(int id)
        {
            // Assign the id to the property
            Id = id;
            VirtualCurrencyList = GetVirtualCurrencies(id);
            if (VirtualCurrencyList != null)
            {
                TotalAmount = VirtualCurrencyList.Sum(x => x.Amount);
            }
            // Your logic here
        }

        public List<VirtualCurrency> GetVirtualCurrencies(int id)
        {
            var list = _context.VirtualCurrencies.Where(x => x.UserId == Id).ToList();
            return list;
        }
    }
}
