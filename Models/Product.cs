using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for OrderItems
        public List<OrderItem> OrderItems { get; set; } = new();
    }

    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }  // FK to ApplicationUser

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        // Navigation property
        public List<OrderItem> OrderItems { get; set; } = new();
    }

    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }      // FK to Order
        public Order Order { get; set; }      // Navigation property

        [Required]
        public int ProductId { get; set; }    // FK to Product
        public Product Product { get; set; }  // Navigation property

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
   
        public class Payment
        {
            public int Id { get; set; }

            [Required]
            public string UserId { get; set; }  // Link to ApplicationUser

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
            [CustomValidation(typeof(Payment), nameof(ValidateExpiryDate))]
            public DateTime ExpiryDate { get; set; }

            [Required]
            [Column(TypeName = "decimal(18,2)")]
            public decimal Amount { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            // Custom validation to ensure expiry date is in the future
            public static ValidationResult ValidateExpiryDate(DateTime expiry, ValidationContext context)
            {
                if (expiry < DateTime.UtcNow.Date)
                    return new ValidationResult("Card has expired");
                return ValidationResult.Success;
            }
        }
    }


