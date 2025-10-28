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
    public class GetToken : TestBase, IAsyncLifetime
    {

        public GetToken() : base("MongoUserOnlyStore-GetToken") { }

        public async ValueTask InitializeAsync()
        {

            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var user = MongoTestUser.First;
            await store.CreateAsync(user);
            await store.SetTokenAsync(user, "provider1", "name1", "token-value1", default);
            await store.SetTokenAsync(user, "provider2", "name2", "token-value2", default);
            await store.UpdateAsync(user);
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task RetrieveLoginsFromUser()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1, TestContext.Current.CancellationToken);

            var token1 = await store.GetTokenAsync(user, "provider1", "name1", TestContext.Current.CancellationToken);
            var token2 = await store.GetTokenAsync(user, "provider2", "name2", TestContext.Current.CancellationToken);

            token1.Should().Be("token-value1");
            token2.Should().Be("token-value2");
        }

        [Fact]
        public async Task ThrowsExceptionWithNullArguments()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var act = async () =>
            {
                await store.GetTokenAsync(null, "", "", default);
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
