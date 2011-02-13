using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using T3ME.Domain.Repositories;
using Twitterizer;
using T3ME.Domain.Models;
using MvcLibrary.AutoMapper;

namespace T3ME.Business.Mapping
{
    public class TwitterUserToTweeterTypeConverter : AdvancedTypeConverter<TwitterUser, Tweeter>
    {
        protected readonly ILanguageRepository LanguageRepository = null;

        public TwitterUserToTweeterTypeConverter(ILanguageRepository languageRepository)
        {
            LanguageRepository = languageRepository;
        }

        protected override Tweeter ConvertCore(TwitterUser source)
        {
            Tweeter tweeter = ExistingDestination ?? new Tweeter();
            tweeter.TwitterId = (long)source.Id;
            tweeter.Username = source.ScreenName;
            tweeter.FullName = source.Name;
            tweeter.ImageUrl = source.ProfileImageLocation;

            string isoCode = source.Language;

            if (!string.IsNullOrWhiteSpace(isoCode))
            {
                Language language = LanguageRepository.Query().FirstOrDefault(l => l.Code == isoCode);

                // TODO: Should this be here?
                if (language == null)
                {
                    language = new Language();
                    language.Code = isoCode.ToLowerInvariant();
                    language.Name = null;
                    language.IsRecognised = false;

                    LanguageRepository.Save(language);
                }

                tweeter.Language = language;
            }

            tweeter.BackgroundImageUrl = source.ProfileBackgroundImageLocation;
            tweeter.IsBackgroundImageTiled = source.IsProfileBackgroundTiled.GetValueOrDefault(false);

            tweeter.Bio = source.Description;
            tweeter.Website = source.Website;
            tweeter.Location = source.Location;

            tweeter.NumberOfTweets = (int)source.NumberOfStatuses;
            tweeter.NumberOfFollowers = (int)source.NumberOfFollowers;
            tweeter.NumberFollowing = (int)source.NumberOfFriends;
            tweeter.NumberListed = (int)source.NumberOfFavorites;

            tweeter.TimeZoneOffset = (int)source.TimeZoneOffset.GetValueOrDefault(0.0);

            tweeter.IsProtected = source.IsProtected;

            return tweeter;
        }
    }
}