using System.Collections.Generic;

namespace bimeh_back.Components.Extensions
{
    public abstract class ModelExtension : SoftDeletes.ModelTools.ModelExtension
    {
        public long Id { get; set; }

        public void Fill(Dictionary<string, object> values, bool withNulls = false)
        {
            foreach (var property in GetType().GetProperties()) {
                if (!values.ContainsKey(property.Name)) continue;

                if (values[property.Name] == null && withNulls) {
                    property.SetValue(this, null);
                }

                if (values[property.Name] != null && values[property.Name].GetType() == property.PropertyType) {
                    property.SetValue(this, values[property.Name]);
                }
            }
        }
    }
}