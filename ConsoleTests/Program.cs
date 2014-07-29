﻿using SharpBucket;
using SharpBucket.POCOs;

namespace ConsoleTests{
    internal class Program{
        private static string email;
        private static string password;
        private static string apiKey;
        private static string secretApiKey;
        private static string accountName;
        private static string repository;

        private static void Main(){
            var sharpBucket = new SharpBucketV1();

            // Do basic auth
            ReadTestDataBasic();
            sharpBucket.BasicAuthentication(email, password);

            // Or OAuth
            //ReadTestDataOauth();
            //sharpBucket.OAuthAuthentication(apiKey, secretApiKey);

            TestUserEndPoint(sharpBucket);
            TestIssuesEndPoint(sharpBucket);
            TestRepositoryEndPoint(sharpBucket);
            TestUsersEndPoint(sharpBucket);
        }

        private static void ReadTestDataOauth(){
            // Reads test data information from a file, you should structure it like this:
            // By default it reads from c:\
            // ApiKey:yourApiKey
            // SecretApiKey:yourSecretApiKey
            // AccountName:yourAccountName
            // Repository:testRepository
            var lines = System.IO.File.ReadAllLines("c:\\TestInformationOauth.txt");
            apiKey = lines[0].Split(':')[1];
            secretApiKey = lines[1].Split(':')[1];
            ReadAccoutNameAndRepository(lines);
        }

        private static void ReadTestDataBasic(){
            // Reads test data information from a file, you should structure it like this:
            // By default it reads from c:\
            // Username:yourUsername
            // Password:yourPassword
            // AccountName:yourAccountName
            // Repository:testRepository
            var lines = System.IO.File.ReadAllLines("c:\\TestInformation.txt");
            email = lines[0].Split(':')[1];
            password = lines[1].Split(':')[1];
            ReadAccoutNameAndRepository(lines);
        }

        private static void ReadAccoutNameAndRepository(string[] lines){
            accountName = lines[2].Split(':')[1];
            repository = lines[3].Split(':')[1];
        }

        private static void TestUserEndPoint(SharpBucketV1 sharpBucketV1){
            var userEP = sharpBucketV1.User();
            var info = userEP.GetInfo();
            var privileges = userEP.GetPrivileges();
            var follows = userEP.ListFollows();
            var userRepos = userEP.ListRepositories();
            var userReposOverview = userEP.RepositoriesOverview();
        }

        private static void TestIssuesEndPoint(SharpBucketV1 sharpBucketV1){
            var issuesEP = sharpBucketV1.Repository(accountName, repository).Issues();
            int ISSUE_ID = 13;

            // Issues
            var issues = issuesEP.ListIssues();
            var newIssue = new Issue{Title = "new title", Content = "new content", Status = "new", Priority = "trivial", Kind = "bug"};
            var newIssueResult = issuesEP.PostIssue(newIssue);
            var changedIssue = new Issue{Title = "Completely new Title", Content = "Hi!", Status = "new", Local_id = newIssueResult.Local_id};
            var changedIssueResult = issuesEP.PutIssue(changedIssue);
            issuesEP.DeleteIssue(changedIssueResult.Local_id);

            // Issue followers
            var issueFollowers = issuesEP.ListIssueFollowers(ISSUE_ID);

            // Issue comments

            var issueComments = issuesEP.ListIssueComments(ISSUE_ID);
            var firstComment = issueComments[0];
            var issueComment = issuesEP.GetIssueComment(ISSUE_ID, firstComment.Comment_id);
            var newComment = new Comment{Content = "This bug is really annoying!"};
            var newCommentResult = issuesEP.PostIssueComment(ISSUE_ID, newComment);
            var updateComment = new Comment{Comment_id = newComment.Comment_id, Content = "updated"};
            var updatedCommentRes = issuesEP.PutIssueComment(ISSUE_ID, newCommentResult.Comment_id, updateComment);
            issuesEP.DeleteIssueComment(13, updatedCommentRes.Comment_id);

            // Components
            var components = issuesEP.ListComponents();
            var newComponent = new Component{Name = "Awesome component"};
            var newComponentRes = issuesEP.PostComponent(newComponent);
            var component = components[0];
            component.Name = "New name";
            var updatedComponent = issuesEP.PutComponent(component);
            issuesEP.DeleteComponent(newComponentRes.Id);


            // Milestones
            var milestones = issuesEP.ListMilestones();
            var updateMilestone = milestones[0];
            var milestone = issuesEP.GetMilestone(updateMilestone.Id);
            var newMilestone = new Milestone{Name = "Awesome milestone"};
            var newMilestoneRes = issuesEP.PostMilestone(newMilestone);
            issuesEP.DeleteMilestone(newMilestoneRes.Id);
            updateMilestone.Name = "New name";
            var updatedMilestone = issuesEP.PutMilestone(updateMilestone);

            // Versions
            var versions = issuesEP.ListVersions();
            var updateVersion = versions[0];
            var version = issuesEP.GetVersion(updateVersion.Id);
            var newVersion = new Version{Name = "Completely new"};
            var newVersionRes = issuesEP.PostVersion(newVersion);
            newVersionRes.Name = "New name";
            var updatedversion = issuesEP.PutVersion(newVersionRes);
            issuesEP.DeleteVersion(updatedversion.Id);
        }

        private static void TestRepositoryEndPoint(SharpBucketV1 sharpBucketV1){
            var repositoryEP = sharpBucketV1.Repository(accountName, repository);
            var tags = repositoryEP.ListTags();
            var branches = repositoryEP.ListBranches();
            var mainBranch = repositoryEP.GetMainBranch();
            string WIKI_PAGE = "";
            var wiki = repositoryEP.GetWiki(WIKI_PAGE);
            var newPage = new Wiki{Data = "Hello to my new page"};
            var newWiki = repositoryEP.PostWiki(newPage, "Location");
            var changeSet = repositoryEP.ListChangeset();
            var change = changeSet.changesets[4];
            var smallerChangeSet = repositoryEP.ListChangeset(start: change.Node, limit: 5);
            var getChange = repositoryEP.GetChangeset(change.Node);
            var diffStats = repositoryEP.GetChangesetDiffstat(change.Node);

            var repoEvents = repositoryEP.ListEvents();
        }

        private static void TestUsersEndPoint(SharpBucketV1 sharpBucketV1){
            var usersEP = sharpBucketV1.Users(accountName);
            var userEvents = usersEP.ListUserEvents();
            var userPrivileges = usersEP.ListUserPrivileges();
            var invitations = usersEP.ListInvitations();
            var invitationsForEmail = usersEP.GetInvitationsFor(email);
            var followers = usersEP.ListFollowers();
            var consumers = usersEP.ListConsumers();
            string CONSUMER_ID = "";
            var consumer = usersEP.ListConsumer(CONSUMER_ID);
            var ssh_keys = usersEP.ListSSHKeys();
            string SSH_ID = "";
            var getSSH = usersEP.GetSSHKey(SSH_ID);
        }
    }
}