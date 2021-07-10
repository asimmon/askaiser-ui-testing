﻿using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Askaiser.UITesting
{
    public sealed class SearchResult : IEnumerable<Rectangle>
    {
        public IElement Element { get; }

        public IReadOnlyList<Rectangle> Areas { get; }

        public bool Success { get; }

        public Rectangle Area
        {
            get
            {
                this.EnsureSingleLocation();
                return this.Areas[0];
            }
        }

        internal SearchResult(IElement element, IEnumerable<Rectangle> areas)
        {
            this.Element = element;
            this.Areas = new List<Rectangle>(areas);
            this.Success = this.Areas.Count > 0;
        }

        public IEnumerator<Rectangle> GetEnumerator()
        {
            return this.Areas.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal SearchResult AdjustToMonitor(MonitorDescription monitor)
        {
            var newAreas = this.Areas.Select(x => x.AddOffset(monitor.Left, monitor.Top));
            return new SearchResult(this.Element, newAreas);
        }

        internal SearchResult AdjustToSearchRectangle(Rectangle rect)
        {
            if (rect == null)
                return this;

            var newAreas = this.Areas.Select(x => x.AddOffset(rect.Left, rect.Top));
            return new SearchResult(this.Element, newAreas);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Search results for '{0}': {1}", this.Element, this.Areas.Count > 0 ? this.Areas.ToCenterString() : "none");
        }

        public static SearchResult NotFound(IElement element)
        {
            return new SearchResult(element, Enumerable.Empty<Rectangle>());
        }

        public void EnsureSingleLocation()
        {
            if (this.Areas.Count == 1)
                return;

            if (this.Areas.Count == 0)
                throw new ElementNotFoundException(this.Element);

            throw new MultipleElementLocationsException(this);
        }
    }
}
