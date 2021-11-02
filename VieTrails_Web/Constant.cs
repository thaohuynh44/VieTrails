using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VieTrails_Web
{
    public static class Constant
    {
        public static string APIBaseUrl = "https://localhost:44332/";
        public static string NationalParkAPIPath = APIBaseUrl + "api/v1/nationalparks/";
        public static string TrailAPIPath = APIBaseUrl + "api/v1/trails/";
        public static string AccountAPIPath = APIBaseUrl + "api/v1/users/";
    }
}
