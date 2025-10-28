using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoEntityFramework.AspNetCore.Identity.Tests.TestClasses;
using AwesomeAssertions;
using Xunit;

namespace MongoEntityFramework.AspNetCore.Identity.Tests.MongoUserStoreTests
{
    public class FindUserRole : TestBase, IAsyncLifetime
    {
        private class TestStore : MongoUserStore<MongoTestUser>
        {
            public TestStore(DbContext context, IdentityErrorDescriber describer = null) : base(context, describer) { }

            public async Task<IdentityUserRole<string>> ExposeFindUserRoleAsync(string userId, string roleId)
            {
                return await base.FindUserRoleAsync(userId, roleId, default);
            }
        }

        public FindUserRole() : base("MongoUserStore-FindUserRole") { }

        public async ValueTask InitializeAsync()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);

            context.Roles.Add(new MongoIdentityRole { Id = TestIds.RoleId1, Name = "Role 1" });
            context.Roles.Add(new MongoIdentityRole { Id = TestIds.RoleId2, Name = "Role 2" });
            context.Roles.Add(new MongoIdentityRole { Id = TestIds.RoleId3, Name = "Role 3" });

            await context.SaveChangesAsync();

            var user = MongoTestUser.First;
            user.Roles.Add(TestIds.RoleId1);
            user.Roles.Add(TestIds.RoleId2);
            await store.CreateAsync(user);

        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task FindUserRoleWithValidRole()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new TestStore(context);

            var role = await store.ExposeFindUserRoleAsync(TestIds.UserId1, TestIds.RoleId1);

            role.Should().NotBeNull();
            role.RoleId.Should().Be(TestIds.RoleId1);
            role.UserId.Should().Be(TestIds.UserId1);        }

        [Fact]
        public async Task FindUserRoleFailsWithInvalidRole()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new TestStore(context);

            var role = await store.ExposeFindUserRoleAsync("a1", "none-rid1");

            role.Should().BeNull();
        }

        [Fact]
        public async Task FindUserRoleFailsWithInvaliUser()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new TestStore(context);

            var role = await store.ExposeFindUserRoleAsync("none-a1", "rid1");

            role.Should().BeNull();
        }

        [Fact]
        public async Task ThrowsExceptionWithNullArguments()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new TestStore(context);

            var act = async () =>
            {
                await store.ExposeFindUserRoleAsync(null, "rid1");
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
            var act1 = async () =>
            {
                await store.ExposeFindUserRoleAsync("a1", null);
            };
            await act1.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
