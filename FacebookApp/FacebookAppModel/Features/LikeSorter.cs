using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;
using System.Collections;

namespace FacebookApp
{
    public class LikeSorter : IFeature, IReturnableCollection
    {
        private const int                   k_StartAmountOfLikes = 1;
        private CollectionMenu              m_CollectionMenu;
        private Dictionary<User, int>       m_FriendsLikeCounter;

        public LikeSorter(UserProxy i_LoggedInUser)
        {
            this.m_FriendsLikeCounter = new Dictionary<User, int>();
            this.LoggedInUser = i_LoggedInUser;
            this.initMenuCollectionList();
        }

        public int SortCollectionIndexSelected { get; set; }

        public UserProxy LoggedInUser { get; set; }

        public ICollection ReturnCollection()
        {
            return this.m_FriendsLikeCounter.Keys;
        }

        public void FeatureCliked()
        {
            this.m_CollectionMenu[this.SortCollectionIndexSelected].Selected();
        }

        public void RunPostLikeSorter()
        {
            try
            {
                for (int i = 0; i < this.LoggedInUser.LoggedInUser.Posts.Count; i++)
                {
                    this.sortCollection(this.LoggedInUser.LoggedInUser.Posts[i].LikedBy);                   
                }

                this.sortedDictionary();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RunPhotosLikeSorter()
        {
            try
            {
                for (int i = 0; i < this.LoggedInUser.LoggedInUser.Albums.Count; i++)
                {
                    for (int j = 0; j < this.LoggedInUser.LoggedInUser.Albums[i].Count; j++)
                    {
                        this.sortCollection(this.LoggedInUser.LoggedInUser.Albums[i].Photos[j].LikedBy);                   
                    }
                }

                this.sortedDictionary();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RunStatusLikeSorter()
        {
            try
            {
                for (int i = 0; i < this.LoggedInUser.LoggedInUser.Statuses.Count; i++)
                {
                    this.sortCollection(this.LoggedInUser.LoggedInUser.Statuses[i].LikedBy);
                }

                this.sortedDictionary();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void sortCollection(FacebookObjectCollection<User> i_LikedByCollection)
        {
            foreach (User user in i_LikedByCollection)
            {
                if (this.isFriendInDictionary(user.Name))
                {
                    this.updateRecordInDictionary(user);
                }
                else
                {
                    this.m_FriendsLikeCounter.Add(user, k_StartAmountOfLikes);
                }
            }
        }
        
        private bool isFriendInDictionary(string i_NameOfFriend)
        {
            bool isExist = false;

                foreach (User user in this.m_FriendsLikeCounter.Keys)
                {
                    if (user.Name.Equals(i_NameOfFriend))
                    {
                        isExist = true;
                        break;
                    }
                }
            
            return isExist;
        }

        private void updateRecordInDictionary(User i_UserToUpdate)
    {
        foreach (User userInDictionary in this.m_FriendsLikeCounter.Keys)
        {
            if (userInDictionary.Name.Equals(i_UserToUpdate.Name))
            {
                int currentUserAmountOfLikesInDictionary = this.m_FriendsLikeCounter[userInDictionary];
                currentUserAmountOfLikesInDictionary++;
                this.m_FriendsLikeCounter[userInDictionary] = currentUserAmountOfLikesInDictionary;
                break;
            }
        }
    }

        private void sortedDictionary()
        {
            try
            {
                this.m_FriendsLikeCounter.OrderByDescending(likeAmount => likeAmount.Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void initMenuCollectionList()
        {
            this.m_CollectionMenu = new CollectionMenu()
            {
            new CollectionMenuItem(this.RunPhotosLikeSorter, "Photos"),
            new CollectionMenuItem(this.RunPostLikeSorter, "Posts"),
            new CollectionMenuItem(this.RunStatusLikeSorter, "Statuses"),
            };
        }
    }
}