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
            public const string UpdateFetus = FetusEndPoint + "/{fetusId}";
            public const string DeleteFetus = FetusEndPoint + "/{id}";
        }
    }
}
