using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentoo.Domain.Entities    
{
    [Table("Cars")]
    public class Car
    {
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string Transmission { get; set; }

        [Required]
        [StringLength(100)]
        public string Model { get; set; }

        [Required]
        [Range(1, 10)]
        public int Seats { get; set; }

        [Required]
        [StringLength(50)]
        public string Color { get; set; }

        [Required]
        public bool AirCondition { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(1900, 2100)]
        public int FactoryYear { get; set; }

        [Required]
        public WithDriverEnum WithDriver { get; set; }

        [Required]
        [StringLength(20)]
        public string Fuel { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public float Mileage { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        public bool IsAvailable { get; set; }

        [Required]
        public int RateCodeId { get; set; }

        [ForeignKey("RateCodeId")]
        public RateCode rateCode { get; set; }

        [Required]
        public int CarDocumentId { get; set; }

        [ForeignKey("CarDocumentId")]
        public CarDocument CarDocument { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public ICollection<CarImage> Images { get; set; }
        public ICollection<Request> Requests { get; set; }
    }

    public enum WithDriverEnum
    {
        Yes,
        No,
        Both
    }
}
