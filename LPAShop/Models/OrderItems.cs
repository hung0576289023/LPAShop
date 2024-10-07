using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LPAShop.NET06.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace LPAShop.NET06.Models
{
    public class OrderItems
    {
        public int OrderItem_ID { get; set; }
        public int Order_ID { get; set; } // Khóa ngoại đến Order
        public string Product_ID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual Order Order { get; set; } // Mối quan hệ đến Order
    }
}
