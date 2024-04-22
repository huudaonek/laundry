using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Laundry.Data;
using Laundry.Models;
using Microsoft.AspNetCore.Http;

namespace Laundry.Controllers
{
    public class LaundryItemsController : Controller
    {
        private readonly LaundryContext _context;

        public LaundryItemsController(LaundryContext context)
        {
            _context = context;
        }

        // GET: LaundryItems
        public async Task<IActionResult> Index()
        {
            return View(await _context.LaundryItems.ToListAsync());
        }

        // GET: LaundryItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laundryItem = await _context.LaundryItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (laundryItem == null)
            {
                return NotFound();
            }

            return View(laundryItem);
        }

        // GET: LaundryItems/Create
        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("FullName")))
            {
                return RedirectToAction("Create", "Appointments");
            }

            var laundryItems = _context.LaundryItems.ToList();
            return View(laundryItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string[] selectedProducts, Dictionary<string, int> quantities)
        {
            if (selectedProducts == null || selectedProducts.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please select at least one product.");
                var laundryItems = _context.LaundryItems.ToList();
                return View(laundryItems);
            }

            var selectedProductsByteArray = selectedProducts.Select(s => int.Parse(s)).SelectMany(BitConverter.GetBytes).ToArray();

            var quantitiesByteArray = quantities.SelectMany(kv => BitConverter.GetBytes(int.Parse(kv.Key))
                                                        .Concat(BitConverter.GetBytes(kv.Value)))
                                                        .ToArray();

            HttpContext.Session.Set("SelectedProducts", selectedProductsByteArray);

            HttpContext.Session.Set("Quantities", quantitiesByteArray);

            return RedirectToAction("ReviewOrder");
        }

        public IActionResult ReviewOrder()
        {
            var fullName = HttpContext.Session.GetString("FullName");
            var phoneNumber = HttpContext.Session.GetString("PhoneNumber");
            var address = HttpContext.Session.GetString("Address");
            var appointmentDate = HttpContext.Session.GetString("AppointmentDate");

            var laundryItems = _context.LaundryItems.ToList();

            var selectedProducts = HttpContext.Session.Get("SelectedProducts");
            var quantities = HttpContext.Session.Get("Quantities");

            var selectedProductIds = new List<int>();

            if (selectedProducts != null)
            {
                for (int i = 0; i < selectedProducts.Length; i += 4)
                {
                    selectedProductIds.Add(BitConverter.ToInt32(selectedProducts, i));
                }
            }

            var totalPrice = 0m; // Tính tổng tiền từ các sản phẩm đã chọn với kiểu decimal
            var viewModel = new OrderViewModel
            {
                FullName = fullName ?? "",
                PhoneNumber = phoneNumber ?? "",
                Address = address ?? "",
                AppointmentDate = appointmentDate ?? "",
                SelectedProducts = selectedProductIds,
                Quantities = ConvertByteArrayToDictionary(quantities),
                LaundryItems = laundryItems
            };

            foreach (var selectedProductId in viewModel.SelectedProducts)
            {
                var selectedItem = viewModel.LaundryItems.FirstOrDefault(item => item.Id == selectedProductId);
                var quantity = viewModel.Quantities.ContainsKey(selectedProductId) ? viewModel.Quantities[selectedProductId] : 0;
                if (selectedItem != null)
                {
                    totalPrice += selectedItem.Price * quantity;
                }
            }

            // Gán tổng tiền vào model
            viewModel.TotalPrice = totalPrice;

            return View("~/Views/Order/ReviewOrder.cshtml", viewModel);
        }



        // Phương thức chuyển đổi từ byte array sang Dictionary<int, int>
        private Dictionary<int, int> ConvertByteArrayToDictionary(byte[] byteArray)
        {
            var quantities = new Dictionary<int, int>();

            if (byteArray != null)
            {
                for (int i = 0; i < byteArray.Length; i += 8)
                {
                    var key = BitConverter.ToInt32(byteArray, i);
                    var value = BitConverter.ToInt32(byteArray, i + 4);
                    quantities.Add(key, value);
                }
            }

            return quantities;
        }




        // GET: LaundryItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laundryItem = await _context.LaundryItems.FindAsync(id);
            if (laundryItem == null)
            {
                return NotFound();
            }
            return View(laundryItem);
        }

        // POST: LaundryItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Product,Description,Weight")] LaundryItem laundryItem)
        {
            if (id != laundryItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(laundryItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LaundryItemExists(laundryItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(laundryItem);
        }

        // GET: LaundryItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var laundryItem = await _context.LaundryItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (laundryItem == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/AddProduct.cshtml", laundryItem);
        }

        // POST: LaundryItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var laundryItem = await _context.LaundryItems.FindAsync(id);
            if (laundryItem != null)
            {
                _context.LaundryItems.Remove(laundryItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LaundryItemExists(int id)
        {
            return _context.LaundryItems.Any(e => e.Id == id);
        }

        public IActionResult Thank()
        {
            // Get the ID of the last saved OrderDetail
            int lastOrderId = _context.OrderDetail.Max(order => order.Id);

            // Retrieve the last saved OrderDetail from the database
            var order = _context.OrderDetail
                            .Where(o => o.Id == lastOrderId)
                            .FirstOrDefault();

            return View("~/Views/Order/Thanks.cshtml", order); // Trả về một OrderDetail thay vì một danh sách
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            // Lấy thông tin từ Session
            var fullName = HttpContext.Session.GetString("FullName");
            var phoneNumber = HttpContext.Session.GetString("PhoneNumber");
            var address = HttpContext.Session.GetString("Address");
            var appointmentDate = HttpContext.Session.GetString("AppointmentDate");
            var selectedProducts = HttpContext.Session.Get("SelectedProducts");
            var quantities = HttpContext.Session.Get("Quantities");
            var totalPrice = 0m; // Khởi tạo tổng tiền

            // Tạo đối tượng OrderDetail
            var orderDetail = new OrderDetail
            {
                OrderCode = GenerateOrderCode(),
                FullName = fullName,
                PhoneNumber = phoneNumber,
                Address = address,
                AppointmentDate = appointmentDate,
                Status = OrderStatus.WaitingForDelivery
            };

            // Lặp qua danh sách các sản phẩm đã chọn để tính tổng tiền
            if (selectedProducts != null && quantities != null)
            {
                for (int i = 0; i < selectedProducts.Length; i += 4)
                {
                    var productId = BitConverter.ToInt32(selectedProducts, i);
                    var quantity = BitConverter.ToInt32(quantities, i);
                    var laundryItem = await _context.LaundryItems.FindAsync(productId);
                    if (laundryItem != null)
                    {
                        totalPrice += laundryItem.Price * quantity;
                        orderDetail.Product += $"{laundryItem.Product} ({quantity}), "; // Gán thông tin về sản phẩm vào đối tượng OrderDetail
                    }
                }
            }

            // Gán tổng tiền vào đối tượng OrderDetail
            orderDetail.Price = totalPrice;

            // Lưu vào cơ sở dữ liệu
            _context.OrderDetail.Add(orderDetail);
            await _context.SaveChangesAsync();

            // Xóa các thông tin trong Session sau khi đã lưu vào cơ sở dữ liệu
            HttpContext.Session.Remove("FullName");
            HttpContext.Session.Remove("PhoneNumber");
            HttpContext.Session.Remove("Address");
            HttpContext.Session.Remove("AppointmentDate");
            HttpContext.Session.Remove("SelectedProducts");
            HttpContext.Session.Remove("Quantities");

            return RedirectToAction("Thank");
        }

        private string GenerateOrderCode()
        {
            return "ORD" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        public async Task<IActionResult> Search(string search)
        {
            // Kiểm tra xem chuỗi tìm kiếm có tồn tại không
            if (string.IsNullOrEmpty(search))
            {
                // Nếu không có chuỗi tìm kiếm, hiển thị trang không có kết quả
                return View("~/Views/Order/OrderStatus.cshtml", new List<OrderDetail>());
            }

            // Tìm kiếm trong cơ sở dữ liệu các đơn hàng có chứa chuỗi tìm kiếm trong OrderCode
            var orders = await _context.OrderDetail
                .Where(o => o.OrderCode.Contains(search))
                .ToListAsync();

            return View("~/Views/Order/OrderStatus.cshtml", orders); // Chuyển dữ liệu orders sang view
        }

        // GET: LaundryItems/AddProduct
        public IActionResult AddProduct()
        {
            var laundryItems = _context.LaundryItems.ToList();
            return View("~/Views/Admin/AddProduct.cshtml", laundryItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(LaundryItem laundryItem)
        {
            if (ModelState.IsValid)
            {
                _context.LaundryItems.Add(laundryItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AddProduct));
            }
            return View("~/Views/Admin/AddProduct.cshtml", laundryItem);
        }
    }
}
