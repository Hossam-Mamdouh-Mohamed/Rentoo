using Rentoo.Domain.Entities;

namespace Rentoo.Web.ViewModels
{
    public class CarDetailsViewModel
    {
        public Car Car { get; set; }
        public List<CarImage> CarImages { get; set; }
        public RateCode RateCode { get; set; }
    }
}
