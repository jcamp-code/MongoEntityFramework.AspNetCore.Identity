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
    public class ReplaceClaim : TestBase, IAsyncLifetime
    {

        public ReplaceClaim() : base("MongoUserOnlyStore-ReplaceClaim") { }

        public async Task InitializeAsync()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var user = MongoTestUser.First;
            await store.CreateAsync(user);
            await store.AddClaimsAsync(user,
                new[]
                {
                    new Claim("type","value"),
                    new Claim("type2", "value2")
                });
            await store.UpdateAsync(user);

        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task ReplaceUsersClaim()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1);

            var claims = await store.GetClaimsAsync(user);
            var claim = claims[0];

            await store.ReplaceClaimAsync(user, claim, new Claim("new-type", "new-value"));

            user.Claims[0].ClaimType.Should().Be("new-type");
            user.Claims[0].ClaimValue.Should().Be("new-value");
        }

        [Fact]
        public async Task SavesData()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1);

            var claims = await store.GetClaimsAsync(user);
            var claim = claims[0];

            await store.ReplaceClaimAsync(user, claim, new Claim("new-type", "new-value"));

            await store.UpdateAsync(user);

            context = new MongoTestContext(GetConnection());
            store = new MongoUserOnlyStore<MongoTestUser>(context);
            user = await store.FindByIdAsync(TestIds.UserId1);

            user.Claims.Count.Should().Be(2);
            user.Claims[0].ClaimType.Should().Be("new-type");
        }

        [Fact]
        public async Task ThrowsExceptionWithNullArguments()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = new MongoTestUser();

            var act = async () =>
            {
                await store.ReplaceClaimAsync(null, new Claim("type", "value"), new Claim("type", "value"));
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
            var act1 = async () =>
            {
                await store.ReplaceClaimAsync(user, null, new Claim("type", "value"));
            };
            await act1.Should().ThrowAsync<ArgumentNullException>();
            var act2 = async () =>
            {
                await store.ReplaceClaimAsync(user, new Claim("type", "value"), null);
            };
            await act2.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
