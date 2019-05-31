using System;
using Microsoft.AspNetCore.Builder;

namespace ShoppingService.Api.Configuration {
    public static class ApplicationConfiguration {
        public static void Configure(IApplicationBuilder app) {
            app.UseMvc();
        }
    }
}
