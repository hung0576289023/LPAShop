﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using LPAShop.NET06.Data;
using LPAShop.NET06.Models;

namespace LPAShop.NET06.Controllers
{
    public class CartController : Controller
    {
        public readonly ILogger<CartController> _logger;
        public readonly LPAShopDBContext _context;

        public CartController(ILogger<CartController> logger, LPAShopDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void getCountCartItem(int id)
        {
            //Lấy ra Cart của userid đó
            Cart userCart = _context.Carts.FirstOrDefault(c => c.User_ID == id);

            //Kiểm tra nếu khác null thì thực hiện việc, lấy số lượng cartItem và lưu vào ViewBag để truyền lại cho _Layout
            if (userCart != null)
            {
                int cartItemCount = _context.CartItems.Count(c => c.Cart_ID == userCart.Cart_ID);
                HttpContext.Session.SetInt32("count", cartItemCount);
                ViewBag.CountCart = HttpContext.Session.GetInt32("count");
            }
        }

        public void getUserIDAndUserName(int id)
        {
            var userName = _context.Users.Where(u => u.User_ID == id).FirstOrDefault();
            ViewBag.UserID = id;
            ViewBag.UserName = userName.User_Name;
        }

        [HttpPost]
        public IActionResult AddToCart(string Proudct_ID, string Product_Quantity)
        {
            var product = _context.Products.Where(p => p.Product_ID == Proudct_ID).FirstOrDefault();

            var userid = HttpContext.Session.GetInt32("UserId");

            var cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.User_ID == userid);
            
            // Kiếm tra nếu userid null nghĩa là người dùng chưa đăng, hiển thị thông báo, và điều hướng đến trang Login
            if (userid == null)
            {
                TempData["LoginToAddCart"] = "Hãy đăng nhập vào tài khoản để thêm sản phẩm vào giỏ hàng của bạn";
                return RedirectToAction("Login", "Account");
            } 

            // Trường hợp user đã đăng nhập, nhưng cart lại chưa có, thì sẽ tạo mới
            if (cart == null)
            {
                cart = new Cart()
                {
                    User_ID = (int)userid,
                    CartItems = new List<CartItem>()
                };
                _context.Carts.Add(cart);
            }

            // Lấy ra CartItem của sản phẩm ta đang định thêm vào
            var cartItem = cart.CartItems.FirstOrDefault(c => c.Product_ID == Proudct_ID);

            // Trường hợp đã có thì ta sẽ tăng thêm số lượng của sản phẩm ta thêm vào giỏ hàng
            if (cartItem != null)
            {
                cartItem.Quantity += int.Parse(Product_Quantity);
            } // Trường hợp chưa có CartItem cho sp đó thì ta sẽ tạo mới
            else
            {
                cartItem = new CartItem()
                {
                    Product_ID = Proudct_ID,
                    Cart_ID = cart.Cart_ID,
                    Quantity = int.Parse(Product_Quantity),
                    Product_Name = product.Product_Name,
                    Product_Price = product.Product_Price,
                };
                cart.CartItems.Add(cartItem);
            }
            // Sau cùng ta sẽ lưu những dữ liệu xuống db
            _context.SaveChanges();

            getCountCartItem((int)userid);


            TempData["AddToCartSuccess"] = "Đã thêm vào giỏ hàng thành công";

            return RedirectToRoute("MyCustomRoute", new { id = product.Product_ID });
            //return RedirectToAction("ViewCart");
            //return RedirectToPage("/chi-tiet-san-pham", new { id = product.Product_ID });
        }

        [Route("gio-hang/chi-tiet-gio-hang-cua-ban")]        
        public IActionResult ViewCart(int page = 1, int pageSize = 4)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["LoginToViewCart"] = "Hãy đăng nhập vào tài khoản để xem giỏ hàng của bạn";
                return RedirectToAction("Login", "Account");
            } 
            else
            {
                getUserIDAndUserName((int)userId);
                getCountCartItem((int)userId);
            }

            var cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.User_ID == userId);

            if (cart == null)
            {
                return View();
            }

            var listCartItems = cart.CartItems.OrderBy(c => c.CartItems_ID)
                                                .Skip((page - 1)*pageSize)
                                                .Take(pageSize)
                                                .ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(cart.CartItems.Count() / (double)pageSize);

            //return View(cart.CartItems.ToList());
            // Tính tổng tiền
            decimal totalPrice = cart.CartItems.Sum(item => item.Quantity * item.Product_Price);

            // Lưu tổng tiền vào ViewBag để sử dụng trong view
            ViewBag.TotalPrice = totalPrice;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(cart.CartItems.Count() / (double)pageSize);
            return View(listCartItems);
        }
        [HttpPost]
        [Route("gio-hang/thanh-toan")]
        public IActionResult Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["LoginToCheckout"] = "Hãy đăng nhập vào tài khoản để tiếp tục thanh toán.";
                return RedirectToAction("Login", "Account");
            }

            // Lấy thông tin giỏ hàng của người dùng
            var cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.User_ID == userId);
            if (cart == null)
            {
                TempData["CartEmpty"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("ViewCart");
            }

            var checkoutViewModel = new CheckoutViewModel
            {
                FullName = "",  // Người dùng sẽ điền thông tin này
                PhoneNumber = "",
                Address = "",
                CartItems = cart.CartItems.ToList(),
                TotalPrice = cart.CartItems.Sum(item => item.Quantity * item.Product_Price)
            };

            return View(checkoutViewModel);
        }
        [HttpPost]
        [Route("gio-hang/xac-nhan-thanh-toan")]
        public IActionResult ConfirmOrder(CheckoutViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["LoginToCheckout"] = "Hãy đăng nhập vào tài khoản để tiếp tục thanh toán.";
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                // Tạo mới đơn hàng
                var order = new Order
                {
                    User_ID = userId.Value,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    PaymentMethod = model.PaymentMethod,
                    OrderDate = DateTime.Now,
                    TotalPrice = model.TotalPrice,
                    OrderItems = model.CartItems.Select(item => new OrderItems
                    {
                        Product_ID = item.Product_ID,
                        Quantity = item.Quantity,
                        Price = item.Product_Price
                    }).ToList()
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                // Xóa giỏ hàng sau khi đặt hàng thành công
                var cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.User_ID == userId);
                _context.CartItems.RemoveRange(cart.CartItems);
                _context.SaveChanges();

                return RedirectToAction("OrderSuccess");
            }

            return View("Checkout", model);
        }


        [HttpPost]
        [Route("xoa-san-pham")]
        public IActionResult RemoveProductFromCart(string Product_ID)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            Cart cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.User_ID == userId);

            if (cart != null)
            {
                CartItem cartItem = cart.CartItems.FirstOrDefault(c => c.Product_ID == Product_ID);
                if (cartItem != null)
                {
                    _context.CartItems.Remove(cartItem);
                    _context.SaveChanges();
                }
                int cartItemCount = _context.CartItems.Count(c => c.Cart_ID == cart.Cart_ID);
                ViewBag.CountCart = cartItemCount;
            }
            TempData["InforSuccessRemove"] = "Bạn đã xóa sản phẩm khỏi giỏ hàng thành công";

            return RedirectToAction("ViewCart", "Cart");
        }

        [HttpPost]
        [Route("cap-nhat-so-luong-san-pham")]
        public ActionResult UpdateProductFromCart(string Product_ID, int Quantity)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            Cart cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.User_ID == userId);

            if (cart != null)
            {
                CartItem cartItem = cart.CartItems.FirstOrDefault(c => c.Product_ID == Product_ID);
                if (cartItem != null)
                {
                    cartItem.Quantity = Quantity;
                    _context.SaveChanges();
                }
                int cartItemCount = _context.CartItems.Count(c => c.Cart_ID == cart.Cart_ID);
                ViewBag.CountCart = cartItemCount;
            }
            TempData["InforSuccessUpdate"] = "Bạn đã cập nhật số lượng sản phẩm trong giỏ hàng thành công";

            return RedirectToAction("ViewCart", "Cart");
            // Chuyển hướng người dùng về trang giỏ hàng hoặc trang khác tùy ý
        }

    }
}
