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
    public class RemoveClaim : TestBase, IAsyncLifetime
    {

        public RemoveClaim() : base("MongoUserOnlyStore-RemoveClaim") { }

        public async ValueTask InitializeAsync()
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

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task RemoveClaimWithExistingClaim()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1, TestContext.Current.CancellationToken);

            var claims = await store.GetClaimsAsync(user, TestContext.Current.CancellationToken);

            await store.RemoveClaimsAsync(user, claims, TestContext.Current.CancellationToken);

            user.Claims.Count.Should().Be(0);
        }

        [Fact]
        public async Task RemoveClaimWithNewClaim()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1, TestContext.Current.CancellationToken);

            await store.RemoveClaimsAsync(user, new[] {
                new Claim("type","value"),
                new Claim("type2", "value2")
            }, TestContext.Current.CancellationToken);

            user.Claims.Count.Should().Be(0);
        }

        [Fact]
        public async Task SavesData()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = await store.FindByIdAsync(TestIds.UserId1, TestContext.Current.CancellationToken);

            var claims = await store.GetClaimsAsync(user, TestContext.Current.CancellationToken);

            await store.RemoveClaimsAsync(user, claims, TestContext.Current.CancellationToken);
            await store.UpdateAsync(user, TestContext.Current.CancellationToken);

            context = new MongoTestContext(GetConnection());
            store = new MongoUserOnlyStore<MongoTestUser>(context);
            user = await store.FindByIdAsync(TestIds.UserId1, TestContext.Current.CancellationToken);

            user.Claims.Count.Should().Be(0);
        }

        [Fact]
        public async Task ThrowsExceptionWithNullArguments()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);
            var user = new MongoTestUser();

            var act = async () =>
            {
                await store.RemoveClaimsAsync(null, new[] { new Claim("type", "value") });
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
            var act1 = async () =>
            {
                await store.RemoveClaimsAsync(user, null);
            };
            await act1.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
