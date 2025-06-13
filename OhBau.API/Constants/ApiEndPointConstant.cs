﻿using System.Runtime.CompilerServices;

namespace OhBau.API.Constants
{
    public static class ApiEndPointConstant
    {
        static ApiEndPointConstant()
        {
        }

        public const string RootEndPoint = "/api";
        public const string ApiVersion = "/v1";
        public const string ApiEndpoint = RootEndPoint + ApiVersion;

        public static class Account
        {
            public const string AccountEndPoint = ApiEndpoint + "/account";
            public const string RegisterAccount = AccountEndPoint;
            public const string GetAccounts = AccountEndPoint;
            public const string GetAccount = AccountEndPoint + "/{id}";
            public const string UpdateAccount = AccountEndPoint;
            public const string DeleteAccount = AccountEndPoint + "/{id}";
            public const string GetAccountProfile = AccountEndPoint + "/profile";
            public const string ChangePassword = AccountEndPoint + "/change-password";
        }

        public static class Authentication
        {
            public const string AuthEndPoint = ApiEndpoint + "/auth";
            public const string Auth = AuthEndPoint;
        }

        public static class Parent
        {
            public const string ParentEndPoint = ApiEndpoint + "/parent";
            public const string CreateParent = ParentEndPoint;
        }

        public static class ParentRelation
        {
            public const string ParentRelationEndPoint = ApiEndpoint + "/parent-relation";
            public const string GetParentRelation = ParentRelationEndPoint;
        }


        public static class Chapter
        {
            public const string AddChapter = "add-chapter";
            public const string GetChapters = "get-chapters";
            public const string UpdateChapter = "update-chapter";
            public const string GetChapter = "get-chapter";
        }

        public static class Fetus
        {
            public const string FetusEndPoint = ApiEndpoint + "/fetus";
            public const string CreateFetus = FetusEndPoint;
            public const string GetAllFetus = FetusEndPoint;
            public const string GetFetusById = FetusEndPoint + "/{id}";
            public const string GetFetusByCode = FetusEndPoint + "/code";
            //public const string UpdateFetus = FetusEndPoint + "/{fetusId}";
            public const string UpdateFetus = FetusEndPoint + "/{id}";
            public const string DeleteFetus = FetusEndPoint + "/{id}";
            public const string UpdateFetusDetail = FetusEndPoint + "/{id}/fetus-detail"; 
        }

        public static class Blog
        {
            public const string CreateBlog = "create";
            public const string GetBlogs = "get-blogs";
            public const string GetBlog = "get-blog";
            public const string DeleteBlog = "delete-blog";
            public const string UpdateBlog = "update-blog/{id}";
        }

        public static class Comment
        {
            public const string CreateComment = "comment";
            public const string Reply = "reply";
            public const string GetComments = "get-comments";
        }

        public static class MotherHealthRecord
        {
            public const string MotherHealthRecordEndPoint = ApiEndpoint + "/mother-health";
            public const string UpdateMotherHealth = MotherHealthRecordEndPoint + "/{id}";
            public const string GetMotherHealth = MotherHealthRecordEndPoint + "/{id}";
        }

        public static class Slot
        {
            public const string SlotEndPoint = ApiEndpoint + "/slot";
            public const string CreateSlot = SlotEndPoint;
            public const string GetAllSlot = SlotEndPoint;
            public const string GetSlot = SlotEndPoint + "/{id}";
        }

        public static class DoctorSlot
        {
            public const string DoctorSlotEndPoint = ApiEndpoint + "/doctor-slot";
            public const string CreateDoctorSlot = DoctorSlotEndPoint;
            public const string GetAllDoctorSlot = DoctorSlotEndPoint;
            public const string GetDoctorSlot = DoctorSlotEndPoint + "/{id}";
            public const string ActiveDoctorSlot = DoctorSlotEndPoint + "/{id}/active";
            public const string UnActiveDoctorSlot = DoctorSlotEndPoint + "/{id}/unactive";
            public const string GetAllDoctorSlotForUser = DoctorSlotEndPoint + "/{id}/user";
        }

        public static class Booking
        {
            public const string BookingEndPoint = ApiEndpoint + "/booking";
            public const string CreateBooking = BookingEndPoint;
            public const string GetBookingById = BookingEndPoint + "/{id}";
            public const string GetAllBookingForAdmin = BookingEndPoint + "/admin";
            public const string GetAllBookingForDoctor = BookingEndPoint + "/doctor";
            public const string GetAllBookingForUser = BookingEndPoint + "/user";
            public const string UpdateStatusBooking = BookingEndPoint + "/{id}";
        }

        public static class Feedback
        {
            public const string FeedbackEndPoint = ApiEndpoint + "/feedback";
            public const string CreateFeedback = FeedbackEndPoint;
        }

        public static class ProductCategory
        {
            public const string ProductCategoryEndPoint = ApiEndpoint + "/product-category";
            public const string CreateProductCategory = ProductCategoryEndPoint;
            public const string GetAllProductCategory = ProductCategoryEndPoint;
            public const string GetProductCategoryById = ProductCategoryEndPoint + "/{id}";
            public const string UpdateProductCategory = ProductCategoryEndPoint + "/{id}";
            public const string DeleteProductCategory = ProductCategoryEndPoint + "/{id}";
            public const string GetAllProductByCategory = ProductCategoryEndPoint + "/{id}/products";
        }

        public static class Product
        {
            public const string ProductEndPoint = ApiEndpoint + "/product";
            public const string CreateProduct = ProductEndPoint;
            public const string GetAllProduct = ProductEndPoint;
            public const string GetProductById = ProductEndPoint + "/{id}";
            public const string UpdateProduct = ProductEndPoint + "/{id}";
            public const string DeleteProduct = ProductEndPoint + "/{id}";
        }
    }
}
