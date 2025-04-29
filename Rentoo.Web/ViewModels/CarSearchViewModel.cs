using Rentoo.Domain.Entities;

namespace Rentoo.Web.ViewModels
{
    public class CarSearchViewModel
    {
        public string Model { get; set; }
        public int FactoryYear { get; set; }
        public string Transmission { get; set; }
        public string Address { get; set; }
        public List<Car> Cars { get; set; }
    }
}
