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

        public async ValueTask InitializeAsync()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var user = MongoTestUser.First;
            await store.CreateAsync(user);
            await store.AddLoginAsync(user, new UserLoginInfo("provider1", "provider-key", "Login Provider"));
            await store.AddLoginAsync(user, new UserLoginInfo("provider2", "provider-key", "Login Provider"));
            await store.UpdateAsync(user);

        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task RetrieveLoginsFromUser()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1);

            var logins = await store.GetLoginsAsync(user);

            logins.Count.Should().Be(2);
            logins[0].LoginProvider.Should().Be("provider1");
        }

        [Fact]
        public async Task ThrowsExceptionWithNullArguments()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var act = async () =>
            {
                await store.GetLoginsAsync(null);
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
