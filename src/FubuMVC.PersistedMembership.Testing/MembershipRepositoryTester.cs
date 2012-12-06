﻿using FubuMVC.Authentication;
using FubuPersistence;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.PersistedMembership.Testing
{
    [TestFixture]
    public class MembershipRepositoryTester
    {
        private EntityRepository theRepository;
        private PasswordHash theHash;
        private MembershipRepository<User> theMembership;

        [SetUp]
        public void SetUp()
        {
            theRepository = EntityRepository.InMemory();
            theHash = new PasswordHash();

            theMembership = new MembershipRepository<User>(theRepository, theHash);
        }

        [Test]
        public void matches_credentials_postive()
        {
            var user1 = new User
            {
                UserName = "jeremy",
                Password = theHash.CreateHash("something")
            };

            var user2 = new User
            {
                UserName = "josh",
                Password = theHash.CreateHash("else")
            };
            
            theRepository.Update(user1);
            theRepository.Update(user2);


            theMembership.MatchesCredentials(new LoginRequest
            {
                UserName = "jeremy",
                Password = "wrong"
            }).ShouldBeFalse();

            theMembership.MatchesCredentials(new LoginRequest
            {
                UserName = "jeremy",
                Password = "else"
            }).ShouldBeFalse();

            theMembership.MatchesCredentials(new LoginRequest
            {
                UserName = "josh",
                Password = "something"
            }).ShouldBeFalse();
        }

        [Test]
        public void matches_credentials_negative()
        {
            var user1 = new User
            {
                UserName = "jeremy",
                Password = theHash.CreateHash("something")
            };

            var user2 = new User
            {
                UserName = "josh",
                Password = theHash.CreateHash("else")
            };

            theRepository.Update(user1);
            theRepository.Update(user2);


            theMembership.MatchesCredentials(new LoginRequest
            {
                UserName = "jeremy",
                Password = "something"
            }).ShouldBeTrue();

            theMembership.MatchesCredentials(new LoginRequest
            {
                UserName = "josh",
                Password = "else"
            }).ShouldBeTrue();
        }

        [Test]
        public void find_by_name()
        {
            var user1 = new User
            {
                UserName = "jeremy",
                Password = theHash.CreateHash("something")
            };

            var user2 = new User
            {
                UserName = "josh",
                Password = theHash.CreateHash("else")
            };

            theRepository.Update(user1);
            theRepository.Update(user2);

            theMembership.FindByName("jeremy").ShouldBeTheSameAs(user1);
            theMembership.FindByName("josh").ShouldBeTheSameAs(user2);
        }
    }
}