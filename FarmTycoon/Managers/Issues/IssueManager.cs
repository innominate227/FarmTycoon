using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{


    /// <summary>
    /// Manages issues that the player should be aware of, such as if a task cannot be started, or a worker cannot reach a destination
    /// </summary>
    public partial class IssueManager : ISavable
    {
        #region Events

        public event Action IssuesListChanged;

        #endregion

        #region Member Vars

        /// <summary>
        /// Dictionray of all game issues, keys on object that is having the issue (which may be a game object, or a task, but it must be something savable)
        /// and then the issue key (so that an object can have multiple issues)
        /// </summary>
        private Dictionary<ISavable, Dictionary<string, Issue>> _issuesDictionary = new Dictionary<ISavable, Dictionary<string, Issue>>();

        #endregion

        #region Setup

        public IssueManager() { }

        #endregion

        #region Properties

        /// <summary>
        /// Return all issues in no particular order
        /// Returns tuples with the object having the issue, and the actual issue
        /// </summary>
        public List<Issue> AllIssues()
        {
            List<Issue> toRet = new List<Issue>();
            foreach (ISavable issueObject in _issuesDictionary.Keys)
            {
                foreach (Issue issue in _issuesDictionary[issueObject].Values)
                {
                    toRet.Add(issue);
                }
            }
            return toRet;
        }

        #endregion

        #region Logic

        
        /// <summary>
        /// Report an issue.
        /// The issue will be shown to the player.
        /// The object and key together make a unique key for the issue, and can be used to clear or update the issue.
        /// Passing empty string as the issue will cause the issue to be cleared.
        /// </summary>
        public void ReportIssue(ISavable hasIssue, string key, string descirption)
        {
            ReportIssue(hasIssue, key, descirption, null);
        }

        /// <summary>
        /// Report an issue.
        /// The issue will be shown to the player.
        /// The object and key together make a unique key for the issue, and can be used to clear or update the issue.
        /// Passing empty string as the issue will cause the issue to be cleared.
        /// </summary>
        public void ReportIssue(ISavable hasIssue, string key, string descirption, Location location)
        {
            //clear if descirption is blank
            if (descirption == "")
            {
                ClearIssue(hasIssue, key);
                return;
            }                       

            //setup the issue dictionary
            if (_issuesDictionary.ContainsKey(hasIssue) == false)
            {
                _issuesDictionary.Add(hasIssue, new Dictionary<string, Issue>());
            }
            if (_issuesDictionary[hasIssue].ContainsKey(key) == false)
            {
                _issuesDictionary[hasIssue].Add(key, null);
            }

            //check if the issue changed, if it not just return
            if (_issuesDictionary[hasIssue][key] != null && _issuesDictionary[hasIssue][key].Description == descirption && _issuesDictionary[hasIssue][key].Location == location)
            {
                return;
            }

            //set the issue
            Issue issue = new Issue(hasIssue, key, descirption, location);
            _issuesDictionary[hasIssue][key] = issue;

            if (IssuesListChanged != null)
            {
                IssuesListChanged();
            }
        }

        /// <summary>
        /// Clear the issue with the object and key passed
        /// If this issue does not eixsist nothing will happen.
        /// </summary>
        public void ClearIssue(ISavable obj, string key)
        {
            //make sure we have this issue
            if (_issuesDictionary.ContainsKey(obj) == false)
            {
                return;                
            }
            if (_issuesDictionary[obj].ContainsKey(key) == false)
            {
                return;
            }

            //remove the issue
            _issuesDictionary[obj].Remove(key);

            //if no more issues for the object remove the obj
            if (_issuesDictionary[obj].Count == 0)
            {
                _issuesDictionary.Remove(obj);
            }

            if (IssuesListChanged != null)
            {
                IssuesListChanged();
            }
        }

        #endregion

        #region Save Load
        public void WriteStateV1(StateWriterV1 writer)
        {
            //count the number of issues
            int issuesCount = 0;
            foreach (ISavable objWithIssues in _issuesDictionary.Keys)
            {
                Dictionary<string, Issue> objIssues = _issuesDictionary[objWithIssues];
                foreach (string issueKey in objIssues.Keys)
                {
                    issuesCount++;
                }
            }

            //write number of issues
            writer.WriteInt(issuesCount);

            //write each issue
            foreach (ISavable objWithIssues in _issuesDictionary.Keys)
            {
                Dictionary<string, Issue> objIssues = _issuesDictionary[objWithIssues];
                foreach (string issueKey in objIssues.Keys)
                {
                    Issue issue = objIssues[issueKey];

                    writer.WriteObject(objWithIssues);
                    writer.WriteString(issueKey);
                    writer.WriteObject(issue);
                    issuesCount++;
                }
            }
        }

        public void ReadStateV1(StateReaderV1 reader)
        {
            int issuesCount = reader.ReadInt();

            for (int i = 0; i < issuesCount; i++)
            {
                ISavable objWithIssues = reader.ReadObject();
                string issueKey = reader.ReadString();
                Issue issue = reader.ReadObject<Issue>();

                //add to the dictionary
                ReportIssue(objWithIssues, issueKey, issue.Description, issue.Location);
            }

        }

        public void AfterReadStateV1()
        {
        }
        #endregion
        
    }
}
