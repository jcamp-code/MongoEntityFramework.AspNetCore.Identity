using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using AwesomeAssertions;
using Xunit;

namespace MongoEntityFramework.AspNetCore.Identity.Tests
{
    public class MongoIdentityRoleTests
    {
        [Fact]
        public void ConstructorCreatesEmptyLists()
        {
            var role = new MongoIdentityRole();

            role.Claims.Should().NotBeNull();
            role.Claims.Count.Should().Be(0);
        }

        [Fact]
        public void ConstructorSucceedsWithValidUserName()
        {
            var role = new MongoIdentityRole("role-name");

            role.Name.Should().Be("role-name");
            role.NormalizedName.Should().Be("ROLE-NAME");
            role.ToString().Should().Be("role-name");
        }

        [Fact]
        public void ConstructorThrowsExceptionWithNullUserName()
        {
            Action action = () =>
            {
                _ = new MongoIdentityRole(null);
            };
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void KeyTypeIsUsedInCollectionsWithInteger()
        {
            var role = new MongoIdentityRole<int>();

            role.Claims.Should().BeOfType<List<IdentityRoleClaim<int>>>();
        }

        [Fact]
        public void KeyTypeIsUsedInCollectionsWithGuid()
        {
            var role = new MongoIdentityRole<Guid>();

            role.Claims.Should().BeOfType<List<IdentityRoleClaim<Guid>>>();
        }

    }
}
