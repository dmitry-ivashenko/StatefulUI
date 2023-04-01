using System.Collections.Generic;

namespace StatefulUI.Editor.Selector
{
    public class HierarchyFilter
    {
        public bool UpdateFilterRecursive(List<HierarchyEntry> entries, string nameFilter)
        {
            if (entries == null)
            {
                return false;
            }

            var result = false;
            foreach (var entry in entries)
            {
                if (entry.Children != null)
                {
                    entry.Hidden = !UpdateFilterRecursive(entry.Children, nameFilter);
                }
                else
                {
                    entry.Hidden = !MatchesName(entry, nameFilter);
                }

                result |= !entry.Hidden;
            }

            return result;
        }
		
        private static bool MatchesName(HierarchyEntry entry, string nameFilter)
        {
            if (string.IsNullOrEmpty(nameFilter)) return true;
            var prettyFilter = nameFilter.Replace("\\", "/");
            return StringUtils.MatchesFilter(entry.Name, prettyFilter);
        }
    }
}