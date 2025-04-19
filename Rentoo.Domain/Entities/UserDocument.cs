﻿namespace Rentoo.Domain.Entities
{
    public class UserDocument
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public string LicenseUrl { get; set; }
        public string NationalIDNumber { get; set; }
        public string NationalIDUrl { get; set; }
        public string Notes { get; set; }
        public UserDocumentStatus Status { get; set; } = UserDocumentStatus.Pending;
        public DateTime? ReviewdAt { get; set; }
        public string? ReviewNotes { get; set; }
        public User User { get; set; }
    }
    public enum UserDocumentStatus
    {
        Pending,
        Accepted,
        Rejected
    }
}
