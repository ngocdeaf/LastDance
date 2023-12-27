﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ngocdeaf.Data;
using Ngocdeaf.Models;
using Newtonsoft.Json;

namespace Ngocdeaf.Controllers
{
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Carts
        public ActionResult Index()
        {

            var _product = getAllProduct();
            ViewBag.product = _product;
            return View();
        }

        public List<Book> getAllProduct()
        {
            return _context.Book.ToList();
        }

        //GET DETAIL PRODUCT
        public Book getDetailProduct(int id)
        {
            var product = _context.Book.Find(id);
            return product;
        }

        //ADD CART
        public IActionResult addCart(int id)
        {
            var cart = HttpContext.Session.GetString("cart");//get key cart
            if (cart == null)
            {
                var product = getDetailProduct(id);
                List<Cart> listCart = new List<Cart>()
               {
                   new Cart
                   {
                       Book = product,
                       Quantity = 1
                   }
               };
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(listCart));

            }
            else
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                bool check = true;
                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Book.Id == id)
                    {
                        dataCart[i].Quantity++;
                        check = false;
                    }
                }
                if (check)
                {
                    dataCart.Add(new Cart
                    {
                        Book = getDetailProduct(id),
                        Quantity = 1
                    });
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
            }

            return RedirectToAction(nameof(Index));

        }

        public IActionResult ListCart()
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                if (dataCart.Count > 0)
                {
                    ViewBag.carts = dataCart;
                    return View();
                }
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult UpdateCart(int newBookid, int newBookquantity)
        {
            var cart2 = HttpContext.Session.GetString("cart");
            if (cart2 != null)
            {
                List<Cart> BookCart = JsonConvert.DeserializeObject<List<Cart>>(cart2);
                if (newBookquantity > 0)
                {
                    for (int i = 0; i < BookCart.Count; i++)
                    {
                        if (BookCart[i].Book.Id == newBookid)
                        {
                            BookCart[i].Quantity = newBookquantity;
                        }
                    }

                    HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(BookCart));

                    return RedirectToAction("ListCart");
                }
            }
            return BadRequest();
        }

        public IActionResult deleteCart(int id)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Book.Id == id)
                    {
                        dataCart.RemoveAt(i);
                    }
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                return RedirectToAction(nameof(ListCart));
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult ConfirmOrder()
        {
            var cartJson = HttpContext.Session.GetString("cart");

            if (cartJson != null)
            {
                var carts = JsonConvert.DeserializeObject<List<Cart>>(cartJson);

                double totalAmount = 0;

                foreach (var item in carts)
                {
                    var product = _context.Book.Find(item.Book.Id);

                    if (product != null)
                    {
                        totalAmount += product.Price * item.Quantity;
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                ViewBag.TotalAmount = totalAmount;
                ViewBag.Carts = carts;
                HttpContext.Session.SetString("TotalAmount", JsonConvert.SerializeObject(totalAmount));
                var totalAmountFromSession = HttpContext.Session.GetString("TotalAmount");
                return View();
            }
            return RedirectToAction(nameof(Index));
        }
        // Trong Controller Orders
        public IActionResult Confirm(Order orderDetails)
        {if (ModelState.IsValid)
            {
                var cart = HttpContext.Session.GetString("cart");
                double totalPrice = 0;

                if (cart != null)
                {
                    List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                    if (dataCart.Count > 0)
                    {
                        // Tính tổng tiền
                        foreach (var item in dataCart)
                        {
                            totalPrice += item.Book.Price * item.Quantity;
                        }

                        // Lưu thông tin vào đối tượng Order
                        Order order = new Order
                        {
                            FullName = orderDetails.FullName,
                            Address = orderDetails.Address,
                            Total = totalPrice
                        };

                        _context.Order.Add(order);
                        _context.SaveChanges();
                        HttpContext.Session.Remove("cart");
                    }
                }

                return RedirectToAction(nameof(Index)); // Hoặc chuyển hướng đến trang Index nếu không có dữ liệu giỏ hàng
            }

            return View(orderDetails);

        }

    }
}
