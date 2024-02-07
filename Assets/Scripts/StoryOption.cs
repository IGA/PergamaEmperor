using System.Collections.Generic;
using JetBrains.Annotations;

namespace PergamaApp
{
    public class StoryOption
    {
        public string Text { get; set; }
        public int? NextNodeId { get; set; }
        [CanBeNull] public string HandlePropertyName { get; set; }
        [CanBeNull] public object HandlePropertyValue { get; set; }
        [CanBeNull] public string RequiredKey { get; set; }
        [CanBeNull] public string RequiredValue { get; set; }

        public void Handle(Dictionary<string, object> state)
        {
            if (HandlePropertyName != null && HandlePropertyValue != null)
            {
                state[HandlePropertyName] = HandlePropertyValue;   
            }
        }
        
        public bool Required(Dictionary<string, object> state)
        {
            if (RequiredKey != null && RequiredValue != null)
            {
                if (!state.TryGetValue(RequiredKey, out var currentValue)) return false;
                return RequiredValue?.Equals(currentValue) ?? currentValue == null;   
            }
            return true;
        }

    }
}