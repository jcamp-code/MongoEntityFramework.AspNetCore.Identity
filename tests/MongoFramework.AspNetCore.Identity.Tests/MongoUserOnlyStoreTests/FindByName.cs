using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoEntityFramework.AspNetCore.Identity.Tests.TestClasses;
using AwesomeAssertions;
using Xunit;

namespace MongoEntityFramework.AspNetCore.Identity.Tests.MongoUserOnlyStoreTests
{
    public class FindByName : TestBase, IAsyncLifetime
    {

        public FindByName() : base("MongoUserOnlyStore-FindByName") { }

        public async Task InitializeAsync()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);

            await store.CreateAsync(TestUser.First);
            await store.CreateAsync(TestUser.Second);
            await store.CreateAsync(TestUser.Third);
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task FindsCorrectUserWithValidUserName()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);

            var result = await store.FindByNameAsync("USER NAME2");

            result.Should().NotBeNull();
            result.UserName.Should().Be("User Name2");
        }


        [Fact]
        public async Task FindsTrackedEntityWithValidUserName()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);
            var tracked = await store.FindByIdAsync(TestIds.UserId2);
            tracked.CustomData = "updated";

            var result = await store.FindByNameAsync("USER NAME2");

            result.Should().BeSameAs(tracked);
            result.CustomData.Should().Be("updated");
        }


        [Fact]
        public async Task ReturnsNullWithInvalidUserName()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);

            var result = await store.FindByNameAsync("none");

            result.Should().BeNull();
        }

        [Fact]
        public Task ThrowsExceptionWithNull()
        {
            var context = new TestContext(GetConnection());
            var store = new MongoUserOnlyStore<TestUser>(context);

            var act = () =>
            {
                var _ = store.FindByNameAsync(null);
            };
            act.Should().Throw<ArgumentNullException>();
        }

    }
}
