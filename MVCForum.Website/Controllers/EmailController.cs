﻿namespace MvcForum.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using Core.Models.Entities;
    using ViewModels.Email;

    public partial class EmailController : BaseController
    {
        private readonly ICategoryNotificationService _categoryNotificationService;
        private readonly ICategoryService _categoryService;
        private readonly ITagNotificationService _tagNotificationService;
        private readonly ITopicNotificationService _topicNotificationService;
        private readonly ITopicService _topicService;
        private readonly ITopicTagService _topicTagService;

        public EmailController(ILoggingService loggingService, IMembershipService membershipService,
            ILocalizationService localizationService, IRoleService roleService, ISettingsService settingsService,
            ITopicNotificationService topicNotificationService,
            ICategoryNotificationService categoryNotificationService, ICategoryService categoryService,
            ITopicService topicService, ITopicTagService topicTagService,
            ITagNotificationService tagNotificationService, ICacheService cacheService, IMvcForumContext context)
            : base(loggingService, membershipService, localizationService, roleService,
                settingsService, cacheService, context)
        {
            _topicNotificationService = topicNotificationService;
            _categoryNotificationService = categoryNotificationService;
            _categoryService = categoryService;
            _topicService = topicService;
            _topicTagService = topicTagService;
            _tagNotificationService = tagNotificationService;
        }

        [HttpPost]
        [Authorize]
        public void Subscribe(EmailSubscriptionViewModel subscription)
        {
            if (Request.IsAjaxRequest())
            {
                try
                {
                    // Add logic to add subscr
                    var isCategory = subscription.SubscriptionType.Contains("category");
                    var isTag = subscription.SubscriptionType.Contains("tag");
                    var id = subscription.Id;
                    var dbUser = MembershipService.GetUser(User.Identity.Name);

                    if (isCategory)
                    {
                        // get the category
                        var cat = _categoryService.Get(id);

                        if (cat != null)
                        {
                            // Create the notification
                            var categoryNotification = new CategoryNotification
                            {
                                Category = cat,
                                User = dbUser
                            };
                            //save

                            _categoryNotificationService.Add(categoryNotification);
                        }
                    }
                    else if (isTag)
                    {
                        // get the tag
                        var tag = _topicTagService.Get(id);

                        if (tag != null)
                        {
                            // Create the notification
                            var tagNotification = new TagNotification
                            {
                                Tag = tag,
                                User = dbUser
                            };
                            //save

                            _tagNotificationService.Add(tagNotification);
                        }
                    }
                    else
                    {
                        // get the category
                        var topic = _topicService.Get(id);

                        // check its not null
                        if (topic != null)
                        {
                            // Create the notification
                            var topicNotification = new TopicNotification
                            {
                                Topic = topic,
                                User = dbUser
                            };
                            //save

                            _topicNotificationService.Add(topicNotification);
                        }
                    }

                    Context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Context.RollBack();
                    LoggingService.Error(ex);
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            else
            {
                throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
            }
        }

        [HttpPost]
        [Authorize]
        public void UnSubscribe(EmailSubscriptionViewModel subscription)
        {
            if (Request.IsAjaxRequest())
            {
                try
                {
                    // Add logic to add subscr
                    var isCategory = subscription.SubscriptionType.Contains("category");
                    var isTag = subscription.SubscriptionType.Contains("tag");
                    var id = subscription.Id;
                    var dbUser = MembershipService.GetUser(User.Identity.Name);
                    if (isCategory)
                    {
                        // get the category
                        var cat = _categoryService.Get(id);

                        if (cat != null)
                        {
                            // get the notifications by user
                            var notifications =
                                _categoryNotificationService.GetByUserAndCategory(dbUser, cat, true);

                            if (notifications.Any())
                            {
                                foreach (var categoryNotification in notifications)
                                {
                                    // Delete
                                    _categoryNotificationService.Delete(categoryNotification);
                                }
                            }
                        }
                    }
                    else if (isTag)
                    {
                        // get the tag
                        var tag = _topicTagService.Get(id);

                        if (tag != null)
                        {
                            // get the notifications by user
                            var notifications =
                                _tagNotificationService.GetByUserAndTag(dbUser, tag, true);

                            if (notifications.Any())
                            {
                                foreach (var n in notifications)
                                {
                                    // Delete
                                    _tagNotificationService.Delete(n);
                                }
                            }
                        }
                    }
                    else
                    {
                        // get the topic
                        var topic = _topicService.Get(id);

                        if (topic != null)
                        {
                            // get the notifications by user
                            var notifications =
                                _topicNotificationService.GetByUserAndTopic(dbUser, topic, true);

                            if (notifications.Any())
                            {
                                foreach (var topicNotification in notifications)
                                {
                                    // Delete
                                    _topicNotificationService.Delete(topicNotification);
                                }
                            }
                        }
                    }

                    Context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Context.RollBack();
                    LoggingService.Error(ex);
                    throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
                }
            }
            else
            {
                throw new Exception(LocalizationService.GetResourceString("Errors.GenericMessage"));
            }
        }
    }
}