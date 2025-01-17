using System;
using System.Collections.Generic;
using Ableton.SDK.Utils;
using Ableton.SDK.Factories;

namespace Ableton.SDK
{
    public static class SchemaAwareProjectMerger
    {
        /// <summary>
        /// Merges multiple "AbletonProject" objects (from possibly different MinorVersions)
        /// into a single new "AbletonProject" of the specified targetMinorVersion.
        /// 
        /// Currently demonstrates a simple "Tracks" merge example, but can be extended.
        /// </summary>
        public static object MergeProjects(IEnumerable<object> projects, string targetMinorVersion)
        {
            // 1) Create the blank target project
            var mergedProject = SchemaAwareProjectFactory.CreateBlankProject(targetMinorVersion);

            // 2) Grab "LiveSet" from merged project
            var mergedLiveSet = ReflectionHelper.GetPropValue(mergedProject, "LiveSet");
            if (mergedLiveSet == null)
                throw new Exception($"Could not find LiveSet on merged project for MinorVersion={targetMinorVersion}");

            // 3) Ensure there's a "Tracks" list
            var mergedTracks = EnsureListProperty(mergedLiveSet, "Tracks");

            // 4) For each source project
            foreach (var proj in projects)
            {
                if (proj == null) continue;

                // 4a) Get that project’s LiveSet
                var sourceLiveSet = ReflectionHelper.GetPropValue(proj, "LiveSet");
                if (sourceLiveSet == null) continue;

                // 4b) Get its "Tracks"
                var sourceTracks = ReflectionHelper.GetPropValue(sourceLiveSet, "Tracks");
                if (sourceTracks == null) continue;

                // 4c) Concat
                ReflectionHelper.ConcatLists(sourceTracks, mergedTracks);
            }

            return mergedProject;
        }

        /// <summary>
        /// Helper: ensures the given propertyName on obj is a list, and returns that list.
        /// If null, we create a new instance via reflection and set it.
        /// </summary>
        private static object EnsureListProperty(object obj, string propertyName)
        {
            var propVal = ReflectionHelper.GetPropValue(obj, propertyName);
            if (propVal == null)
            {
                // create
                var propInfo = obj.GetType().GetProperty(propertyName);
                if (propInfo != null)
                {
                    var newList = ReflectionHelper.CreateInstance(propInfo.PropertyType);
                    if (newList != null)
                    {
                        ReflectionHelper.SetPropValue(obj, propertyName, newList);
                        propVal = newList;
                    }
                }
            }

            if (propVal == null)
                throw new InvalidOperationException($"Cannot ensure list property {propertyName} on {obj.GetType()}");
            return propVal;
        }
    }
}