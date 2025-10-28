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
    public class RemoveToken : TestBase, IAsyncLifetime
    {

        public RemoveToken() : base("MongoUserOnlyStore-RemoveToken") { }

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
        public async Task RemovesToken()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1);

            await store.RemoveTokenAsync(user, "provider2", "name2", default);

            user.Tokens.Count.Should().Be(1);
        }

        [Fact]
        public async Task SavesData()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1);

            await store.RemoveTokenAsync(user, "provider2", "name2", default);

            await store.UpdateAsync(user);

            context = new MongoTestContext(GetConnection());
            store = new MongoUserOnlyStore<MongoTestUser>(context);

            user = await store.FindByIdAsync(TestIds.UserId1);

            user.Tokens.Count.Should().Be(1);
        }

        [Fact]
        public async Task ThrowsExceptionWithNullArguments()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var act = async () =>
            {
                await store.RemoveTokenAsync(null, "", "", default);
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
