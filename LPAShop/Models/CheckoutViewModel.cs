using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LPAShop.NET06.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Collections.Generic;

namespace LPAShop.NET06.Models
{
    public class CheckoutViewModel
    {
        public string FullName { get; set; } // Tên người nhận
        public string PhoneNumber { get; set; } // Số điện thoại người nhận
        public string Address { get; set; } // Địa chỉ giao hàng
        public string PaymentMethod { get; set; } // Phương thức thanh toán

        public List<CartItem> CartItems { get; set; } // Danh sách sản phẩm trong giỏ hàng
        public decimal TotalPrice { get; set; } // Tổng giá tiền
    }

}
