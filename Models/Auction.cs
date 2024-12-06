using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
namespace Lab2Auction.Models
{
    public class Auction
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal StartingPrice { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string? UserId { get; set; } // string reference to User ID
        public string? UserEmail { get; set; }
        public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();
    }
}
