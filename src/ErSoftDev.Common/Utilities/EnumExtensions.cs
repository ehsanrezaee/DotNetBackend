using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ErSoftDev.Common.Utilities
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetEnumValues<T>(this T input) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new NotSupportedException();

            return Enum.GetValues(input.GetType()).Cast<T>();
        }

        public static int ToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }


        public static string ToDisplay(this Enum value, DisplayProperty property = DisplayProperty.Name)
        {
            var attribute = value.GetType().GetField(value.ToString()).GetCustomAttributes<DisplayAttribute>(false).FirstOrDefault();

            if (attribute == null)
                return value.ToString();

            var propValue = attribute.GetType().GetProperty(property.ToString()).GetValue(attribute, null);
            return propValue.ToString();
        }

        public static Dictionary<int, string> ToDictionary(this Enum value)
        {
            return Enum.GetValues(value.GetType()).Cast<Enum>().ToDictionary(p => Convert.ToInt32(p), q => ToDisplay(q));
        }

        public static bool IsValidEnum(this Enum value)
        {
            return value.GetType().GetField(value.ToString()) != null;
        }
        public static string GetStringValue(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            // Get the stringvalue attributes  
            EnumStringAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(EnumStringAttribute), false) as EnumStringAttribute[];
            // Return the first if there was a match.  
            return attribs.Length > 0 ? attribs[0].StringValue : null;
        }

    }

    public enum DisplayProperty
    {
        Description,
        GroupName,
        Name,
        Prompt,
        ShortName,
        Order
    }

    public class EnumStringAttribute : Attribute
    {
        public EnumStringAttribute(string stringValue)
        {
            this.stringValue = stringValue;
        }
        private string stringValue;
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
