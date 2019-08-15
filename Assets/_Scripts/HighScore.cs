using System;
using System.Collections.Generic;
using System.IO;
using LibGit2Sharp;
using System.Linq;

namespace HighScore
{
    public class HighScore
    {
        private readonly string UserName = String.Empty;
        private readonly string Token = String.Empty;
        private readonly string RepoURL = String.Empty;
        private readonly string HighScorePath = String.Empty;
        private readonly string BranchName = String.Empty;

        public void Init()
        {
            if (Directory.Exists(HighScorePath))
            {
                try
                {
                    CheckoutBranch();
                    ResetRepo();
                    UpdateRepo();
                    return;
                }
                catch (Exception)
                {
                    Directory.Delete(HighScorePath, true);
                }
            }

            CloneRepo();
        }

        private void CloneRepo()
        {
            var co = new CloneOptions
            {
                CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                {
                    Username = UserName,
                    Password = Token
                }
            };
            Repository.Clone(RepoURL, HighScorePath, co);

            CheckoutBranch();
        }

        private void CheckoutBranch()
        {
            using (var repo = new Repository(HighScorePath))
            {
                var localBranch = repo.Branches[BranchName];
                if (localBranch == null)
                {
                    var remoteBranch = repo.Branches["origin/" + BranchName];
                    localBranch = repo.CreateBranch(BranchName, remoteBranch.Tip);

                    repo.Branches.Update(localBranch, b =>
                    {
                        b.UpstreamBranch = "refs/heads/" + BranchName;
                        b.Remote = "origin";
                    });
                }

                if (repo.Head.CanonicalName != repo.Branches[BranchName].CanonicalName)
                {
                    Commands.Checkout(repo, repo.Branches[BranchName]);
                }
            }
        }

        private void UpdateRepo()
        {
            using (var repo = new Repository(HighScorePath))
            {
                // Credential information to fetch
                var options = new PullOptions
                {
                    FetchOptions = new FetchOptions
                    {
                        CredentialsProvider = (url, usernameFromUrl, types) =>
                            new UsernamePasswordCredentials { Username = UserName, Password = Token }
                    }
                };

                // User information to create a merge commit
                var signature = new Signature(
                    new Identity(UserName, "@none"), DateTimeOffset.Now);

                // Pull
                var result = Commands.Pull(repo, signature, options);
                if (result.Status == MergeStatus.Conflicts)
                {
                    CheckoutBranch();
                    ResetRepo();
                    UpdateRepo();
                }
            }
        }

        private void ResetRepo()
        {
            using (var repo = new Repository(HighScorePath))
            {
                var origin = repo.Branches["origin/" + BranchName];
                repo.Reset(ResetMode.Hard, origin.Tip);
            }
        }

        private void CommitToRepo(string file, string message)
        {
            using (var repo = new Repository(HighScorePath))
            {
                repo.Index.Add(file);
                repo.Index.Write();

                var author = new Signature(UserName, "@none", DateTime.Now);
                var committer = author;

                repo.Commit(message, author, committer);
            }
        }

        private void PushToRepo()
        {
            using (var repo = new Repository(HighScorePath))
            {
                var options = new PushOptions
                {
                    CredentialsProvider = (url, usernameFromUrl, types) =>
                        new UsernamePasswordCredentials { Username = UserName, Password = Token }
                };
                repo.Network.Push(repo.Branches[BranchName], options);
            }
        }

        public void AddHighScoreToSong(string songHash, string userName, string difficulty, long score)
        {
            if (!Directory.Exists(Path.Combine(HighScorePath, songHash)))
            {
                Directory.CreateDirectory(Path.Combine(HighScorePath, songHash));
            }

            var existingHighScores = GetHighScoreOfSong(songHash, difficulty);

            var found = false;

            var removed = existingHighScores.RemoveAll(s =>
            {
                if (!s.Username.Equals(userName))
                {
                    return false;
                }

                found = true;

                return s.Score < score;
            });

            // HighScore of this user is already higher
            if (found && removed == 0)
            {
                return;
            }

            existingHighScores.Add(new HighScoreEntry { Username = userName, Score = score });
            File.WriteAllLines(Path.Combine(HighScorePath, songHash) + "/" + difficulty, existingHighScores.Select(e => e.ToString()).ToArray());

            CommitToRepo(songHash + "/" + difficulty, $"{songHash}: {userName} with {score} on {difficulty}");

            try
            {
                PushToRepo();
            }
            catch (NonFastForwardException)
            {
                CheckoutBranch();
                UpdateRepo();
                ResetRepo();
                AddHighScoreToSong(songHash, userName, difficulty, score);
            }
        }

        public List<HighScoreEntry> GetHighScoreOfSong(string songHash, string difficulty)
        {
            CheckoutBranch();
            UpdateRepo();

            if (!Directory.Exists(Path.Combine(HighScorePath, songHash)) || !File.Exists(Path.Combine(HighScorePath, songHash) + "/" + difficulty))
            {
                return new List<HighScoreEntry>();
            }

            return File.ReadAllLines(Path.Combine(HighScorePath, songHash) + "/" + difficulty).Select(entry => new HighScoreEntry(entry)).ToList();
        }

        public List<HighScoreEntry> GetFirstTenHighScoreOfSong(string songHash, string difficulty)
        {
            var completeHighscore = GetHighScoreOfSong(songHash, difficulty);
            if (completeHighscore.Count > 0)
            {
                return completeHighscore.OrderBy(h => h.Score).Take(10).ToList();
            }
            else
            {
                return completeHighscore;
            }
        }
    }

    public class HighScoreLocal
    {
        private readonly string HighScorePath = "./localhighscore";

        public void Init()
        {
            if (!Directory.Exists(Path.Combine(HighScorePath)))
            {
                Directory.CreateDirectory(Path.Combine(HighScorePath));
            }
        }

        public void AddHighScoreToSong(string songHash, string userName, string difficulty, long score)
        {
            if (!Directory.Exists(Path.Combine(HighScorePath, songHash)))
            {
                Directory.CreateDirectory(Path.Combine(HighScorePath, songHash));
            }

            var existingHighScores = GetHighScoreOfSong(songHash, difficulty);

            existingHighScores.Add(new HighScoreEntry { Username = userName, Score = score });
            File.WriteAllLines(Path.Combine(HighScorePath, songHash) + "/" + difficulty, existingHighScores.Select(e => e.ToString()).ToArray());
        }

        public List<HighScoreEntry> GetHighScoreOfSong(string songHash, string difficulty)
        {
            if (!Directory.Exists(Path.Combine(HighScorePath, songHash)) || !File.Exists(Path.Combine(HighScorePath, songHash) + "/" + difficulty))
            {
                return new List<HighScoreEntry>();
            }

            return File.ReadAllLines(Path.Combine(HighScorePath, songHash) + "/" + difficulty).Select(entry => new HighScoreEntry(entry)).ToList();
        }

        public List<HighScoreEntry> GetFirstTenHighScoreOfSong(string songHash, string difficulty)
        {
            var completeHighscore = GetHighScoreOfSong(songHash, difficulty);
            if (completeHighscore.Count > 0)
            {
                return completeHighscore.OrderBy(h => h.Score).Take(10).ToList();
            }
            else
            {
                return completeHighscore;
            }
        }
    }

    public class HighScoreEntry
    {
        public string Username;
        public long Score;

        public HighScoreEntry(string entry)
        {
            Username = entry.Split(new string[] { "##" }, StringSplitOptions.RemoveEmptyEntries)[0];
            Score = Convert.ToInt64(entry.Split(new string[] { "##" }, StringSplitOptions.RemoveEmptyEntries)[1]);
        }

        public HighScoreEntry()
        {

        }

        public override string ToString()
        {
            return Username + "##" + Score;
        }
    }
}
