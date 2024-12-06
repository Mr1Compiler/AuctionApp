using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Lab2Auction.Models
{
    public class Bid
    {
        public int Id { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [Required]
        public DateTime BidDate { get; set; }
        [Required]
        public int AuctionId { get; set; } // FK to Auction        
        public string? UserId { get; set; } // string reference to User ID
        public string? UserEmail { get; set; }
        public virtual Auction? Auction { get; set; }
    }
}
