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

        public async ValueTask InitializeAsync()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            await store.CreateAsync(MongoTestUser.First);
            await store.CreateAsync(MongoTestUser.Second);
            await store.CreateAsync(MongoTestUser.Third);
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task UpdatesUser()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1, TestContext.Current.CancellationToken);

            await store.AddLoginAsync(user, new UserLoginInfo("provider1", "provider-key", "Login Provider"), TestContext.Current.CancellationToken);

            user.Logins.Count.Should().Be(1);
            user.Logins[0].LoginProvider.Should().Be("provider1");
        }

        [Fact]
        public async Task SavesData()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1, TestContext.Current.CancellationToken);

            await store.AddLoginAsync(user, new UserLoginInfo("provider1", "provider-key", "Login Provider"), TestContext.Current.CancellationToken);
            await store.UpdateAsync(user, TestContext.Current.CancellationToken);

            context = new MongoTestContext(GetConnection());
            store = new MongoUserOnlyStore<MongoTestUser>(context);
            user = await store.FindByIdAsync(TestIds.UserId1, TestContext.Current.CancellationToken);

            user.Logins.Count.Should().Be(1);
            user.Logins[0].LoginProvider.Should().Be("provider1");
        }

        [Fact]
        public async Task ThrowsExceptionWithNullArguments()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = new MongoTestUser();

            var act = async () =>
            {
                await store.AddLoginAsync(null, new UserLoginInfo("", "", ""));
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
            var act1 = async () =>
            {
                await store.AddLoginAsync(user, null);
            };
            await act1.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
