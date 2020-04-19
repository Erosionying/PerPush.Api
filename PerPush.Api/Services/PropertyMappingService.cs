using PerPush.Api.Entities;
using PerPush.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerPush.Api.Services
{
    public class PropertyMappingService:IPropertyMappingService
    {
        private readonly Dictionary<string, PropertyMappingValue> paperPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {"Id",new PropertyMappingValue(new List<string>{"Id"}) },
                {"UserId",new PropertyMappingValue(new List<string>{"UserId"}) },
                {"Title",new PropertyMappingValue(new List<string>{"Title"}) },
                {"Description",new PropertyMappingValue(new List<string>{"Description"}) },
                {"Lable",new PropertyMappingValue(new List<string>{"Lable"}) },
                {"StartTime",new PropertyMappingValue(new List<string>{"StartTime"}, true) },
                {"Visitors",new PropertyMappingValue(new List<string>{"Visitors"}, true) },
                {"Likes",new PropertyMappingValue(new List<string>{"Likes"}, true) },
            };

        private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<PaperDto, Paper>(paperPropertyMapping));
        }
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            var propertyMappings = matchingMapping.ToList();

            if(propertyMappings.Count() == 1)
            {
                return propertyMappings.First().MappingDictionary;
            }

            throw new Exception($"No unique mapping relationship found!{typeof(TSource)},{typeof(TDestination)}");
        }
        public bool ValidMappingExists<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();
            if(string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }
            var fieldAfterSplit = fields.Split(",");
            foreach(var field in fieldAfterSplit)
            {
                var trimmedField = field.Trim();
                var indexOfFirstSpace = trimmedField.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ? trimmedField : trimmedField.Remove(indexOfFirstSpace);

                if(!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
