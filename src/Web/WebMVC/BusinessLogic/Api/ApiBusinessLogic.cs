using Codecamp.Data;
using Codecamp.Models;
using Codecamp.Models.Api;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Codecamp.BusinessLogic.Api
{
    public class ApiBusinessLogic
    {
        protected ApiBusinessLogic(CodecampDbContext context, string imageFolder = "")
        {
            Context = context;
            ImageFolder = imageFolder;
        }

        protected CodecampDbContext Context { get; set; }

        #region Events

        public ApiEvent GetActiveEvent()
        {
            var webEvent = GetWebEvent();
            var apiEvent = GetApiEvent(webEvent);

            return apiEvent;
        }

        public ApiEvent GetEvent(int year)
        {
            var webEvent = GetWebEvent(year);
            var apiEvent = GetApiEvent(webEvent);

            return apiEvent;
        }

        private ApiEvent GetApiEvent(Event webEvent)
        {
            if (webEvent == null)
                return null;

            var apiEvent = new ApiEvent(webEvent);

            return apiEvent;
        }

        private Event GetWebEvent(int? year = null)
        {
            var webEvents = Context.Events
                .OrderByDescending(@event => @event.StartDateTime.Year);

            if (year != null)
                return webEvents.FirstOrDefault(
                    @event => @event.StartDateTime.Year == year);

            return webEvents.FirstOrDefault(@event => @event.IsActive);
        }

        #endregion

        #region Images (Speakers and Sponsors)

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        protected string ImageFolder { get; set; }

        protected Uri GetImageUrl(int id)
        {
            var imageUrl =
                new Uri($"/api/{ImageFolder}/{id}/image", UriKind.Relative);

            return imageUrl;
        }

        #endregion
    }
}