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
    public class GetLogins : TestBase, IAsyncLifetime
    {

        public GetLogins() : base("MongoUserOnlyStore-GetLogins") { }

        public async Task InitializeAsync()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);

            var user = TestUser.First;
            await store.CreateAsync(user);
            await store.AddLoginAsync(user, new UserLoginInfo("provider1", "provider-key", "Login Provider"));
            await store.AddLoginAsync(user, new UserLoginInfo("provider2", "provider-key", "Login Provider"));
            await store.UpdateAsync(user);

        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task RetrieveLoginsFromUser()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1);

            var logins = await store.GetLoginsAsync(user);

            logins.Count.Should().Be(2);
            logins[0].LoginProvider.Should().Be("provider1");
        }

        [Fact]
        public Task ThrowsExceptionWithNullArguments()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);

            var act = () =>
            {
                var _ = store.GetLoginsAsync(null);
            };
            act.Should().Throw<ArgumentNullException>();
        }

    }
}
