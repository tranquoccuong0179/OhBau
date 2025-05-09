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

    }
}
