using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoEntityFramework.AspNetCore.Identity.Tests.TestClasses;
using AwesomeAssertions;
using Xunit;

namespace MongoEntityFramework.AspNetCore.Identity.Tests.MongoUserStoreTests
{
    public class FindByEmail : TestBase, IAsyncLifetime
    {

        public FindByEmail() : base("MongoUserStore-FindByEmail") { }

        public async Task InitializeAsync()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);

            await store.CreateAsync(MongoTestUser.First);
            await store.CreateAsync(MongoTestUser.Second);
            await store.CreateAsync(MongoTestUser.Third);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task FindsCorrectUserWithValidEmail()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);

            var result = await store.FindByEmailAsync("TEST3@TESTING.COM");

            result.Should().NotBeNull();
            result.UserName.Should().Be("User Name3");
        }

        [Fact]
        public async Task FindsTrackedEntityWithValidEmail()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);
            var tracked = await store.FindByIdAsync(TestIds.UserId2);
            tracked.CustomData = "updated";

            var result = await store.FindByEmailAsync("TEST2@TESTING.COM");

            result.Should().BeSameAs(tracked);
            result.CustomData.Should().Be("updated");
        }

        [Fact]
        public async Task ReturnsNullWithInvalidEmail()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);

            var result = await store.FindByEmailAsync("none");

            result.Should().BeNull();
        }

        [Fact]
        public async Task ThrowsExceptionWithNull()
        {
            var context = new MongoTestContext(GetConnection());
            var store = new MongoUserStore<MongoTestUser>(context);

            var act = async () =>
            {
                await store.FindByEmailAsync(null);
            };
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

    }
}
