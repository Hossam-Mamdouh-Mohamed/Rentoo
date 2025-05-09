﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rentoo.Application.Interfaces;
using Rentoo.Domain.Entities;

namespace Rentoo.Web.Controllers
{
    [Authorize]
    public class ClientDashboardController : Controller
    {
        private readonly IService<User> _userService;
        private readonly IService<Request> _requestService;
        private readonly IService<UserDocument> _userDoucoment;
        private readonly IService<RequestReview> _reviewService;
        public ClientDashboardController(
            IService<User> userService, 
            IService<Request> requestService, 
            IService<UserDocument> userDoucoment,
            IService<RequestReview> reviewService)
        {
            _userService = userService;
            _requestService = requestService;
            _userDoucoment = userDoucoment;
            _reviewService = reviewService;
        }
        public async Task<IActionResult> ClientProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userService.GetByIdAsync(userId);
            return View(currentUser);
        }
        [HttpPost]
        public async Task<IActionResult> ClientProfile(User user, IFormFile? ProfileImage)
        {
            try
            {
                var id = user.Id;
                var existingUser = await _userService.GetByIdAsync(id);
                if (existingUser == null)
                {
                    ModelState.AddModelError("", "User not found");
                    return View("Profile", user);
                }
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var uploadPath = Path.Combine("wwwroot", "uploads");
                    var fileName = ProfileImage.FileName;
                    var filePath = Path.Combine(uploadPath, fileName);
                    await ProfileImage.CopyToAsync(new FileStream(filePath, FileMode.Create));
                    existingUser.UserImage = "uploads/" + fileName;
                }
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.BirthDate = user.BirthDate;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Address = user.Address;
                // Update the user in the database
                await _userService.UpdateAsync(existingUser);
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("ClientProfile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating your profile. Please try again.");
                return View("ClientProfile", user);
            }
        }
        [HttpGet]
        public async Task<IActionResult> MyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requests = await _requestService.GetAllAsync(r => r.UserID == userId, "Car", "User");
            
            // Get all reviews for the current user
            var reviews = await _reviewService.GetAllAsync();
            
            // Create a dictionary of request IDs that have reviews
            var reviewedRequestIds = new HashSet<int>();
            foreach (var review in reviews)
            {
                if (review.RequestId.HasValue)
                {
                    reviewedRequestIds.Add(review.RequestId.Value);
                }
            }
            
            // Pass the reviewed request IDs to the view
            ViewBag.ReviewedRequestIds = reviewedRequestIds;
            
            return View(requests);
        }
        [HttpGet]
        public async Task<IActionResult> YourDoucoment()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userDocument = await _userDoucoment.GetAllAsync(ud => ud.UserId == userId);
            return View(userDocument.FirstOrDefault());
        }

        [HttpPost]
        public async Task<IActionResult> YourDoucoment(UserDocument userDocument, IFormFile nationalIdFile, IFormFile licenseFile)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                userDocument.UserId = userId;

                // If this is an update (ID > 0), check the current status
                if (userDocument.ID > 0)
                {
                    var existingDocument = await _userDoucoment.GetByIdAsync(userDocument.ID);
                    if (existingDocument == null)
                    {
                        return NotFound();
                    }

                    // Only allow updates if status is Accepted
                    if (existingDocument.Status != UserDocumentStatus.Accepted)
                    {
                        TempData["ErrorMessage"] = "You can only update documents when their status is 'Accepted'. Current status is: " + existingDocument.Status;
                        return RedirectToAction(nameof(YourDoucoment));
                    }

                    // Update the existing document instead of creating a new one
                    existingDocument.NationalIDNumber = userDocument.NationalIDNumber;
                    existingDocument.Notes = userDocument.Notes;
                    existingDocument.Status = UserDocumentStatus.Pending;

                    var uploadPath = Path.Combine("wwwroot", "uploads", "documents");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    if (nationalIdFile != null && nationalIdFile.Length > 0)
                    {
                        var fileName = nationalIdFile.FileName;
                        var filePath = Path.Combine(uploadPath, fileName);
                        await nationalIdFile.CopyToAsync(new FileStream(filePath, FileMode.Create));
                        existingDocument.NationalIDUrl = "uploads/documents/" + fileName;
                    }

                    if (licenseFile != null && licenseFile.Length > 0)
                    {
                        var fileName = licenseFile.FileName;
                        var filePath = Path.Combine(uploadPath, fileName);
                        await licenseFile.CopyToAsync(new FileStream(filePath, FileMode.Create));
                        existingDocument.LicenseUrl = "uploads/documents/" + fileName;
                    }

                    await _userDoucoment.UpdateAsync(existingDocument);
                    TempData["SuccessMessage"] = "Documents updated successfully! Status has been set to Pending for review.";
                }
                else
                {
                    // For new documents
                userDocument.Status = UserDocumentStatus.Pending;

                var uploadPath = Path.Combine("wwwroot", "uploads", "documents");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                if (nationalIdFile != null && nationalIdFile.Length > 0)
                {
                    var fileName = nationalIdFile.FileName;
                    var filePath = Path.Combine(uploadPath, fileName);
                    await nationalIdFile.CopyToAsync(new FileStream(filePath, FileMode.Create));
                    userDocument.NationalIDUrl = "uploads/documents/" + fileName;
                }

                if (licenseFile != null && licenseFile.Length > 0)
                {
                    var fileName = licenseFile.FileName;
                    var filePath = Path.Combine(uploadPath, fileName);
                    await licenseFile.CopyToAsync(new FileStream(filePath, FileMode.Create));
                    userDocument.LicenseUrl = "uploads/documents/" + fileName;
                }

                    await _userDoucoment.AddAsync(userDocument);
                    TempData["SuccessMessage"] = "Documents uploaded successfully!";
                }

                return RedirectToAction(nameof(YourDoucoment));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your documents. Please try again.");
                return View(userDocument);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userDocument = await _userDoucoment.GetByIdAsync(id);

                if (userDocument == null || userDocument.UserId != userId)
                {
                    return NotFound();
                }

                // Only delete the database record, leave files on server
                await _userDoucoment.DeleteAsync(userDocument);
                TempData["SuccessMessage"] = "Documents deleted successfully!";
                return RedirectToAction(nameof(YourDoucoment));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting your documents. Please try again.";
                return RedirectToAction(nameof(YourDoucoment));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(int requestId, int rating, string comment)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                // Check if the request exists and belongs to the current user
                var request = await _requestService.GetByIdAsync(requestId);
                if (request == null || request.UserID != userId)
                {
                    TempData["ErrorMessage"] = "Request not found or you don't have permission to review it.";
                    return RedirectToAction("MyRequests");
                }
                
                // Check if the request is completed
                if (request.Status != RequestStatus.Completed)
                {
                    TempData["ErrorMessage"] = "You can only review completed requests.";
                    return RedirectToAction("MyRequests");
                }
                
                // Check if a review already exists for this request
                var existingReviews = await _reviewService.GetAllAsync(r => r.RequestId == requestId);
                if (existingReviews.Any())
                {
                    TempData["ErrorMessage"] = "You have already reviewed this request.";
                    return RedirectToAction("MyRequests");
                }
                
                // Create the review
                var review = new RequestReview
                {
                    RequestId = requestId,
                    Rating = rating,
                    Comment = comment,
                    ReviewDate = DateTime.Now
                };
                
                await _reviewService.AddAsync(review);
                
                // Update the request with the review ID
                request.reviewId = review.ID;
                await _requestService.UpdateAsync(request);
                
                TempData["SuccessMessage"] = "Review submitted successfully!";
                return RedirectToAction("MyRequests");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while submitting your review. Please try again.";
                return RedirectToAction("MyRequests");
            }
        }
    }
}
