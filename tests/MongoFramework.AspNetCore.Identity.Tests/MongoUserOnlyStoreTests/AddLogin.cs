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
    public class AddLogin : TestBase, IAsyncLifetime
    {

        public AddLogin() : base("MongoUserOnlyStore-AddLogin") { }

        public async Task InitializeAsync()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);

            await store.CreateAsync(TestUser.First);
            await store.CreateAsync(TestUser.Second);
            await store.CreateAsync(TestUser.Third);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task UpdatesUser()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1);

            await store.AddLoginAsync(user, new UserLoginInfo("provider1", "provider-key", "Login Provider"));

            user.Logins.Count.Should().Be(1);
            user.Logins[0].LoginProvider.Should().Be("provider1");
        }

        [Fact]
        public async Task SavesData()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1);

            await store.AddLoginAsync(user, new UserLoginInfo("provider1", "provider-key", "Login Provider"));
            await store.UpdateAsync(user);

            context = new TestContext(GetConnection());
            store = new MongoUserOnlyStore<TestUser>(context);
            user = await store.FindByIdAsync(TestIds.UserId1);

            user.Logins.Count.Should().Be(1);
            user.Logins[0].LoginProvider.Should().Be("provider1");
        }

        [Fact]
        public Task ThrowsExceptionWithNullArguments()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);
            var user = new TestUser();

            var act = () =>
            {
                var _ = store.AddLoginAsync(null, new UserLoginInfo("", "", ""));
            };
            act.Should().Throw<ArgumentNullException>();
            var act1 = () =>
            {
                var _ = store.AddLoginAsync(user, null);
            };
            act1.Should().Throw<ArgumentNullException>();
        }

    }
}
