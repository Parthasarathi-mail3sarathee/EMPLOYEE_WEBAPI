using System.ComponentModel.DataAnnotations;

namespace Employee_Shared_Service_Model.Model
{
    public class Login
    {
        [StringLength(20), Required]
        public string username { get; set; }
        [StringLength(20), Required]
        public string password { get; set; }
    }
}
