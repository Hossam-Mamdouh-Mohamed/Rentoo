namespace Rentoo.Web.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int OwnersCount { get; set; }
        public int RentersCount { get; set; }
        public int CarsCount { get; set; }
        public int RentalsCount { get; set; }
        public int RentalsInProgressCount { get; set; }
        public int RentalsCompletedCount { get; set; }
        public int RentalsCancelledCount { get; set; }
        public int RentalsPendingCount { get; set; }

    }
}
