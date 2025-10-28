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
    public class SetToken : TestBase, IAsyncLifetime
    {

        public SetToken() : base("MongoUserOnlyStore-SetToken") { }

        public async Task InitializeAsync()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            await store.CreateAsync(MongoTestUser.First);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task UpdatesUserWithValidData()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1);

            await store.SetTokenAsync(user, "provider", "name", "token-value", default);

            user.Tokens.Count.Should().Be(1);
            user.Tokens[0].Value.Should().Be("token-value");
        }

        [Fact]
        public async Task SavesData()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1);

            await store.SetTokenAsync(user, "provider", "name", "token-value", default);

            await store.UpdateAsync(user);

            context = new MongoTestContext(GetConnection());
            store = new MongoUserOnlyStore<MongoTestUser>(context);
            user = await store.FindByIdAsync(TestIds.UserId1);

            user.Tokens.Count.Should().Be(1);
            user.Tokens[0].Value.Should().Be("token-value");
        }

        [Fact]
        public async Task ThrowsExceptionWithNullArguments()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var act = async () =>
            {
                await store.SetTokenAsync(null, "", "", "", default);
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
