using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;

namespace FacebookApp
{
    public class FacebookAppFacade : IEnumerable<User>
    {     
        internal LoginResult        m_LoginResult;
        internal UserProxy          m_LoggedInUserProxy;
        private IFeature            m_CurrentFeature;

        public AppSettings AppSettings { get; set; }

        public void LoginAndInit()
        {
            AppSettings.LoginAndInit(out this.m_LoginResult, out this.m_LoggedInUserProxy);
        }

        public void PostStatus(string i_NewPost)
        {         
            if (!string.IsNullOrEmpty(i_NewPost))
            {
                this.m_CurrentFeature = FeaturesFactoryMethod.Create(MethodFactoryTypes.eFacebookFeatureType.Post, this.m_LoggedInUserProxy);
                (this.m_CurrentFeature as PostNewStatus).NewPost = i_NewPost;
                this.m_CurrentFeature.FeatureCliked();
            }
        }

        public void SortLikes(int i_CollectionIndexSelected)
        {
            if (i_CollectionIndexSelected >= 0)
            {
                this.m_CurrentFeature = FeaturesFactoryMethod.Create(MethodFactoryTypes.eFacebookFeatureType.LikeSorter, this.m_LoggedInUserProxy);
                (this.m_CurrentFeature as LikeSorter).SortCollectionIndexSelected = i_CollectionIndexSelected;
                this.m_CurrentFeature.FeatureCliked();
            }
            else
            {
                throw new Exception("No Collection Selected");
            }
        }

        public void FindMatch(bool i_IsFemaleChecked, bool i_IsMaleChecked, string i_AgeRangeSelected)
        {
            this.m_CurrentFeature = FeaturesFactoryMethod.Create(MethodFactoryTypes.eFacebookFeatureType.MatchFinder, this.m_LoggedInUserProxy);
            (this.m_CurrentFeature as MatchFinder).MatchInitializer(m_CurrentFeature as MatchFinder, i_IsFemaleChecked, i_IsMaleChecked, i_AgeRangeSelected);

            if ((i_IsFemaleChecked || i_IsMaleChecked) && !i_AgeRangeSelected.Equals("Select your desired age range"))
            {
                (this.m_CurrentFeature as MatchFinder).FeatureCliked();
            }
            else
            {
                throw new Exception("Not all parameters are selected");
            }
        }

        public List<PostAdapter> GetPosts()
        {
            return PostAdapter.CreateAdapterPosts(this.m_LoggedInUserProxy.LoggedInUser.Posts);
        }

        public IEnumerator<User> GetEnumerator()
        {
            if (this.m_CurrentFeature is IReturnableCollection)
            {
                foreach (User user in (this.m_CurrentFeature as IReturnableCollection).ReturnCollection())
                {
                    yield return user;
                }
            }
            else
            {
                throw new Exception("Current feature is not a returnable collection");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        } 
    }
}