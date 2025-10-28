using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoEntityFramework.AspNetCore.Identity.Tests.TestClasses;
using AwesomeAssertions;
using Xunit;

namespace MongoEntityFramework.AspNetCore.Identity.Tests.MongoUserOnlyStoreTests
{
    public class GetRoles : TestBase, IAsyncLifetime
    {

        public GetRoles() : base("MongoUserStore-GetRoles") { }

        public async Task InitializeAsync()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);

            context.Roles.Add(new MongoIdentityRole { Id = "rid1", Name = "Role 1" });
            context.Roles.Add(new MongoIdentityRole { Id = "rid2", Name = "Role 2" });
            context.Roles.Add(new MongoIdentityRole { Id = "rid3", Name = "Role 3" });

            await context.SaveChangesAsync();

            var user = MongoTestUser.First;
            user.Roles.Add("rid1");
            user.Roles.Add("rid2");
            await store.CreateAsync(user);

        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetRolesWithUser()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1);

            var roles = await store.GetRolesAsync(user);

            roles.Count.Should().Be(2);
            roles[0].Should().Be("Role 1");
        }

        [Fact]
        public async Task ThrowsExceptionWithNullArguments()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);

            var act = async () =>
            {
                await store.GetRolesAsync(null);
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
