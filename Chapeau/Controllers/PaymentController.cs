using Chapeau.Enums;
using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Chapeau.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly string _connectionString;

        public PaymentController(IPaymentRepository paymentRepository, IConfiguration configuration)
        {
            _paymentRepository = paymentRepository;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        [HttpGet]
        public IActionResult Index(int orderId)
        {
            var activeOrders = GetActiveOrdersFromDatabase();

            if (orderId == 0 && activeOrders.Count > 0)
            {
                orderId = int.Parse(activeOrders[0].Value);
            }

            var orderItems = _paymentRepository.GetOrderItemsByOrderId(orderId);
            if (orderItems == null)
            {
                orderItems = new List<OrderItem>();
            }

            decimal subTotal = 0;
            foreach (var item in orderItems)
            {
                decimal itemPrice = GetMenuItemPriceFromDb(item.MenuItemId);
                subTotal += (itemPrice * item.OrderItemQuantity);
            }

            decimal highVat = subTotal * 0.21m;
            decimal lowVat = subTotal * 0.09m;
            decimal totalAmount = subTotal + highVat + lowVat;

            var viewModel = new PaymentViewModel
            {
                OrderItems = orderItems,
                OrderId = orderId,
                TableId = GetTableIdByOrderId(orderId),
                SubTotal = subTotal,
                HighVat = highVat,
                LowVat = lowVat,
                TotalAmount = totalAmount,
                AmountToPayNow = totalAmount,
                ActiveOrders = activeOrders
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Process(IFormCollection form)
        {
            int orderId = int.Parse(form["orderId"]!);
            int tableId = int.Parse(form["tableId"]!);
            decimal totalAmount = decimal.Parse(form["totalAmount"]!);
            decimal highVat = decimal.Parse(form["highVat"]!);
            decimal lowVat = decimal.Parse(form["lowVat"]!);
            string paymentMethod = form["paymentMethod"]!;
            string feedback = form["feedback"]!;
            string splitType = form["splitType"]!;

            decimal tipAmount = 0;
            decimal.TryParse(form["tipAmount"], out tipAmount);

            decimal customAmount = 0;
            decimal.TryParse(form["customAmount"], out customAmount);

            int customSplits = 1;
            int.TryParse(form["customSplits"], out customSplits);

            decimal actualPaidAmount = totalAmount;

            if (splitType == "equal" && customSplits > 0)
            {
                actualPaidAmount = (totalAmount / customSplits);
            }
            else if (splitType == "custom")
            {
                actualPaidAmount = customAmount;
            }

            var order = new Order(orderId, tableId, 0, DateTime.Now, null, "settled");

            var payment = new Payment
            {
                Order = order,
                TotalAmount = actualPaidAmount + tipAmount,
                HighVatAmount = highVat,
                LowVatAmount = lowVat,
                TipAmount = tipAmount,
                Method = paymentMethod == "Credit Card" ? PaymentMethod.CreditCard : PaymentMethod.Cash,
                Feedback = feedback,
                PaymentTime = DateTime.Now
            };

            _paymentRepository.InsertPayment(payment);

            if (actualPaidAmount >= totalAmount || splitType == "full")
            {
                using var connection = new SqlConnection(_connectionString);
                string updateOrderQuery = "UPDATE [Order] SET OrderStatus = 'settled' WHERE OrderId = @OrderId";
                using var cmd1 = new SqlCommand(updateOrderQuery, connection);
                cmd1.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();
                cmd1.ExecuteNonQuery();
            }

            return RedirectToAction("Success", new { orderId = orderId });
        }

        [HttpGet]
        public IActionResult Success(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        private List<SelectListItem> GetActiveOrdersFromDatabase()
        {
            var list = new List<SelectListItem>();
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT OrderId, TableId FROM [Order] WHERE OrderStatus != 'settled' OR OrderStatus IS NULL";
            using var command = new SqlCommand(query, connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                string orderId = reader["OrderId"].ToString()!;
                string tableId = reader["TableId"].ToString()!;
                list.Add(new SelectListItem { Value = orderId, Text = $"Table {tableId} (Order ID: {orderId})" });
            }
            return list;
        }

        private int GetTableIdByOrderId(int orderId)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT TableId FROM [Order] WHERE OrderId = @OrderId";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@OrderId", orderId);
            connection.Open();
            var result = command.ExecuteScalar();
            return result != null ? (int)result : 1;
        }

        private decimal GetMenuItemPriceFromDb(int menuItemId)
        {
            using var connection = new SqlConnection(_connectionString);
            string query = "SELECT MenuItemPrice FROM MenuItem WHERE MenuItemId = @MenuItemId";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@MenuItemId", menuItemId);

            connection.Open();
            var result = command.ExecuteScalar();
            return result != null ? (decimal)result : 0.00m;
        }
    }
}