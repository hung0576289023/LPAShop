using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LPAShop.NET06.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace LPAShop.NET06.Models
{
    public class Order
    {
        public int Order_ID { get; set; }
        public int User_ID { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }

        public virtual ICollection<OrderItems> OrderItems { get; set; }   // Mối quan hệ đến OrderItem
    }
}