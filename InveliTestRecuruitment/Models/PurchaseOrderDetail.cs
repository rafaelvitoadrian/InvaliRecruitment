using System.ComponentModel.DataAnnotations;

namespace InveliTestRecuruitment.Models
{
    public class PurchaseOrderDetail
    {
        [Key]
        public int Id { get; set; }
        public int PurchaseOrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
    }
}
