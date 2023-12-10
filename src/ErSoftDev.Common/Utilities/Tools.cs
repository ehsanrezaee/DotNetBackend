using System.Data;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Microsoft.Extensions.DependencyModel;
using SixLabors.ImageSharp;

namespace ErSoftDev.Common.Utilities
{
    public static class Tools
    {
        public static IEnumerable<Assembly> GetAllAssemblies()
        {
            var platform = Environment.OSVersion.Platform.ToString();
            var runtimeAssemblyNames = DependencyContext.Default!.GetRuntimeAssemblyNames(platform);

            var res = new List<Assembly>();

            foreach (var assembly in runtimeAssemblyNames)
            {
                try
                {
                    if (assembly.FullName == "Microsoft.Data.SqlClient")
                        continue;

                    res.Add(Assembly.Load(assembly.FullName));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            return res;
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);


            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var type = (prop.PropertyType.IsGenericType &&
                            prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? Nullable.GetUnderlyingType(prop.PropertyType)
                    : prop.PropertyType);
                dataTable.Columns.Add(prop.Name, type ?? throw new InvalidOperationException());
            }

            if (items.Count <= 0)
                return dataTable;

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                    values[i] = props[i].GetValue(item, null)!;
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static string ToXml(object input)
        {
            var response = "";
            try
            {
                var stringWriter = new StringWriter();
                var serializer = new XmlSerializer(input.GetType());
                serializer.Serialize(stringWriter, input);
                response = stringWriter.ToString();
            }
            catch
            {
                //Ignore
            }

            return response;
        }

        public static bool CheckEMail(string email)
        {
            bool result;
            try
            {
                var unused = new MailAddress(email);
                result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public static Image CovertByteArrayToImage(byte[] bytes)
        {
            return Image.Load(bytes);
        }

        public static async Task SaveImage(Image image, string path)
        {
            await image.SaveAsPngAsync(path);
        }

        public static DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(path);

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }

        public static long ConvertStringIpToLong(string ip)
        {
            return BitConverter.ToInt32(IPAddress.Parse(ip).GetAddressBytes().Reverse().ToArray(), 0);
        }

        /// <summary>
        /// Set any requested character to middle of string 
        /// </summary>
        /// <param name="value">string that must be changed</param>
        /// <param name="character">character that replaced in string</param>
        /// <param name="characterNotChangeCount">how many character from begin and end of string not change</param>
        /// <returns></returns>
        public static string SetMiddleOfStringWithCharacter(this string value, char character,
            int characterNotChangeCount)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            if (characterNotChangeCount * 2 > value.Length)
                return value;

            return value.Substring(0, characterNotChangeCount) +
                   new string(character, value.Length - 2 * characterNotChangeCount) +
                   value.Substring(value.Length - characterNotChangeCount);
        }

        public static bool CheckCellPhone(this string cellPhone)
        {
            return new Regex("0?9[0-9]{9}").IsMatch(cellPhone);
        }

    }
}