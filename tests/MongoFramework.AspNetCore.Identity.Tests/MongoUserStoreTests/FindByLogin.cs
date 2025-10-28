using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoEntityFramework.AspNetCore.Identity.Tests.TestClasses;
using AwesomeAssertions;
using Xunit;

namespace MongoEntityFramework.AspNetCore.Identity.Tests.MongoUserStoreTests
{
    public class FindByLogin : TestBase, IAsyncLifetime
    {

        private class TestStore : MongoUserStore<MongoTestUser>
        {
            public TestStore(DbContext context, IdentityErrorDescriber describer = null) : base(context, describer) { }

            public async Task<IdentityUserLogin<string>> ExposeFindUserLoginAsync(string userId, string loginProvider, string providerKey)
            {
                return await base.FindUserLoginAsync(userId, loginProvider, providerKey, default);
            }
        }

        public FindByLogin() : base("MongoUserStore-FindByLogin") { }

        public async ValueTask InitializeAsync()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);

            var user = MongoTestUser.First;
            await store.CreateAsync(user);
            await store.AddLoginAsync(user, new UserLoginInfo("provider1", "provider-key", "Login Provider"));
            await store.AddLoginAsync(user, new UserLoginInfo("provider2", "provider-key", "Login Provider"));
            await store.UpdateAsync(user);

            var user2 = MongoTestUser.Second;
            await store.CreateAsync(user2);
            await store.AddLoginAsync(user2, new UserLoginInfo("provider3", "provider-key", "Login Provider"));
            await store.AddLoginAsync(user2, new UserLoginInfo("provider4", "provider-key", "Login Provider"));
            await store.UpdateAsync(user2);

        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task GetsCorrectUserFromLogin()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);

            var user = await store.FindByLoginAsync("provider3", "provider-key", TestContext.Current.CancellationToken);

            user.Should().NotBeNull();
            user.Id.Should().Be(TestIds.UserId2);
        }

        [Fact]
        public async Task GetsLoginWithUserIdAndProvider()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new TestStore(context);

            var login = await store.ExposeFindUserLoginAsync(TestIds.UserId1, "provider2", "provider-key");

            login.Should().NotBeNull();
            login.UserId.Should().Be(TestIds.UserId1);
            login.LoginProvider.Should().Be("provider2");
        }

        [Fact]
        public async Task ReturnsNullFromNonExisting()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);

            var user = await store.FindByLoginAsync("provider5", "provider-key", TestContext.Current.CancellationToken);

            user.Should().BeNull();
        }

        [Fact]
        public async Task ReturnsNullFromNull()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);

            var user = await store.FindByLoginAsync(null, "provider-key", TestContext.Current.CancellationToken);

            user.Should().BeNull();
        }

    }
}
