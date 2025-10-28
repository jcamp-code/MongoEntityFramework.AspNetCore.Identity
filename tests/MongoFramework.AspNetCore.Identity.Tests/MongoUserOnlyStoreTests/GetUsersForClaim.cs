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
    public class GetUsersForClaim : TestBase, IAsyncLifetime
    {

        public GetUsersForClaim() : base("MongoUserOnlyStore-GetUsersForClaim") { }

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
            user = MongoTestUser.Second;
            await store.CreateAsync(user);
            await store.AddClaimsAsync(user,
                new[]
                {
                    new Claim("type","value"),
                    new Claim("type2", "value2")
                });
            await store.UpdateAsync(user);
            user = MongoTestUser.Third;
            await store.CreateAsync(user);
            await store.AddClaimsAsync(user,
                new[]
                {
                    new Claim("type","value"),
                    new Claim("type3", "value3")
                });
            await store.UpdateAsync(user);

        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task RetrieveUsersForClaim()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var users = await store.GetUsersForClaimAsync(new Claim("type", "value"));
            var users2 = await store.GetUsersForClaimAsync(new Claim("type2", "value2"));

            users.Count.Should().Be(3);
            users2.Count.Should().Be(2);

        }

        [Fact]
        public async Task ThrowsExceptionWithNullArguments()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserOnlyStore<MongoTestUser>(context);

            var act = async () =>
            {
                await store.GetClaimsAsync(null);
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
