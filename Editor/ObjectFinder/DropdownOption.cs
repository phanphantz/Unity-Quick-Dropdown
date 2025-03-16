using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace PhEngine.QuickDropdown.Editor
{
    [Serializable]
    public class DropdownOption
    {
        public string text;
        public List<DropdownOption> options = new List<DropdownOption>();
		public DropdownOption(string text)
		{
			this.text = text;
		}

		public bool IsGroup => options.Count > 0;

		public void AddPath(IEnumerable<string> pathSegments)
		{
			var currentLevel = this;
			foreach (var segment in pathSegments)
			{
				var existingOption = currentLevel.options.FirstOrDefault(o => o.text == segment);
				if (existingOption == null)
				{
					existingOption = new DropdownOption(segment);
					currentLevel.options.Add(existingOption);
				}
				currentLevel = existingOption;
			}
		}

		public static DropdownOption[] FromFlatPaths(string[] flatPaths)
		{
			var root = new DropdownOption(string.Empty);
			foreach (var path in flatPaths)
			{
				var segments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				root.AddPath(segments);
			}
			return root.options.ToArray();
		}
    }
}