﻿namespace Badge.PostMentionsUmbraco
{
    using System.Linq;
    using MvcForum.Core.Interfaces.Badges;
    using MvcForum.Core.Models.Attributes;
    using MvcForum.Core.Models.Entities;

    [Id("9a247d50-35b5-4cd2-adaa-a0cf013325ac")]
    [Name("PostMentionsUmbraco")]
    [DisplayName("Badge.PostMentionsUmbraco.Name")]
    [Description("Badge.PostMentionsUmbraco.Desc")]
    [Image("MentionsUmbracoBadge.png")]
    [AwardsPoints(1)]
    public class PostMentionsUmbraco : IPostBadge
    {
        public bool Rule(MembershipUser user)
        {
            var lastPost = user.Posts.OrderByDescending(x => x.DateCreated).FirstOrDefault();
            if (lastPost != null && lastPost.PostContent.ToLower().Contains("umbraco"))
            {
                return true;
            }
            return false;
        }
    }
}