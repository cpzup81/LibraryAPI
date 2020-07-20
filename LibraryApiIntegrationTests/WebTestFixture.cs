using LibraryApi;
using LibraryApi.Domain;
using LibraryApi.Services;
using LibraryApiIntegrationTests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryApiIntegrationTests
{
    public class WebTestFixture : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // todo
                // Check to see if ISystemTime is set up as a service.
                var systemTimeDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ISystemTime));

                if (systemTimeDescriptor != null)
                {
                    services.Remove(systemTimeDescriptor);
                    services.AddTransient<ISystemTime, FakeSystemTime>();
                }

                var dbContextOptionsDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<LibraryDataContext>)
                    );

                if (dbContextOptionsDescriptor != null)
                {
                    services.Remove(dbContextOptionsDescriptor);
                    services.AddDbContext<LibraryDataContext>(options =>
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString())
                    );
                }

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();

                var scopedServices = scope.ServiceProvider;

                var db = scopedServices.GetRequiredService<LibraryDataContext>();
                db.Database.EnsureCreated();


                db.Books.AddRange(
                        new Book { Title = "Jaws", Author = "Benchely", Genre = "Horror", NumberOfPages = 289, InStock = true },
                        new Book { Title = "Fight Club", Author = "Poluiuck", Genre = "Fiction", NumberOfPages = 289, InStock = true },
                        new Book { Title = "Jaws", Author = "Benchely", Genre = "Horror", NumberOfPages = 289, InStock = false }
                    );
                db.SaveChanges();
            });
        }
    }
}
