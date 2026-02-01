using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
    public class HomeViewModel
    {
        public List<Product> Products { get; set; }
        public bool IsLoggedIn { get; set; }  // Add this
        public string UserName { get; set; } = string.Empty;


        // Cart info
        public List<CartItemViewModel> CartItems { get; set; } = new();
        public int CartCount => CartItems.Sum(c => c.Quantity);
    }



    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int StockQuantity { get; set; }

        // Allow ANY image type
        [DataType(DataType.Upload)]
        public IFormFile? Image { get; set; }

        public string? ExistingImageUrl { get; set; }
    }
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
        public Product Product { get; set; }
        public int Stock { get; set; } // <-- maximum available
    }

    public class ReceiptViewModel
    {
        public Order Order { get; set; }
        public Payment Payment { get; set; }
    }
    public class AdminOrdersViewModel
    {
      public AdminOrdersViewModel() { }
      
            public Order Order { get; set; }
            public Payment Payment { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
    

    public class OrderWithPaymentViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }   // from Payment.CardHolder
        public string Email { get; set; }      // from Payment.Email
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }


    public class AdminOrderDetailsViewModel
    {
        public Order Order { get; set; }
        public Payment Payment { get; set; }
        // Optional: Fetch username/email from session or payment
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }

    public class CheckoutViewModel
    {
        public List<CartItemViewModel> CartItems { get; set; } = new();

        // Payment details
        [Required]
        [StringLength(100)]
        public string CardHolder { get; set; }

        [Required]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must be 16 digits")]
        [DataType(DataType.CreditCard)]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV must be 3 digits")]
        public string CVV { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        [CustomValidation(typeof(CheckoutViewModel), nameof(ValidateExpiryDate))]
        public DateTime ExpiryDate { get; set; }

        public decimal TotalAmount => CartItems != null ? CartItems.Sum(x => x.Price * x.Quantity) : 0;

        // Custom validation for expiry
        public static ValidationResult ValidateExpiryDate(DateTime expiry, ValidationContext context)
        {
            if (expiry < DateTime.UtcNow.Date)
                return new ValidationResult("Card has expired");
            return ValidationResult.Success;
        }
    }


}



