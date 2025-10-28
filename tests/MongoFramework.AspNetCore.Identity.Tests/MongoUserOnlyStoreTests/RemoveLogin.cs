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
    public class RemoveLogin : TestBase, IAsyncLifetime
    {

        public RemoveLogin() : base("MongoUserOnlyStore-RemoveLogin") { }

        public async ValueTask InitializeAsync()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var user = MongoTestUser.First;
            await store.CreateAsync(user);
            await store.AddLoginAsync(user, new UserLoginInfo("provider1", "provider-key", "Login Provider"));
            await store.UpdateAsync(user);

        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task RemoveLoginWithExistingLogin()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1, TestContext.Current.CancellationToken);

            await store.RemoveLoginAsync(user, "provider1", "provider-key", TestContext.Current.CancellationToken);

            user.Logins.Count.Should().Be(0);
        }

        [Fact]
        public async Task SavesData()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1, TestContext.Current.CancellationToken);

            await store.RemoveLoginAsync(user, "provider1", "provider-key", TestContext.Current.CancellationToken);
            await store.UpdateAsync(user, TestContext.Current.CancellationToken);

            context = new MongoTestContext(GetConnection());
            store = new MongoUserOnlyStore<MongoTestUser>(context);
            user = await store.FindByIdAsync(TestIds.UserId1, TestContext.Current.CancellationToken);

            user.Logins.Count.Should().Be(0);
        }

        [Fact]
        public async Task ThrowsExceptionWithNullArguments()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var act = async () =>
            {
                await store.RemoveLoginAsync(null, "", "");
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
