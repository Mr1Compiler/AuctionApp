using System.ComponentModel.DataAnnotations;
namespace Lab2Auction.Models
{
    public class AuctionEditModel
    {
        public int Id { get; set; }
        [Required]
        public string? Description { get; set; }
    }
}