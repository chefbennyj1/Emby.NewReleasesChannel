using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.Serialization;

namespace NewReleases
{
    public class Channel : IChannel, IHasCacheKey, IRequiresMediaInfoCallback
    {
        private IHttpClient HttpClient         { get; set; }
        private ILogger Logger                 { get; set; }
        private IJsonSerializer JsonSerializer { get; set; }
        private ILibraryManager LibraryManager { get; set; }

        public Channel(IHttpClient httpClient, ILogManager logManager, IJsonSerializer jsonSerializer, ILibraryManager lib)
        {
            HttpClient     = httpClient;
            Logger         = logManager.GetLogger(GetType().Name);
            JsonSerializer = jsonSerializer;
            LibraryManager = lib;
        }

        public string DataVersion => "12";

        public InternalChannelFeatures GetChannelFeatures()
        {
            return new InternalChannelFeatures
            {
                ContentTypes = new List<ChannelMediaContentType>
                {
                    ChannelMediaContentType.Movie
                },

                MediaTypes = new List<ChannelMediaType>
                {
                    ChannelMediaType.Video
                },

                SupportsContentDownloading = true,
            };
        }

        private Task<ChannelItemResult> GetChannelItemsInternal()
        {
            Plugin.Instance.UpdateConfiguration(Plugin.Instance.Configuration);
            var config = Plugin.Instance.Configuration;
            var channel = LibraryManager.GetItemList(new InternalItemsQuery()
            {
                Name = "New Releases"
            });
            // ReSharper disable once ComplexConditionExpression
            var ids = LibraryManager.GetInternalItemIds(new InternalItemsQuery()
            {
                IncludeItemTypes = new[] {"Movie"},
                MinPremiereDate =  config.MinPremiereDate != null ? DateTimeOffset.Parse(config.MinPremiereDate) : DateTimeOffset.Parse("1/1/"+DateTime.Now.Year),
            });

            var libraryItems = LibraryManager.GetInternalItemIds(new InternalItemsQuery()
            {
                ParentIds = new[] {channel[0].InternalId}
            }).ToList();

            var sortedList = new List<long>();

            foreach (var i in ids)
            {
                if (!libraryItems.Exists(id => id == i))
                {
                    sortedList.Add(i);
                }
            }

            var items = sortedList.Select(id => LibraryManager.GetItemById(id))
                .Select(movie => new ChannelItemInfo
                {
                    Name          = movie.Name,
                    ImageUrl      = movie.PrimaryImagePath,
                    Id            = movie.Id.ToString(),
                    Type          = ChannelItemType.Media,
                    ContentType   = ChannelMediaContentType.Movie,
                    MediaType     = ChannelMediaType.Video,
                    IsLiveStream  = false,
                    MediaSources  = new List<MediaSourceInfo> {new ChannelMediaInfo {Path = movie.Path, Protocol = MediaProtocol.File}.ToMediaSource()},
                    OriginalTitle = movie.OriginalTitle
                });

            return Task.FromResult(new ChannelItemResult
            {
                Items = items.ToList()
            });
        }

        public bool IsEnabledFor(string userId)
        {
            return true;
        }

        public async Task<ChannelItemResult> GetChannelItems(InternalChannelItemQuery query, CancellationToken cancellationToken)
        {
            return await GetChannelItemsInternal();
        }

        public async Task<DynamicImageResponse> GetChannelImage(ImageType type, CancellationToken cancellationToken)
        {
            switch (type)
            {
                case ImageType.Backdrop:
                    {
                        var path = GetType().Namespace + ".Images." + type.ToString().ToLower() + ".png";

                        return await Task.FromResult(new DynamicImageResponse
                        {
                            Format = ImageFormat.Png,
                            HasImage = true,
                            Stream = GetType().Assembly.GetManifestResourceStream(path)
                        });
                    }
                default:
                    throw new ArgumentException("Unsupported image type: " + type);

            }
        }

        public IEnumerable<ImageType> GetSupportedChannelImages()
        {
            return new List<ImageType>() { ImageType.Primary };
        }

        public string Name => "New Releases";
        public string Description { get; private set; }
        public string HomePageUrl
        {
            get { return ""; }
        }

        public ChannelParentalRating ParentalRating
        {
            get { return ChannelParentalRating.GeneralAudience; }
        }

        public string GetCacheKey(string userId)
        {
            return Guid.NewGuid().ToString("N");
        }

        public Task<IEnumerable<MediaSourceInfo>> GetChannelItemMediaInfo(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
