using System.ComponentModel.DataAnnotations;

namespace InveliTestRecuruitment.Models
{
    public class SupplierModel
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
    }
}
