using System;

namespace AuthDemo.Security.Authorization
{
    public static class AuthPolicies
    { 
        public static class Roles
        {
            public const string Admin = "SuperAdmin";
            public const string CompanyUser = "CompanyUser";
            public const string CustomerUser = "CustomerUser";
        }
        
    }

   
}
