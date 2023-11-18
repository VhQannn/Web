using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập Username.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Username không được chứa ký tự đặc biệt hoặc khoảng trắng.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Usernam phải dài hơn 6 kí tự.")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải dài hơn 6 kí tự.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        public string? ConfirmPassword { get; set; }
        
        [Required(ErrorMessage = "Vui lòng Email.")]
        public string? Email { get; set; } = null!;
    }
}
