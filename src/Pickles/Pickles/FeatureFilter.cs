using System;
using System.Linq;
using PicklesDoc.Pickles.ObjectModel;

namespace PicklesDoc.Pickles
{
    internal class FeatureFilter
    {
        private readonly Feature feature;
        private readonly string excludeTags;
        private readonly string filterTags;

        public FeatureFilter(Feature feature, string excludeTags, string filterTags)
        {
            this.feature = feature;
            this.excludeTags = excludeTags;
            this.filterTags = filterTags;
        }

        public Feature ExcludeScenariosByTags()
        {
            if (this.FeatureShouldBeExcuded()
                || this.AllFeatureElementsShouldBeExcluded())
                return null;

            var filteredFeatures = string.IsNullOrEmpty(filterTags)|| FeatureShouldBeFullyUsed() ? this.feature.FeatureElements : this.feature.FeatureElements.Where(fe => fe.Tags.Any(tag => this.IsFilteredTag(tag))).ToList();

            if (filteredFeatures.Count==0)
            {
                return null;
            }

            var wantedFeatures = filteredFeatures.Where(fe => fe.Tags.All(tag => !this.IsExcludedTag(tag))).ToList();
            
            this.feature.FeatureElements.Clear();
            this.feature.FeatureElements.AddRange(wantedFeatures);

            return this.feature;
        }

        private bool FeatureShouldBeExcuded()
        {
            return this.feature.Tags.Any(this.IsExcludedTag);
        }

        private bool FeatureShouldBeFullyUsed()
        {
            return this.feature.Tags.Any(this.IsFilteredTag);
        }

        private bool AllFeatureElementsShouldBeExcluded()
        {
            return this.feature.FeatureElements.All(fe => fe.Tags.Any(this.IsExcludedTag));
        }

        private bool IsExcludedTag(string tag)
        {
            return tag.Equals($"@{this.excludeTags}", StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsFilteredTag(string tag)
        {
            return tag.Equals($"@{this.filterTags}", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}