using Microsoft.AspNetCore.Identity;

namespace Rentoo.Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
        public string? Address { get; set; }
        public int UserDocumentId { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; } // Ensure this property matches IdentityUser's type
        public UserDocument UserDocument { get; set; }
        public ICollection<Request> Requests { get; set; }
        public ICollection<Car> Cars { get; set; }
        public ICollection<RequestReview> requestreview { get; set; }
    }
}