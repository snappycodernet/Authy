using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authy.Common.Enums
{
    public class EnvironmentVariableEnum
    {
        public const string DATABASE_CONNECTION_STRING = "DATABASE_CONNECTION_STRING";
        public const string DATABASE_CONNECTION_STRING_PROD = "DATABASE_CONNECTION_STRING_PROD";
        public const string SERVICE_STACK_LICENSE = "SERVICE_STACK_LICENSE";
        public const string COMMA_SEPARATED_CORS_WHITELIST = "COMMA_SEPARATED_CORS_WHITELIST";
        public const string APPLICATIONINSIGHTS_CONNECTION_STRING = "APPLICATIONINSIGHTS_CONNECTION_STRING";
    }
}
