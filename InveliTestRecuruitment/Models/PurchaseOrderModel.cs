using System.ComponentModel.DataAnnotations;

namespace InveliTestRecuruitment.Models
{
    public class PurchaseOrderModel
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int SupplierID{ get; set; }
        public string Remarks { get; set; }

/*        public SupplierModel Supplier { get; set; }*/
    }
}
