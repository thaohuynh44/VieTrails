using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using VieTrails_Web.Models;
using VieTrails_Web.Service.IService;

namespace VieTrails_Web.Service
{
    public class NationalParkService : Service<NationalPark>, INationalParkService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public NationalParkService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
    }
}
