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
    public class AddClaims : TestBase, IAsyncLifetime
    {

        public AddClaims() : base("MongoUserOnlyStore-AddClaims") { }

        public async Task InitializeAsync()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            await store.CreateAsync(MongoTestUser.First);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task UpdatesUser()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1);

            await store.AddClaimsAsync(user,
                new[]
                {
                    new Claim("type","value"),
                    new Claim("type2", "value2")
                });

            user.Claims.Count.Should().Be(2);
            user.Claims[0].ClaimType.Should().Be("type");
        }

        [Fact]
        public async Task SavesData()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1);

            await store.AddClaimsAsync(user,
                new[]
                {
                    new Claim("type","value"),
                    new Claim("type2", "value2")
                });

            await store.UpdateAsync(user);

            context = new MongoTestContext(GetConnection());
            store = new MongoUserOnlyStore<MongoTestUser>(context);
            user = await store.FindByIdAsync(TestIds.UserId1);

            user.Claims.Count.Should().Be(2);
            user.Claims[0].ClaimType.Should().Be("type");
        }

        [Fact]
        public async Task ThrowsExceptionWithNullArguments()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = new MongoTestUser();

            var act = async () =>
            {
                await store.AddClaimsAsync(null, new[] { new Claim("type", "value") });
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
            var act1 = async () =>
            {
                await store.AddClaimsAsync(user, null);
            };
            await act1.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
