using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using AwesomeAssertions;
using Xunit;

namespace MongoEntityFramework.AspNetCore.Identity.Tests
{
    public class MongoIdentityUserTests
    {
        [Fact]
        public void ConstructorCreatesEmptyLists()
        {
            var user = new MongoIdentityUser();

            user.Claims.Should().NotBeNull();
            user.Roles.Should().NotBeNull();
            user.Tokens.Should().NotBeNull();
            user.Logins.Should().NotBeNull();

            user.Claims.Count.Should().Be(0);
            user.Roles.Count.Should().Be(0);
            user.Tokens.Count.Should().Be(0);
            user.Logins.Count.Should().Be(0);
        }

        [Fact]
        public void ConstructorSucceedsWithValidUserName()
        {
            var user = new MongoIdentityUser("username");

            user.UserName.Should().Be("username");
            user.NormalizedUserName.Should().Be("USERNAME");
        }

        [Fact]
        public void ConstructorThrowsExceptionWithNullUserName()
        {
            Action action = () =>
            {
                _ = new MongoIdentityUser(null);
            };
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void KeyTypeIsUsedInCollectionsWithInteger()
        {
            var user = new MongoIdentityUser<int>();

            user.Claims.Should().BeOfType<List<IdentityUserClaim<int>>>();
            user.Tokens.Should().BeOfType<List<IdentityUserToken<int>>>();
            user.Logins.Should().BeOfType<List<IdentityUserLogin<int>>>();
        }

        [Fact]
        public void KeyTypeIsUsedInCollectionsWithGuid()
        {
            var user = new MongoIdentityUser<Guid>();

            user.Claims.Should().BeOfType<List<IdentityUserClaim<Guid>>>();
            user.Tokens.Should().BeOfType<List<IdentityUserToken<Guid>>>();
            user.Logins.Should().BeOfType<List<IdentityUserLogin<Guid>>>();
        }



    }
}
