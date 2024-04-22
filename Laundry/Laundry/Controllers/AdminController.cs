using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Laundry.Data;
using Laundry.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Laundry.Controllers
{
    public class AdminController : Controller
    {
        private readonly LaundryContext _context;

        public AdminController(LaundryContext context)
        {
            _context = context;
        }

		public IActionResult Index(int page = 1)
		{
			if (HttpContext.Session.GetString("UserId") != null)
			{
				var pageSize = 5;
				var allOrders = _context.OrderDetail.ToList();

				var sortedOrders = allOrders.OrderBy(order => order.Status switch
				{
					OrderStatus.WaitingForDelivery => 0,
					OrderStatus.Processing => 1,
					OrderStatus.ShippedToYou => 2,
					_ => 3
				});

				var totalItems = sortedOrders.Count();
				var totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize);
				var currentPageOrders = sortedOrders.Skip((page - 1) * pageSize).Take(pageSize);

				// Tính tổng doanh số
				ViewBag.TotalOrders = allOrders.Count;

				decimal totalSales = 0;
				//foreach (var order in allOrders)
				//{
				//	if (decimal.TryParse(order.Price, out decimal price))
				//	{
				//		totalSales += price;
				//	}
				//}

				ViewBag.TotalOrders = totalItems;
				ViewBag.TotalPages = totalPages;
				ViewBag.PageIndex = page;
				ViewBag.HasPreviousPage = page > 1;
				ViewBag.HasNextPage = page < totalPages;

				// Đưa tổng doanh số vào ViewBag
				ViewBag.TotalSales = totalSales;

				return View(currentPageOrders);
			}
			else
			{
				return RedirectToAction("Login");
			}
		}

        public IActionResult OrdersByStatus(OrderStatus status, int page = 1)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                var pageSize = 5;
                var allOrders = _context.OrderDetail.Where(order => order.Status == status).ToList();

                var sortedOrders = allOrders.OrderBy(order => order.Status switch
                {
                    OrderStatus.WaitingForDelivery => 0,
                    OrderStatus.Processing => 1,
                    OrderStatus.ShippedToYou => 2,
                    _ => 3
                });

                var totalItems = sortedOrders.Count();
                var totalPages = (int)Math.Ceiling((decimal)totalItems / pageSize);
                var currentPageOrders = sortedOrders.Skip((page - 1) * pageSize).Take(pageSize);

                // Tính tổng doanh số
                ViewBag.TotalOrders = allOrders.Count;

                decimal totalSales = 0;
                //foreach (var order in allOrders)
                //{
                //    if (decimal.TryParse(order.Price, out decimal price))
                //    {
                //        totalSales += price;
                //    }
                //}

                ViewBag.TotalOrders = totalItems;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageIndex = page;
                ViewBag.HasPreviousPage = page > 1;
                ViewBag.HasNextPage = page < totalPages;

                // Đưa tổng doanh số vào ViewBag
                ViewBag.TotalSales = totalSales;

                return View("Index", currentPageOrders);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult AllOrder()
        {
            var allOrders = _context.OrderDetail.ToList();
            return View("Detail", allOrders);
        }
        public IActionResult Detail(int orderId)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                var order = _context.OrderDetail.FirstOrDefault(o => o.Id == orderId);
                if (order == null)
                {
                    return NotFound();
                }

                var orderList = new List<OrderDetail> { order };

                return View(orderList);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }



        [HttpPost]
        public IActionResult UpdateStatus(int orderId, OrderStatus status)
        {
            var order = _context.OrderDetail.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound();
            }

            if (order.Status == OrderStatus.WaitingForDelivery)
            {
                order.Status = OrderStatus.Processing;
            }
            else if (order.Status == OrderStatus.Processing)
            {
                order.Status = OrderStatus.ShippedToYou;
            }

            _context.SaveChanges();

            return RedirectToAction("Detail", new { orderId = order.Id });
        }

        public IActionResult Login()
        {
            return View("~/Views/Login/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            var existingUser = _context.User.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            if (existingUser != null)
            {
                HttpContext.Session.SetString("UserId", existingUser.Id.ToString());
                HttpContext.Session.SetString("UserName", existingUser.Name);
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ViewBag.Error = "Invalid email or password";
                return View("Login");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult UpdateOrder(int orderId, int quantity, decimal price)
        {
            var order = _context.OrderDetail.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Quantity = quantity;
            order.Price = price;

            _context.SaveChanges();

            return RedirectToAction("Detail", new { orderId = order.Id });
        }
        public IActionResult AddOrder()
        {
            return View();
        }

        // POST: Admin/AddOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrder([Bind("OrderCode,FullName,PhoneNumber,Address,AppointmentDate,Product,Quantity,Price")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(orderDetail);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có
                    ViewBag.Error = "An error occurred while adding the order.";
                    return View("Error");
                }
            }
            return View(orderDetail);
        }
    }
}
