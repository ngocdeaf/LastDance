using Microsoft.AspNetCore.Mvc;
using Ngocdeaf.Data;

namespace Ngocdeaf.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = _context.Order.ToList();
            return View(orders);
        }
    }
}
